using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Model.LiveTv;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.XtreamCodes.DVR;

/// <summary>
/// Manages DVR recordings.
/// </summary>
public class RecordingManager
{
    private readonly ILogger<RecordingManager> _logger;
    private readonly IApplicationPaths _appPaths;
    private readonly ConcurrentDictionary<string, ActiveRecording> _activeRecordings;
    private readonly ConcurrentDictionary<string, RecordingInfo> _recordings;
    private readonly ConcurrentDictionary<string, TimerInfo> _timers;
    private readonly ConcurrentDictionary<string, SeriesTimerInfo> _seriesTimers;
    private readonly string _recordingsPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordingManager"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="appPaths">Application paths.</param>
    public RecordingManager(ILogger<RecordingManager> logger, IApplicationPaths appPaths)
    {
        _logger = logger;
        _appPaths = appPaths;
        _activeRecordings = new ConcurrentDictionary<string, ActiveRecording>();
        _recordings = new ConcurrentDictionary<string, RecordingInfo>();
        _timers = new ConcurrentDictionary<string, TimerInfo>();
        _seriesTimers = new ConcurrentDictionary<string, SeriesTimerInfo>();

        _recordingsPath = Path.Combine(_appPaths.DataPath, "xtream-recordings");
        Directory.CreateDirectory(_recordingsPath);

        // Load existing recordings
        LoadRecordings();
    }

    /// <summary>
    /// Event raised when recording status changes.
    /// </summary>
    public event EventHandler<RecordingStatusChangedEventArgs>? RecordingStatusChanged;

    /// <summary>
    /// Gets the recordings path.
    /// </summary>
    public string RecordingsPath => _recordingsPath;

    /// <summary>
    /// Starts a recording.
    /// </summary>
    /// <param name="timerId">Timer ID.</param>
    /// <param name="streamUrl">Stream URL to record.</param>
    /// <param name="programInfo">Program information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Recording ID.</returns>
    public async Task<string> StartRecordingAsync(string timerId, string streamUrl, ProgramInfo programInfo, CancellationToken cancellationToken)
    {
        var recordingId = Guid.NewGuid().ToString("N");
        var fileName = GetSafeFileName(programInfo.Name ?? "recording", programInfo.StartDate);
        var filePath = Path.Combine(_recordingsPath, $"{fileName}.ts");

        var recordingInfo = new RecordingInfo
        {
            Id = recordingId,
            TimerId = timerId,
            Name = programInfo.Name,
            Overview = programInfo.Overview,
            StartDate = programInfo.StartDate,
            EndDate = programInfo.EndDate,
            ChannelId = programInfo.ChannelId,
            Status = RecordingStatus.InProgress,
            Path = filePath,
            SeriesTimerId = programInfo.SeriesTimerId
        };

        _recordings[recordingId] = recordingInfo;

        var activeRecording = new ActiveRecording
        {
            RecordingInfo = recordingInfo,
            Process = null,
            CancellationTokenSource = new CancellationTokenSource()
        };

        _activeRecordings[recordingId] = activeRecording;

        // Start the recording process
        _ = Task.Run(async () =>
        {
            try
            {
                await RecordStreamAsync(streamUrl, filePath, activeRecording.CancellationTokenSource.Token).ConfigureAwait(false);

                recordingInfo.Status = RecordingStatus.Completed;
                _logger.LogInformation("Recording completed: {Name}", recordingInfo.Name);

                RecordingStatusChanged?.Invoke(this, new RecordingStatusChangedEventArgs
                {
                    RecordingId = recordingId,
                    Status = RecordingStatus.Completed
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Recording failed: {Name}", recordingInfo.Name);
                recordingInfo.Status = RecordingStatus.Error;

                RecordingStatusChanged?.Invoke(this, new RecordingStatusChangedEventArgs
                {
                    RecordingId = recordingId,
                    Status = RecordingStatus.Error
                });
            }
            finally
            {
                _activeRecordings.TryRemove(recordingId, out _);
            }
        }, cancellationToken);

        SaveRecordings();
        return recordingId;
    }

    /// <summary>
    /// Stops a recording.
    /// </summary>
    /// <param name="recordingId">Recording ID.</param>
    /// <returns>Task.</returns>
    public Task StopRecordingAsync(string recordingId)
    {
        if (_activeRecordings.TryGetValue(recordingId, out var activeRecording))
        {
            activeRecording.CancellationTokenSource?.Cancel();
            activeRecording.Process?.Kill();

            if (_recordings.TryGetValue(recordingId, out var recordingInfo))
            {
                recordingInfo.Status = RecordingStatus.Completed;
                SaveRecordings();
            }

            _logger.LogInformation("Stopped recording: {RecordingId}", recordingId);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes a recording.
    /// </summary>
    /// <param name="recordingId">Recording ID.</param>
    /// <returns>Task.</returns>
    public Task DeleteRecordingAsync(string recordingId)
    {
        if (_recordings.TryRemove(recordingId, out var recordingInfo))
        {
            if (!string.IsNullOrEmpty(recordingInfo.Path) && File.Exists(recordingInfo.Path))
            {
                try
                {
                    File.Delete(recordingInfo.Path);
                    _logger.LogInformation("Deleted recording file: {Path}", recordingInfo.Path);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete recording file: {Path}", recordingInfo.Path);
                }
            }

            SaveRecordings();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets all recordings.
    /// </summary>
    /// <returns>List of recordings.</returns>
    public IEnumerable<RecordingInfo> GetRecordings()
    {
        return _recordings.Values.ToList();
    }

    /// <summary>
    /// Gets a recording by ID.
    /// </summary>
    /// <param name="recordingId">Recording ID.</param>
    /// <returns>Recording info or null.</returns>
    public RecordingInfo? GetRecording(string recordingId)
    {
        _recordings.TryGetValue(recordingId, out var recording);
        return recording;
    }

    /// <summary>
    /// Creates a timer.
    /// </summary>
    /// <param name="timer">Timer info.</param>
    /// <returns>Task.</returns>
    public Task CreateTimerAsync(TimerInfo timer)
    {
        timer.Id = timer.Id ?? Guid.NewGuid().ToString("N");
        _timers[timer.Id] = timer;
        SaveTimers();

        _logger.LogInformation("Created timer: {Name} at {StartDate}", timer.Name, timer.StartDate);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates a timer.
    /// </summary>
    /// <param name="timer">Timer info.</param>
    /// <returns>Task.</returns>
    public Task UpdateTimerAsync(TimerInfo timer)
    {
        if (!string.IsNullOrEmpty(timer.Id))
        {
            _timers[timer.Id] = timer;
            SaveTimers();
            _logger.LogInformation("Updated timer: {Name}", timer.Name);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Cancels a timer.
    /// </summary>
    /// <param name="timerId">Timer ID.</param>
    /// <returns>Task.</returns>
    public Task CancelTimerAsync(string timerId)
    {
        if (_timers.TryRemove(timerId, out var timer))
        {
            SaveTimers();
            _logger.LogInformation("Cancelled timer: {Name}", timer.Name);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets all timers.
    /// </summary>
    /// <returns>List of timers.</returns>
    public IEnumerable<TimerInfo> GetTimers()
    {
        return _timers.Values.ToList();
    }

    /// <summary>
    /// Creates a series timer.
    /// </summary>
    /// <param name="seriesTimer">Series timer info.</param>
    /// <returns>Task.</returns>
    public Task CreateSeriesTimerAsync(SeriesTimerInfo seriesTimer)
    {
        seriesTimer.Id = seriesTimer.Id ?? Guid.NewGuid().ToString("N");
        _seriesTimers[seriesTimer.Id] = seriesTimer;
        SaveSeriesTimers();

        _logger.LogInformation("Created series timer: {Name}", seriesTimer.Name);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates a series timer.
    /// </summary>
    /// <param name="seriesTimer">Series timer info.</param>
    /// <returns>Task.</returns>
    public Task UpdateSeriesTimerAsync(SeriesTimerInfo seriesTimer)
    {
        if (!string.IsNullOrEmpty(seriesTimer.Id))
        {
            _seriesTimers[seriesTimer.Id] = seriesTimer;
            SaveSeriesTimers();
            _logger.LogInformation("Updated series timer: {Name}", seriesTimer.Name);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Cancels a series timer.
    /// </summary>
    /// <param name="seriesTimerId">Series timer ID.</param>
    /// <returns>Task.</returns>
    public Task CancelSeriesTimerAsync(string seriesTimerId)
    {
        if (_seriesTimers.TryRemove(seriesTimerId, out var seriesTimer))
        {
            SaveSeriesTimers();
            _logger.LogInformation("Cancelled series timer: {Name}", seriesTimer.Name);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets all series timers.
    /// </summary>
    /// <returns>List of series timers.</returns>
    public IEnumerable<SeriesTimerInfo> GetSeriesTimers()
    {
        return _seriesTimers.Values.ToList();
    }

    private async Task RecordStreamAsync(string streamUrl, string outputPath, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting recording to: {Path}", outputPath);

        // Use ffmpeg to record the stream
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i \"{streamUrl}\" -c copy -y \"{outputPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processStartInfo };

        process.Start();

        // Monitor cancellation
        await Task.Run(() =>
        {
            while (!process.HasExited && !cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(1000);
            }

            if (!process.HasExited)
            {
                process.Kill();
            }
        }, cancellationToken).ConfigureAwait(false);
    }

    private static string GetSafeFileName(string name, DateTime startDate)
    {
        var safeName = string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
        var timestamp = startDate.ToString("yyyyMMdd_HHmmss");
        return $"{safeName}_{timestamp}";
    }

    private void LoadRecordings()
    {
        var metadataFile = Path.Combine(_recordingsPath, "recordings.json");
        if (File.Exists(metadataFile))
        {
            try
            {
                var json = File.ReadAllText(metadataFile);
                var recordings = System.Text.Json.JsonSerializer.Deserialize<List<RecordingInfo>>(json);

                if (recordings != null)
                {
                    foreach (var recording in recordings)
                    {
                        if (!string.IsNullOrEmpty(recording.Id))
                        {
                            _recordings[recording.Id] = recording;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load recordings metadata");
            }
        }
    }

    private void SaveRecordings()
    {
        var metadataFile = Path.Combine(_recordingsPath, "recordings.json");
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(_recordings.Values.ToList());
            File.WriteAllText(metadataFile, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save recordings metadata");
        }
    }

    private void SaveTimers()
    {
        var metadataFile = Path.Combine(_recordingsPath, "timers.json");
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(_timers.Values.ToList());
            File.WriteAllText(metadataFile, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save timers");
        }
    }

    private void SaveSeriesTimers()
    {
        var metadataFile = Path.Combine(_recordingsPath, "series-timers.json");
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(_seriesTimers.Values.ToList());
            File.WriteAllText(metadataFile, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save series timers");
        }
    }

    private class ActiveRecording
    {
        public RecordingInfo RecordingInfo { get; set; } = null!;
        public Process? Process { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; } = null!;
    }
}
