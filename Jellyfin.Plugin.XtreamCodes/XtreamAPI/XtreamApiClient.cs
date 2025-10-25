using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.XtreamCodes.XtreamAPI;

/// <summary>
/// Client for interacting with Xtream Codes API.
/// </summary>
public class XtreamApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _username;
    private readonly string _password;

    /// <summary>
    /// Initializes a new instance of the <see cref="XtreamApiClient"/> class.
    /// </summary>
    /// <param name="baseUrl">The base URL of the Xtream server.</param>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    public XtreamApiClient(string baseUrl, string username, string password)
    {
        _httpClient = new HttpClient();
        _baseUrl = baseUrl.TrimEnd('/');
        _username = username;
        _password = password;
    }

    /// <summary>
    /// Gets the player API authentication info.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authentication response.</returns>
    public async Task<XtreamAuthResponse?> GetAuthInfoAsync(CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/player_api.php?username={_username}&password={_password}";
        var response = await _httpClient.GetStringAsync(url, cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<XtreamAuthResponse>(response);
    }

    /// <summary>
    /// Gets the list of live TV categories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of live TV categories.</returns>
    public async Task<List<XtreamCategory>?> GetLiveTvCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/player_api.php?username={_username}&password={_password}&action=get_live_categories";
        var response = await _httpClient.GetStringAsync(url, cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<XtreamCategory>>(response);
    }

    /// <summary>
    /// Gets the list of live TV streams.
    /// </summary>
    /// <param name="categoryId">Optional category ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of live TV streams.</returns>
    public async Task<List<XtreamLiveStream>?> GetLiveStreamsAsync(string? categoryId = null, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/player_api.php?username={_username}&password={_password}&action=get_live_streams";
        if (!string.IsNullOrEmpty(categoryId))
        {
            url += $"&category_id={categoryId}";
        }

        var response = await _httpClient.GetStringAsync(url, cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<XtreamLiveStream>>(response);
    }

    /// <summary>
    /// Gets the list of VOD categories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of VOD categories.</returns>
    public async Task<List<XtreamCategory>?> GetVodCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/player_api.php?username={_username}&password={_password}&action=get_vod_categories";
        var response = await _httpClient.GetStringAsync(url, cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<XtreamCategory>>(response);
    }

    /// <summary>
    /// Gets the list of VOD streams.
    /// </summary>
    /// <param name="categoryId">Optional category ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of VOD streams.</returns>
    public async Task<List<XtreamVodStream>?> GetVodStreamsAsync(string? categoryId = null, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/player_api.php?username={_username}&password={_password}&action=get_vod_streams";
        if (!string.IsNullOrEmpty(categoryId))
        {
            url += $"&category_id={categoryId}";
        }

        var response = await _httpClient.GetStringAsync(url, cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<XtreamVodStream>>(response);
    }

    /// <summary>
    /// Gets VOD info.
    /// </summary>
    /// <param name="vodId">The VOD ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>VOD info.</returns>
    public async Task<XtreamVodInfo?> GetVodInfoAsync(string vodId, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/player_api.php?username={_username}&password={_password}&action=get_vod_info&vod_id={vodId}";
        var response = await _httpClient.GetStringAsync(url, cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<XtreamVodInfo>(response);
    }

    /// <summary>
    /// Gets the list of series categories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of series categories.</returns>
    public async Task<List<XtreamCategory>?> GetSeriesCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/player_api.php?username={_username}&password={_password}&action=get_series_categories";
        var response = await _httpClient.GetStringAsync(url, cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<XtreamCategory>>(response);
    }

    /// <summary>
    /// Gets the list of series.
    /// </summary>
    /// <param name="categoryId">Optional category ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of series.</returns>
    public async Task<List<XtreamSeries>?> GetSeriesAsync(string? categoryId = null, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/player_api.php?username={_username}&password={_password}&action=get_series";
        if (!string.IsNullOrEmpty(categoryId))
        {
            url += $"&category_id={categoryId}";
        }

        var response = await _httpClient.GetStringAsync(url, cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<XtreamSeries>>(response);
    }

    /// <summary>
    /// Gets series info.
    /// </summary>
    /// <param name="seriesId">The series ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Series info.</returns>
    public async Task<XtreamSeriesInfo?> GetSeriesInfoAsync(string seriesId, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/player_api.php?username={_username}&password={_password}&action=get_series_info&series_id={seriesId}";
        var response = await _httpClient.GetStringAsync(url, cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<XtreamSeriesInfo>(response);
    }

    /// <summary>
    /// Gets the stream URL for a live channel.
    /// </summary>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="extension">The stream extension (ts, m3u8, etc.).</param>
    /// <returns>Stream URL.</returns>
    public string GetLiveStreamUrl(string streamId, string extension = "ts")
    {
        return $"{_baseUrl}/live/{_username}/{_password}/{streamId}.{extension}";
    }

    /// <summary>
    /// Gets the stream URL for a VOD.
    /// </summary>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="extension">The stream extension.</param>
    /// <returns>Stream URL.</returns>
    public string GetVodStreamUrl(string streamId, string extension)
    {
        return $"{_baseUrl}/movie/{_username}/{_password}/{streamId}.{extension}";
    }

    /// <summary>
    /// Gets the stream URL for a series episode.
    /// </summary>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="extension">The stream extension.</param>
    /// <returns>Stream URL.</returns>
    public string GetSeriesStreamUrl(string streamId, string extension)
    {
        return $"{_baseUrl}/series/{_username}/{_password}/{streamId}.{extension}";
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _httpClient?.Dispose();
        }
    }
}
