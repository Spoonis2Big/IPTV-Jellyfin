using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.XtreamCodes.XtreamAPI;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.XtreamCodes.Providers;

/// <summary>
/// Series provider for Xtream Codes.
/// </summary>
public class XtreamSeriesProvider : IRemoteMetadataProvider<Series, SeriesInfo>
{
    private readonly ILogger<XtreamSeriesProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="XtreamSeriesProvider"/> class.
    /// </summary>
    /// <param name="logger">Instance of the <see cref="ILogger{XtreamSeriesProvider}"/> interface.</param>
    public XtreamSeriesProvider(ILogger<XtreamSeriesProvider> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public string Name => "Xtream Codes Series";

    /// <inheritdoc />
    public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeriesInfo searchInfo, CancellationToken cancellationToken)
    {
        var results = new List<RemoteSearchResult>();
        var config = Plugin.Instance?.Configuration;

        if (config?.Servers == null)
        {
            return results;
        }

        foreach (var server in config.Servers.Where(s => s.ImportSeries))
        {
            try
            {
                using var client = new XtreamApiClient(server.Url, server.Username, server.Password);
                var series = await client.GetSeriesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

                if (series == null)
                {
                    continue;
                }

                var matches = series.Where(s =>
                    s.Name != null &&
                    s.Name.Contains(searchInfo.Name ?? string.Empty, StringComparison.OrdinalIgnoreCase));

                foreach (var show in matches)
                {
                    var result = new RemoteSearchResult
                    {
                        Name = show.Name,
                        ImageUrl = show.Cover,
                        SearchProviderName = Name,
                        ProductionYear = ParseYear(show.ReleaseDate)
                    };

                    result.SetProviderId("XtreamSeries", $"{server.Name}_{show.SeriesId}");
                    results.Add(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching series from {Server}", server.Name);
            }
        }

        return results;
    }

    /// <inheritdoc />
    public async Task<MetadataResult<Series>> GetMetadata(SeriesInfo info, CancellationToken cancellationToken)
    {
        var result = new MetadataResult<Series>();
        var config = Plugin.Instance?.Configuration;

        if (config?.Servers == null)
        {
            return result;
        }

        var providerId = info.GetProviderId("XtreamSeries");
        if (string.IsNullOrEmpty(providerId))
        {
            return result;
        }

        // Parse provider ID: {serverName}_{seriesId}
        var parts = providerId.Split('_');
        if (parts.Length < 2)
        {
            return result;
        }

        var seriesId = parts[^1];
        var serverName = string.Join("_", parts.Take(parts.Length - 1));

        var server = config.Servers.FirstOrDefault(s => s.Name == serverName);
        if (server == null)
        {
            return result;
        }

        try
        {
            using var client = new XtreamApiClient(server.Url, server.Username, server.Password);
            var seriesInfo = await client.GetSeriesInfoAsync(seriesId, cancellationToken).ConfigureAwait(false);

            if (seriesInfo?.Info == null)
            {
                return result;
            }

            var series = new Series
            {
                Name = seriesInfo.Info.Name ?? "Unknown",
                Overview = seriesInfo.Info.Plot,
                ProductionYear = ParseYear(seriesInfo.Info.ReleaseDate)
            };

            series.SetProviderId("XtreamSeries", providerId);

            // Set genres
            if (!string.IsNullOrEmpty(seriesInfo.Info.Genre))
            {
                series.Genres = seriesInfo.Info.Genre.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(g => g.Trim())
                    .ToArray();
            }

            // Set rating
            if (!string.IsNullOrEmpty(seriesInfo.Info.Rating) && double.TryParse(seriesInfo.Info.Rating, out var rating))
            {
                series.CommunityRating = (float)rating;
            }

            result.Item = series;
            result.HasMetadata = true;

            _logger.LogInformation("Retrieved metadata for series: {Name}", series.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving series metadata from {Server}", server.Name);
        }

        return result;
    }

    /// <inheritdoc />
    public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        var httpClient = new HttpClient();
        return httpClient.GetAsync(url, cancellationToken);
    }

    private static int? ParseYear(string? dateString)
    {
        if (string.IsNullOrEmpty(dateString))
        {
            return null;
        }

        if (DateTime.TryParse(dateString, out var date))
        {
            return date.Year;
        }

        if (int.TryParse(dateString.AsSpan(0, Math.Min(4, dateString.Length)), out var year))
        {
            return year;
        }

        return null;
    }
}

/// <summary>
/// Episode provider for Xtream Codes.
/// </summary>
public class XtreamEpisodeProvider : IRemoteMetadataProvider<Episode, EpisodeInfo>
{
    private readonly ILogger<XtreamEpisodeProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="XtreamEpisodeProvider"/> class.
    /// </summary>
    /// <param name="logger">Instance of the <see cref="ILogger{XtreamEpisodeProvider}"/> interface.</param>
    public XtreamEpisodeProvider(ILogger<XtreamEpisodeProvider> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public string Name => "Xtream Codes Episodes";

    /// <inheritdoc />
    public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(EpisodeInfo searchInfo, CancellationToken cancellationToken)
    {
        // Episodes are typically loaded through series, not searched individually
        return Task.FromResult(Enumerable.Empty<RemoteSearchResult>());
    }

    /// <inheritdoc />
    public async Task<MetadataResult<Episode>> GetMetadata(EpisodeInfo info, CancellationToken cancellationToken)
    {
        var result = new MetadataResult<Episode>();
        var config = Plugin.Instance?.Configuration;

        if (config?.Servers == null)
        {
            return result;
        }

        var providerId = info.GetProviderId("XtreamEpisode");
        if (string.IsNullOrEmpty(providerId))
        {
            return result;
        }

        // Parse provider ID: {serverName}_{seriesId}_{episodeId}
        var parts = providerId.Split('_');
        if (parts.Length < 3)
        {
            return result;
        }

        var episodeId = parts[^1];
        var seriesId = parts[^2];
        var serverName = string.Join("_", parts.Take(parts.Length - 2));

        var server = config.Servers.FirstOrDefault(s => s.Name == serverName);
        if (server == null)
        {
            return result;
        }

        try
        {
            using var client = new XtreamApiClient(server.Url, server.Username, server.Password);
            var seriesInfo = await client.GetSeriesInfoAsync(seriesId, cancellationToken).ConfigureAwait(false);

            if (seriesInfo?.Episodes == null)
            {
                return result;
            }

            // Find the episode
            XtreamEpisode? episodeData = null;
            foreach (var seasonEpisodes in seriesInfo.Episodes.Values)
            {
                episodeData = seasonEpisodes.FirstOrDefault(e => e.Id == episodeId);
                if (episodeData != null)
                {
                    break;
                }
            }

            if (episodeData == null)
            {
                return result;
            }

            var episode = new Episode
            {
                Name = episodeData.Title ?? "Unknown",
                IndexNumber = episodeData.EpisodeNum,
                ParentIndexNumber = episodeData.Season,
                Overview = episodeData.Info?.Plot
            };

            if (episodeData.Info != null && !string.IsNullOrEmpty(episodeData.Info.ReleaseDate))
            {
                if (DateTime.TryParse(episodeData.Info.ReleaseDate, out var airDate))
                {
                    episode.PremiereDate = airDate;
                }
            }

            episode.SetProviderId("XtreamEpisode", providerId);

            result.Item = episode;
            result.HasMetadata = true;

            _logger.LogInformation("Retrieved metadata for episode: {Name}", episode.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving episode metadata from {Server}", server.Name);
        }

        return result;
    }

    /// <inheritdoc />
    public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        var httpClient = new HttpClient();
        return httpClient.GetAsync(url, cancellationToken);
    }
}
