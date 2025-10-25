using System;
using System.Collections.Generic;

namespace Jellyfin.Plugin.XtreamCodes.Metadata;

/// <summary>
/// Represents metadata overrides for IPTV content.
/// </summary>
public class MetadataOverride
{
    /// <summary>
    /// Gets or sets the content ID (channel/vod/series ID).
    /// </summary>
    public string ContentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content type.
    /// </summary>
    public ContentType ContentType { get; set; }

    /// <summary>
    /// Gets or sets the custom name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the custom overview/description.
    /// </summary>
    public string? Overview { get; set; }

    /// <summary>
    /// Gets or sets the custom image URL.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the custom channel number (for Live TV).
    /// </summary>
    public string? ChannelNumber { get; set; }

    /// <summary>
    /// Gets or sets custom genres.
    /// </summary>
    public List<string>? Genres { get; set; }

    /// <summary>
    /// Gets or sets the custom production year.
    /// </summary>
    public int? ProductionYear { get; set; }

    /// <summary>
    /// Gets or sets custom rating.
    /// </summary>
    public float? Rating { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this content is hidden.
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Gets or sets custom tags.
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Gets or sets the date this override was created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date this override was last modified.
    /// </summary>
    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Content type enum.
/// </summary>
public enum ContentType
{
    /// <summary>
    /// Live TV channel.
    /// </summary>
    LiveTV,

    /// <summary>
    /// VOD/Movie.
    /// </summary>
    VOD,

    /// <summary>
    /// Series/TV Show.
    /// </summary>
    Series
}
