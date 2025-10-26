using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller.LiveTv;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.XtreamCodes.DVR;

/// <summary>
/// Manages DVR recordings.
/// </summary>
public class RecordingManager
{
    private readonly ILogger<RecordingManager> _logger;
    private readonly IApplicationPaths _appPaths;
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
        _timers = new ConcurrentDictionary<string, TimerInfo>();
        _seriesTimers = new ConcurrentDictionary<string, SeriesTimerInfo>();

        _recordingsPath = Path.Combine(_appPaths.DataPath, "xtream-recordings");
        Directory.CreateDirectory(_recordingsPath);

        // Load existing timers
        LoadTimers();
        LoadSeriesTimers();
    }

    /// <summary>
    /// Gets the recordings path.
    /// </summary>
    public string RecordingsPath => _recordingsPath;

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

    private void LoadTimers()
    {
        var metadataFile = Path.Combine(_recordingsPath, "timers.json");
        if (File.Exists(metadataFile))
        {
            try
            {
                var json = File.ReadAllText(metadataFile);
                var timers = System.Text.Json.JsonSerializer.Deserialize<List<TimerInfo>>(json);

                if (timers != null)
                {
                    foreach (var timer in timers)
                    {
                        if (!string.IsNullOrEmpty(timer.Id))
                        {
                            _timers[timer.Id] = timer;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load timers");
            }
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

    private void LoadSeriesTimers()
    {
        var metadataFile = Path.Combine(_recordingsPath, "series-timers.json");
        if (File.Exists(metadataFile))
        {
            try
            {
                var json = File.ReadAllText(metadataFile);
                var seriesTimers = System.Text.Json.JsonSerializer.Deserialize<List<SeriesTimerInfo>>(json);

                if (seriesTimers != null)
                {
                    foreach (var seriesTimer in seriesTimers)
                    {
                        if (!string.IsNullOrEmpty(seriesTimer.Id))
                        {
                            _seriesTimers[seriesTimer.Id] = seriesTimer;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load series timers");
            }
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
}
