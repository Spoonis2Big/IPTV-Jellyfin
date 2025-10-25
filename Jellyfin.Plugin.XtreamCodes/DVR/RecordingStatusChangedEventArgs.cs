using System;
using MediaBrowser.Model.LiveTv;

namespace Jellyfin.Plugin.XtreamCodes.DVR;

/// <summary>
/// Event args for recording status changes.
/// </summary>
public class RecordingStatusChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the recording ID.
    /// </summary>
    public string RecordingId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the recording status.
    /// </summary>
    public RecordingStatus Status { get; set; }
}
