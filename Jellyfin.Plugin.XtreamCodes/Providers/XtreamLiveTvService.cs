using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.XtreamCodes.Configuration;
using Jellyfin.Plugin.XtreamCodes.DVR;
using Jellyfin.Plugin.XtreamCodes.Metadata;
using Jellyfin.Plugin.XtreamCodes.XtreamAPI;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.MediaInfo;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.XtreamCodes.Providers;

/// <summary>
/// Live TV service for Xtream Codes.
/// </summary>
public class XtreamLiveTvService : ILiveTvService
{
    private readonly ILogger<XtreamLiveTvService> _logger;
    private readonly RecordingManager _recordingManager;
    private readonly MetadataManager _metadataManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="XtreamLiveTvService"/> class.
    /// </summary>
    /// <param name="logger">Instance of the <see cref="ILogger{XtreamLiveTvService}"/> interface.</param>
    /// <param name="appPaths">Application paths.</param>
    public XtreamLiveTvService(ILogger<XtreamLiveTvService> logger, IApplicationPaths appPaths)
    {
        _logger = logger;
        _recordingManager = new RecordingManager(
            new Microsoft.Extensions.Logging.Abstractions.NullLogger<RecordingManager>(),
            appPaths);
        _recordingManager.RecordingStatusChanged += OnRecordingStatusChanged;

        _metadataManager = new MetadataManager(
            new Microsoft.Extensions.Logging.Abstractions.NullLogger<MetadataManager>(),
            appPaths);
    }

    private void OnRecordingStatusChanged(object? sender, DVR.RecordingStatusChangedEventArgs e)
    {
        RecordingStatusChanged?.Invoke(this, new MediaBrowser.Model.LiveTv.RecordingStatusChangedEventArgs
        {
            RecordingId = e.RecordingId,
            Status = e.Status
        });
    }

    /// <inheritdoc />
    public string Name => "Xtream Codes";

    /// <inheritdoc />
    public string HomePageUrl => "https://github.com/yourusername/jellyfin-plugin-xtreamcodes";

    /// <inheritdoc />
    public async Task<IEnumerable<ChannelInfo>> GetChannelsAsync(CancellationToken cancellationToken)
    {
        var channels = new List<ChannelInfo>();
        var config = Plugin.Instance?.Configuration;

        if (config?.Servers == null)
        {
            return channels;
        }

        foreach (var server in config.Servers.Where(s => s.ImportLiveTv))
        {
            try
            {
                using var client = new XtreamApiClient(server.Url, server.Username, server.Password);
                var streams = await client.GetLiveStreamsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

                if (streams == null)
                {
                    continue;
                }

                foreach (var stream in streams)
                {
                    var channelId = $"xtream_live_{server.Name}_{stream.StreamId}";

                    // Skip if hidden by metadata override
                    if (_metadataManager.IsHidden(channelId))
                    {
                        continue;
                    }

                    var channelInfo = new ChannelInfo
                    {
                        Id = channelId,
                        Name = _metadataManager.ApplyNameOverride(channelId, stream.Name ?? "Unknown Channel"),
                        Number = stream.Num.ToString(),
                        ImageUrl = _metadataManager.ApplyImageUrlOverride(channelId, stream.StreamIcon),
                        ChannelType = ChannelType.TV
                    };

                    // Apply custom channel number if set
                    var metadataOverride = _metadataManager.GetOverride(channelId);
                    if (metadataOverride != null && !string.IsNullOrEmpty(metadataOverride.ChannelNumber))
                    {
                        channelInfo.Number = metadataOverride.ChannelNumber;
                    }

                    channels.Add(channelInfo);
                }

                _logger.LogInformation("Loaded {Count} channels from {Server}", streams.Count, server.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading channels from {Server}", server.Name);
            }
        }

        return channels;
    }

    /// <inheritdoc />
    public Task<List<MediaSourceInfo>> GetChannelStreamMediaSources(string channelId, CancellationToken cancellationToken)
    {
        var config = Plugin.Instance?.Configuration;
        var mediaSources = new List<MediaSourceInfo>();

        if (config?.Servers == null || !channelId.StartsWith("xtream_live_", StringComparison.Ordinal))
        {
            return Task.FromResult(mediaSources);
        }

        // Parse channel ID: xtream_live_{serverName}_{streamId}
        var parts = channelId.Split('_');
        if (parts.Length < 4)
        {
            return Task.FromResult(mediaSources);
        }

        var streamId = parts[^1];
        var serverName = string.Join("_", parts.Skip(2).Take(parts.Length - 3));

        var server = config.Servers.FirstOrDefault(s => s.Name == serverName);
        if (server == null)
        {
            return Task.FromResult(mediaSources);
        }

        using var client = new XtreamApiClient(server.Url, server.Username, server.Password);
        var streamUrl = client.GetLiveStreamUrl(streamId, "m3u8");

        var mediaSource = new MediaSourceInfo
        {
            Id = channelId,
            Path = streamUrl,
            Protocol = MediaBrowser.Model.MediaInfo.MediaProtocol.Http,
            IsInfiniteStream = true,
            IsRemote = true,
            SupportsDirectPlay = true,
            SupportsDirectStream = true,
            SupportsTranscoding = true,
            Container = "hls"
        };

        mediaSources.Add(mediaSource);
        return Task.FromResult(mediaSources);
    }

    /// <inheritdoc />
    public Task<IEnumerable<ProgramInfo>> GetProgramsAsync(string channelId, DateTime startDateUtc, DateTime endDateUtc, CancellationToken cancellationToken)
    {
        // EPG support can be implemented later
        return Task.FromResult(Enumerable.Empty<ProgramInfo>());
    }

    /// <inheritdoc />
    public Task<MediaSourceInfo> GetChannelStream(string channelId, string streamId, CancellationToken cancellationToken)
    {
        var sources = GetChannelStreamMediaSources(channelId, cancellationToken).Result;
        return Task.FromResult(sources.FirstOrDefault() ?? new MediaSourceInfo());
    }

    /// <inheritdoc />
    public Task<List<MediaSourceInfo>> GetRecordingStreamMediaSources(string recordingId, CancellationToken cancellationToken)
    {
        var mediaSources = new List<MediaSourceInfo>();
        var recording = _recordingManager.GetRecording(recordingId);

        if (recording != null && !string.IsNullOrEmpty(recording.Path) && File.Exists(recording.Path))
        {
            var mediaSource = new MediaSourceInfo
            {
                Id = recordingId,
                Path = recording.Path,
                Protocol = MediaProtocol.File,
                IsInfiniteStream = false,
                IsRemote = false,
                SupportsDirectPlay = true,
                SupportsDirectStream = true,
                SupportsTranscoding = true
            };

            mediaSources.Add(mediaSource);
        }

        return Task.FromResult(mediaSources);
    }

    /// <inheritdoc />
    public Task<MediaSourceInfo> GetRecordingStream(string recordingId, string streamId, CancellationToken cancellationToken)
    {
        var sources = GetRecordingStreamMediaSources(recordingId, cancellationToken).Result;
        return Task.FromResult(sources.FirstOrDefault() ?? new MediaSourceInfo());
    }

    /// <inheritdoc />
    public Task CloseLiveStream(string id, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task RecordLiveStream(string id, CancellationToken cancellationToken)
    {
        // Get the timer info
        var timers = _recordingManager.GetTimers();
        var timer = timers.FirstOrDefault(t => t.Id == id);

        if (timer == null)
        {
            _logger.LogWarning("Timer not found for recording: {Id}", id);
            return;
        }

        // Get stream URL
        var streamSources = await GetChannelStreamMediaSources(timer.ChannelId, cancellationToken).ConfigureAwait(false);
        var streamUrl = streamSources.FirstOrDefault()?.Path;

        if (string.IsNullOrEmpty(streamUrl))
        {
            _logger.LogError("Stream URL not found for channel: {ChannelId}", timer.ChannelId);
            return;
        }

        // Create program info from timer
        var programInfo = new ProgramInfo
        {
            Id = timer.ProgramId,
            Name = timer.Name,
            Overview = timer.Overview,
            StartDate = timer.StartDate,
            EndDate = timer.EndDate,
            ChannelId = timer.ChannelId,
            SeriesTimerId = timer.SeriesTimerId
        };

        await _recordingManager.StartRecordingAsync(id, streamUrl, programInfo, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Started recording for timer: {Name}", timer.Name);
    }

    /// <inheritdoc />
    public Task ResetTuner(string id, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public event EventHandler<RecordingStatusChangedEventArgs>? RecordingStatusChanged;

    /// <inheritdoc />
    public Task<SeriesTimerInfo> GetNewTimerDefaultsAsync(CancellationToken cancellationToken, ProgramInfo? program = null)
    {
        var defaults = new SeriesTimerInfo
        {
            RecordAnyChannel = false,
            RecordAnyTime = false,
            RecordNewOnly = true,
            KeepUpTo = 10,
            SkipEpisodesInLibrary = true
        };

        if (program != null)
        {
            defaults.Name = program.Name;
            defaults.ChannelId = program.ChannelId;
            defaults.ProgramId = program.Id;
            defaults.StartDate = program.StartDate;
            defaults.EndDate = program.EndDate;
        }

        return Task.FromResult(defaults);
    }

    /// <inheritdoc />
    public Task<IEnumerable<SeriesTimerInfo>> GetSeriesTimersAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_recordingManager.GetSeriesTimers());
    }

    /// <inheritdoc />
    public Task<IEnumerable<TimerInfo>> GetTimersAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_recordingManager.GetTimers());
    }

    /// <inheritdoc />
    public Task CreateSeriesTimerAsync(SeriesTimerInfo info, CancellationToken cancellationToken)
    {
        return _recordingManager.CreateSeriesTimerAsync(info);
    }

    /// <inheritdoc />
    public Task CreateTimerAsync(TimerInfo info, CancellationToken cancellationToken)
    {
        return _recordingManager.CreateTimerAsync(info);
    }

    /// <inheritdoc />
    public Task UpdateSeriesTimerAsync(SeriesTimerInfo info, CancellationToken cancellationToken)
    {
        return _recordingManager.UpdateSeriesTimerAsync(info);
    }

    /// <inheritdoc />
    public Task UpdateTimerAsync(TimerInfo info, CancellationToken cancellationToken)
    {
        return _recordingManager.UpdateTimerAsync(info);
    }

    /// <inheritdoc />
    public Task CancelSeriesTimerAsync(string timerId, CancellationToken cancellationToken)
    {
        return _recordingManager.CancelSeriesTimerAsync(timerId);
    }

    /// <inheritdoc />
    public Task CancelTimerAsync(string timerId, CancellationToken cancellationToken)
    {
        return _recordingManager.CancelTimerAsync(timerId);
    }

    /// <inheritdoc />
    public Task DeleteRecordingAsync(string recordingId, CancellationToken cancellationToken)
    {
        return _recordingManager.DeleteRecordingAsync(recordingId);
    }

    /// <inheritdoc />
    public Task<IEnumerable<RecordingInfo>> GetRecordingsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_recordingManager.GetRecordings());
    }

    /// <inheritdoc />
    public Task<ImageStream?> GetChannelImageAsync(string channelId, CancellationToken cancellationToken)
    {
        return Task.FromResult<ImageStream?>(null);
    }

    /// <inheritdoc />
    public Task<ImageStream?> GetRecordingImageAsync(string recordingId, CancellationToken cancellationToken)
    {
        return Task.FromResult<ImageStream?>(null);
    }

    /// <inheritdoc />
    public Task<ImageStream?> GetProgramImageAsync(string programId, string channelId, CancellationToken cancellationToken)
    {
        return Task.FromResult<ImageStream?>(null);
    }
}
