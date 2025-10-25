using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.XtreamCodes.XtreamAPI;

/// <summary>
/// Xtream API authentication response.
/// </summary>
public class XtreamAuthResponse
{
    /// <summary>
    /// Gets or sets the user info.
    /// </summary>
    [JsonPropertyName("user_info")]
    public XtreamUserInfo? UserInfo { get; set; }

    /// <summary>
    /// Gets or sets the server info.
    /// </summary>
    [JsonPropertyName("server_info")]
    public XtreamServerInfo? ServerInfo { get; set; }
}

/// <summary>
/// Xtream user info.
/// </summary>
public class XtreamUserInfo
{
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the expiration date.
    /// </summary>
    [JsonPropertyName("exp_date")]
    public string? ExpDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the account is active.
    /// </summary>
    [JsonPropertyName("is_trial")]
    public string? IsTrial { get; set; }

    /// <summary>
    /// Gets or sets the active connections.
    /// </summary>
    [JsonPropertyName("active_cons")]
    public string? ActiveConnections { get; set; }

    /// <summary>
    /// Gets or sets the max connections.
    /// </summary>
    [JsonPropertyName("max_connections")]
    public string? MaxConnections { get; set; }
}

/// <summary>
/// Xtream server info.
/// </summary>
public class XtreamServerInfo
{
    /// <summary>
    /// Gets or sets the server URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the server port.
    /// </summary>
    [JsonPropertyName("port")]
    public string? Port { get; set; }

    /// <summary>
    /// Gets or sets the HTTPS port.
    /// </summary>
    [JsonPropertyName("https_port")]
    public string? HttpsPort { get; set; }

    /// <summary>
    /// Gets or sets the server protocol.
    /// </summary>
    [JsonPropertyName("server_protocol")]
    public string? ServerProtocol { get; set; }

    /// <summary>
    /// Gets or sets the timestamp now.
    /// </summary>
    [JsonPropertyName("timestamp_now")]
    public long TimestampNow { get; set; }
}

/// <summary>
/// Xtream category.
/// </summary>
public class XtreamCategory
{
    /// <summary>
    /// Gets or sets the category ID.
    /// </summary>
    [JsonPropertyName("category_id")]
    public string? CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    [JsonPropertyName("category_name")]
    public string? CategoryName { get; set; }

    /// <summary>
    /// Gets or sets the parent ID.
    /// </summary>
    [JsonPropertyName("parent_id")]
    public int ParentId { get; set; }
}

/// <summary>
/// Xtream live stream.
/// </summary>
public class XtreamLiveStream
{
    /// <summary>
    /// Gets or sets the stream ID.
    /// </summary>
    [JsonPropertyName("stream_id")]
    public long StreamId { get; set; }

    /// <summary>
    /// Gets or sets the number.
    /// </summary>
    [JsonPropertyName("num")]
    public int Num { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the stream type.
    /// </summary>
    [JsonPropertyName("stream_type")]
    public string? StreamType { get; set; }

    /// <summary>
    /// Gets or sets the stream icon.
    /// </summary>
    [JsonPropertyName("stream_icon")]
    public string? StreamIcon { get; set; }

    /// <summary>
    /// Gets or sets the EPG channel ID.
    /// </summary>
    [JsonPropertyName("epg_channel_id")]
    public string? EpgChannelId { get; set; }

    /// <summary>
    /// Gets or sets the added timestamp.
    /// </summary>
    [JsonPropertyName("added")]
    public string? Added { get; set; }

    /// <summary>
    /// Gets or sets the category ID.
    /// </summary>
    [JsonPropertyName("category_id")]
    public string? CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the custom SID.
    /// </summary>
    [JsonPropertyName("custom_sid")]
    public string? CustomSid { get; set; }

    /// <summary>
    /// Gets or sets the TV archive.
    /// </summary>
    [JsonPropertyName("tv_archive")]
    public int TvArchive { get; set; }

    /// <summary>
    /// Gets or sets the direct source.
    /// </summary>
    [JsonPropertyName("direct_source")]
    public string? DirectSource { get; set; }

    /// <summary>
    /// Gets or sets the TV archive duration.
    /// </summary>
    [JsonPropertyName("tv_archive_duration")]
    public int TvArchiveDuration { get; set; }
}

/// <summary>
/// Xtream VOD stream.
/// </summary>
public class XtreamVodStream
{
    /// <summary>
    /// Gets or sets the stream ID.
    /// </summary>
    [JsonPropertyName("stream_id")]
    public long StreamId { get; set; }

    /// <summary>
    /// Gets or sets the number.
    /// </summary>
    [JsonPropertyName("num")]
    public int Num { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the stream type.
    /// </summary>
    [JsonPropertyName("stream_type")]
    public string? StreamType { get; set; }

    /// <summary>
    /// Gets or sets the stream icon.
    /// </summary>
    [JsonPropertyName("stream_icon")]
    public string? StreamIcon { get; set; }

    /// <summary>
    /// Gets or sets the rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public string? Rating { get; set; }

    /// <summary>
    /// Gets or sets the rating 5 based.
    /// </summary>
    [JsonPropertyName("rating_5based")]
    public double Rating5Based { get; set; }

    /// <summary>
    /// Gets or sets the added timestamp.
    /// </summary>
    [JsonPropertyName("added")]
    public string? Added { get; set; }

    /// <summary>
    /// Gets or sets the category ID.
    /// </summary>
    [JsonPropertyName("category_id")]
    public string? CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the container extension.
    /// </summary>
    [JsonPropertyName("container_extension")]
    public string? ContainerExtension { get; set; }

    /// <summary>
    /// Gets or sets the custom SID.
    /// </summary>
    [JsonPropertyName("custom_sid")]
    public string? CustomSid { get; set; }

    /// <summary>
    /// Gets or sets the direct source.
    /// </summary>
    [JsonPropertyName("direct_source")]
    public string? DirectSource { get; set; }
}

/// <summary>
/// Xtream VOD info.
/// </summary>
public class XtreamVodInfo
{
    /// <summary>
    /// Gets or sets the info.
    /// </summary>
    [JsonPropertyName("info")]
    public XtreamVodInfoDetails? Info { get; set; }

    /// <summary>
    /// Gets or sets the movie data.
    /// </summary>
    [JsonPropertyName("movie_data")]
    public XtreamMovieData? MovieData { get; set; }
}

/// <summary>
/// Xtream VOD info details.
/// </summary>
public class XtreamVodInfoDetails
{
    /// <summary>
    /// Gets or sets the TMDB ID.
    /// </summary>
    [JsonPropertyName("tmdb_id")]
    public string? TmdbId { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the cover.
    /// </summary>
    [JsonPropertyName("cover")]
    public string? Cover { get; set; }

    /// <summary>
    /// Gets or sets the plot.
    /// </summary>
    [JsonPropertyName("plot")]
    public string? Plot { get; set; }

    /// <summary>
    /// Gets or sets the cast.
    /// </summary>
    [JsonPropertyName("cast")]
    public string? Cast { get; set; }

    /// <summary>
    /// Gets or sets the director.
    /// </summary>
    [JsonPropertyName("director")]
    public string? Director { get; set; }

    /// <summary>
    /// Gets or sets the genre.
    /// </summary>
    [JsonPropertyName("genre")]
    public string? Genre { get; set; }

    /// <summary>
    /// Gets or sets the release date.
    /// </summary>
    [JsonPropertyName("releasedate")]
    public string? ReleaseDate { get; set; }

    /// <summary>
    /// Gets or sets the duration.
    /// </summary>
    [JsonPropertyName("duration")]
    public string? Duration { get; set; }

    /// <summary>
    /// Gets or sets the rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public string? Rating { get; set; }
}

/// <summary>
/// Xtream movie data.
/// </summary>
public class XtreamMovieData
{
    /// <summary>
    /// Gets or sets the stream ID.
    /// </summary>
    [JsonPropertyName("stream_id")]
    public long StreamId { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the added.
    /// </summary>
    [JsonPropertyName("added")]
    public string? Added { get; set; }

    /// <summary>
    /// Gets or sets the category ID.
    /// </summary>
    [JsonPropertyName("category_id")]
    public string? CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the container extension.
    /// </summary>
    [JsonPropertyName("container_extension")]
    public string? ContainerExtension { get; set; }

    /// <summary>
    /// Gets or sets the custom SID.
    /// </summary>
    [JsonPropertyName("custom_sid")]
    public string? CustomSid { get; set; }

    /// <summary>
    /// Gets or sets the direct source.
    /// </summary>
    [JsonPropertyName("direct_source")]
    public string? DirectSource { get; set; }
}

/// <summary>
/// Xtream series.
/// </summary>
public class XtreamSeries
{
    /// <summary>
    /// Gets or sets the series ID.
    /// </summary>
    [JsonPropertyName("series_id")]
    public long SeriesId { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the cover.
    /// </summary>
    [JsonPropertyName("cover")]
    public string? Cover { get; set; }

    /// <summary>
    /// Gets or sets the plot.
    /// </summary>
    [JsonPropertyName("plot")]
    public string? Plot { get; set; }

    /// <summary>
    /// Gets or sets the cast.
    /// </summary>
    [JsonPropertyName("cast")]
    public string? Cast { get; set; }

    /// <summary>
    /// Gets or sets the director.
    /// </summary>
    [JsonPropertyName("director")]
    public string? Director { get; set; }

    /// <summary>
    /// Gets or sets the genre.
    /// </summary>
    [JsonPropertyName("genre")]
    public string? Genre { get; set; }

    /// <summary>
    /// Gets or sets the release date.
    /// </summary>
    [JsonPropertyName("releaseDate")]
    public string? ReleaseDate { get; set; }

    /// <summary>
    /// Gets or sets the last modified.
    /// </summary>
    [JsonPropertyName("last_modified")]
    public string? LastModified { get; set; }

    /// <summary>
    /// Gets or sets the rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public string? Rating { get; set; }

    /// <summary>
    /// Gets or sets the rating 5 based.
    /// </summary>
    [JsonPropertyName("rating_5based")]
    public double Rating5Based { get; set; }

    /// <summary>
    /// Gets or sets the backdrop path.
    /// </summary>
    [JsonPropertyName("backdrop_path")]
    public List<string>? BackdropPath { get; set; }

    /// <summary>
    /// Gets or sets the YouTube trailer.
    /// </summary>
    [JsonPropertyName("youtube_trailer")]
    public string? YoutubeTrailer { get; set; }

    /// <summary>
    /// Gets or sets the episode run time.
    /// </summary>
    [JsonPropertyName("episode_run_time")]
    public string? EpisodeRunTime { get; set; }

    /// <summary>
    /// Gets or sets the category ID.
    /// </summary>
    [JsonPropertyName("category_id")]
    public string? CategoryId { get; set; }
}

/// <summary>
/// Xtream series info.
/// </summary>
public class XtreamSeriesInfo
{
    /// <summary>
    /// Gets or sets the seasons.
    /// </summary>
    [JsonPropertyName("seasons")]
    public List<XtreamSeason>? Seasons { get; set; }

    /// <summary>
    /// Gets or sets the info.
    /// </summary>
    [JsonPropertyName("info")]
    public XtreamSeries? Info { get; set; }

    /// <summary>
    /// Gets or sets the episodes.
    /// </summary>
    [JsonPropertyName("episodes")]
    public Dictionary<string, List<XtreamEpisode>>? Episodes { get; set; }
}

/// <summary>
/// Xtream season.
/// </summary>
public class XtreamSeason
{
    /// <summary>
    /// Gets or sets the air date.
    /// </summary>
    [JsonPropertyName("air_date")]
    public string? AirDate { get; set; }

    /// <summary>
    /// Gets or sets the episode count.
    /// </summary>
    [JsonPropertyName("episode_count")]
    public int EpisodeCount { get; set; }

    /// <summary>
    /// Gets or sets the ID.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the overview.
    /// </summary>
    [JsonPropertyName("overview")]
    public string? Overview { get; set; }

    /// <summary>
    /// Gets or sets the season number.
    /// </summary>
    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }

    /// <summary>
    /// Gets or sets the cover.
    /// </summary>
    [JsonPropertyName("cover")]
    public string? Cover { get; set; }

    /// <summary>
    /// Gets or sets the cover big.
    /// </summary>
    [JsonPropertyName("cover_big")]
    public string? CoverBig { get; set; }
}

/// <summary>
/// Xtream episode.
/// </summary>
public class XtreamEpisode
{
    /// <summary>
    /// Gets or sets the ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the episode number.
    /// </summary>
    [JsonPropertyName("episode_num")]
    public int EpisodeNum { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the container extension.
    /// </summary>
    [JsonPropertyName("container_extension")]
    public string? ContainerExtension { get; set; }

    /// <summary>
    /// Gets or sets the info.
    /// </summary>
    [JsonPropertyName("info")]
    public XtreamEpisodeInfo? Info { get; set; }

    /// <summary>
    /// Gets or sets the custom SID.
    /// </summary>
    [JsonPropertyName("custom_sid")]
    public string? CustomSid { get; set; }

    /// <summary>
    /// Gets or sets the added.
    /// </summary>
    [JsonPropertyName("added")]
    public string? Added { get; set; }

    /// <summary>
    /// Gets or sets the season.
    /// </summary>
    [JsonPropertyName("season")]
    public int Season { get; set; }

    /// <summary>
    /// Gets or sets the direct source.
    /// </summary>
    [JsonPropertyName("direct_source")]
    public string? DirectSource { get; set; }
}

/// <summary>
/// Xtream episode info.
/// </summary>
public class XtreamEpisodeInfo
{
    /// <summary>
    /// Gets or sets the TMDB ID.
    /// </summary>
    [JsonPropertyName("tmdb_id")]
    public int TmdbId { get; set; }

    /// <summary>
    /// Gets or sets the release date.
    /// </summary>
    [JsonPropertyName("releasedate")]
    public string? ReleaseDate { get; set; }

    /// <summary>
    /// Gets or sets the plot.
    /// </summary>
    [JsonPropertyName("plot")]
    public string? Plot { get; set; }

    /// <summary>
    /// Gets or sets the duration.
    /// </summary>
    [JsonPropertyName("duration")]
    public string? Duration { get; set; }

    /// <summary>
    /// Gets or sets the duration in seconds.
    /// </summary>
    [JsonPropertyName("duration_secs")]
    public int DurationSecs { get; set; }

    /// <summary>
    /// Gets or sets the movie image.
    /// </summary>
    [JsonPropertyName("movie_image")]
    public string? MovieImage { get; set; }

    /// <summary>
    /// Gets or sets the rating.
    /// </summary>
    [JsonPropertyName("rating")]
    public string? Rating { get; set; }
}
