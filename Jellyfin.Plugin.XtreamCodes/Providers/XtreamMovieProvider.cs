using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.XtreamCodes.XtreamAPI;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.XtreamCodes.Providers;

/// <summary>
/// Movie provider for Xtream Codes VOD.
/// </summary>
public class XtreamMovieProvider : IRemoteMetadataProvider<Movie, MovieInfo>
{
    private readonly ILogger<XtreamMovieProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="XtreamMovieProvider"/> class.
    /// </summary>
    /// <param name="logger">Instance of the <see cref="ILogger{XtreamMovieProvider}"/> interface.</param>
    public XtreamMovieProvider(ILogger<XtreamMovieProvider> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public string Name => "Xtream Codes VOD";

    /// <inheritdoc />
    public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo searchInfo, CancellationToken cancellationToken)
    {
        var results = new List<RemoteSearchResult>();
        var config = Plugin.Instance?.Configuration;

        if (config?.Servers == null)
        {
            return results;
        }

        foreach (var server in config.Servers.Where(s => s.ImportVod))
        {
            try
            {
                using var client = new XtreamApiClient(server.Url, server.Username, server.Password);
                var vodStreams = await client.GetVodStreamsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

                if (vodStreams == null)
                {
                    continue;
                }

                var matches = vodStreams.Where(v =>
                    v.Name != null &&
                    v.Name.Contains(searchInfo.Name ?? string.Empty, StringComparison.OrdinalIgnoreCase));

                foreach (var vod in matches)
                {
                    var result = new RemoteSearchResult
                    {
                        Name = vod.Name,
                        ImageUrl = vod.StreamIcon,
                        SearchProviderName = Name,
                        ProductionYear = null
                    };

                    result.SetProviderId("XtreamVod", $"{server.Name}_{vod.StreamId}");
                    results.Add(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching VOD from {Server}", server.Name);
            }
        }

        return results;
    }

    /// <inheritdoc />
    public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info, CancellationToken cancellationToken)
    {
        var result = new MetadataResult<Movie>();
        var config = Plugin.Instance?.Configuration;

        if (config?.Servers == null)
        {
            return result;
        }

        var providerId = info.GetProviderId("XtreamVod");
        if (string.IsNullOrEmpty(providerId))
        {
            return result;
        }

        // Parse provider ID: {serverName}_{vodId}
        var parts = providerId.Split('_');
        if (parts.Length < 2)
        {
            return result;
        }

        var vodId = parts[^1];
        var serverName = string.Join("_", parts.Take(parts.Length - 1));

        var server = config.Servers.FirstOrDefault(s => s.Name == serverName);
        if (server == null)
        {
            return result;
        }

        try
        {
            using var client = new XtreamApiClient(server.Url, server.Username, server.Password);
            var vodInfo = await client.GetVodInfoAsync(vodId, cancellationToken).ConfigureAwait(false);

            if (vodInfo?.Info == null)
            {
                return result;
            }

            var movie = new Movie
            {
                Name = vodInfo.Info.Name ?? "Unknown",
                Overview = vodInfo.Info.Plot,
                ProductionYear = ParseYear(vodInfo.Info.ReleaseDate)
            };

            if (!string.IsNullOrEmpty(vodInfo.Info.TmdbId))
            {
                movie.SetProviderId(MetadataProvider.Tmdb, vodInfo.Info.TmdbId);
            }

            movie.SetProviderId("XtreamVod", providerId);

            // Set genres
            if (!string.IsNullOrEmpty(vodInfo.Info.Genre))
            {
                movie.Genres = vodInfo.Info.Genre.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(g => g.Trim())
                    .ToArray();
            }

            // Set rating
            if (!string.IsNullOrEmpty(vodInfo.Info.Rating) && double.TryParse(vodInfo.Info.Rating, out var rating))
            {
                movie.CommunityRating = (float)rating;
            }

            result.Item = movie;
            result.HasMetadata = true;

            _logger.LogInformation("Retrieved metadata for VOD: {Name}", movie.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving VOD metadata from {Server}", server.Name);
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
