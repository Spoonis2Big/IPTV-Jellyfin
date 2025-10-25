using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.XtreamCodes.Configuration;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        Servers = new XtreamServer[0];
        EnableDvr = true;
        RecordingPath = string.Empty;
        PostPaddingSeconds = 300; // 5 minutes
        PrePaddingSeconds = 60; // 1 minute
    }

    /// <summary>
    /// Gets or sets the Xtream Codes servers.
    /// </summary>
    public XtreamServer[] Servers { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether DVR is enabled.
    /// </summary>
    public bool EnableDvr { get; set; }

    /// <summary>
    /// Gets or sets the custom recording path. If empty, uses default path.
    /// </summary>
    public string RecordingPath { get; set; }

    /// <summary>
    /// Gets or sets the post-padding in seconds (time to record after program ends).
    /// </summary>
    public int PostPaddingSeconds { get; set; }

    /// <summary>
    /// Gets or sets the pre-padding in seconds (time to record before program starts).
    /// </summary>
    public int PrePaddingSeconds { get; set; }
}

/// <summary>
/// Represents an Xtream Codes server configuration.
/// </summary>
public class XtreamServer
{
    /// <summary>
    /// Gets or sets the server name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the server URL.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether to import live TV.
    /// </summary>
    public bool ImportLiveTv { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to import VOD.
    /// </summary>
    public bool ImportVod { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to import series.
    /// </summary>
    public bool ImportSeries { get; set; } = true;
}
