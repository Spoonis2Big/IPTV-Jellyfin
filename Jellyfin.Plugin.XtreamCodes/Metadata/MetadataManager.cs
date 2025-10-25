using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MediaBrowser.Common.Configuration;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.XtreamCodes.Metadata;

/// <summary>
/// Manages metadata overrides for IPTV content.
/// </summary>
public class MetadataManager
{
    private readonly ILogger<MetadataManager> _logger;
    private readonly string _metadataPath;
    private readonly ConcurrentDictionary<string, MetadataOverride> _overrides;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetadataManager"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="appPaths">Application paths.</param>
    public MetadataManager(ILogger<MetadataManager> logger, IApplicationPaths appPaths)
    {
        _logger = logger;
        _metadataPath = Path.Combine(appPaths.DataPath, "xtream-metadata");
        _overrides = new ConcurrentDictionary<string, MetadataOverride>();

        Directory.CreateDirectory(_metadataPath);
        LoadOverrides();
    }

    /// <summary>
    /// Gets a metadata override for a content ID.
    /// </summary>
    /// <param name="contentId">Content ID.</param>
    /// <returns>Metadata override or null.</returns>
    public MetadataOverride? GetOverride(string contentId)
    {
        _overrides.TryGetValue(contentId, out var result);
        return result;
    }

    /// <summary>
    /// Gets all metadata overrides.
    /// </summary>
    /// <returns>List of all overrides.</returns>
    public IEnumerable<MetadataOverride> GetAllOverrides()
    {
        return _overrides.Values.ToList();
    }

    /// <summary>
    /// Gets metadata overrides by content type.
    /// </summary>
    /// <param name="contentType">Content type.</param>
    /// <returns>List of overrides.</returns>
    public IEnumerable<MetadataOverride> GetOverridesByType(ContentType contentType)
    {
        return _overrides.Values.Where(o => o.ContentType == contentType).ToList();
    }

    /// <summary>
    /// Saves or updates a metadata override.
    /// </summary>
    /// <param name="override">Metadata override.</param>
    /// <returns>Task.</returns>
    public Task SaveOverrideAsync(MetadataOverride @override)
    {
        @override.ModifiedDate = DateTime.UtcNow;
        _overrides[@override.ContentId] = @override;
        SaveOverrides();
        _logger.LogInformation("Saved metadata override for: {ContentId}", @override.ContentId);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes a metadata override.
    /// </summary>
    /// <param name="contentId">Content ID.</param>
    /// <returns>Task.</returns>
    public Task DeleteOverrideAsync(string contentId)
    {
        if (_overrides.TryRemove(contentId, out _))
        {
            SaveOverrides();
            _logger.LogInformation("Deleted metadata override for: {ContentId}", contentId);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if content is hidden.
    /// </summary>
    /// <param name="contentId">Content ID.</param>
    /// <returns>True if hidden.</returns>
    public bool IsHidden(string contentId)
    {
        return _overrides.TryGetValue(contentId, out var @override) && @override.IsHidden;
    }

    /// <summary>
    /// Exports metadata overrides to JSON.
    /// </summary>
    /// <returns>JSON string.</returns>
    public string ExportOverrides()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        return JsonSerializer.Serialize(_overrides.Values.ToList(), options);
    }

    /// <summary>
    /// Imports metadata overrides from JSON.
    /// </summary>
    /// <param name="json">JSON string.</param>
    /// <returns>Number of overrides imported.</returns>
    public int ImportOverrides(string json)
    {
        try
        {
            var overrides = JsonSerializer.Deserialize<List<MetadataOverride>>(json);
            if (overrides == null)
            {
                return 0;
            }

            int count = 0;
            foreach (var @override in overrides)
            {
                if (!string.IsNullOrEmpty(@override.ContentId))
                {
                    _overrides[@override.ContentId] = @override;
                    count++;
                }
            }

            SaveOverrides();
            _logger.LogInformation("Imported {Count} metadata overrides", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import metadata overrides");
            return 0;
        }
    }

    /// <summary>
    /// Applies overrides to a channel name.
    /// </summary>
    /// <param name="contentId">Content ID.</param>
    /// <param name="originalName">Original name.</param>
    /// <returns>Overridden name or original.</returns>
    public string ApplyNameOverride(string contentId, string originalName)
    {
        if (_overrides.TryGetValue(contentId, out var @override) && !string.IsNullOrEmpty(@override.Name))
        {
            return @override.Name;
        }

        return originalName;
    }

    /// <summary>
    /// Applies overrides to content overview.
    /// </summary>
    /// <param name="contentId">Content ID.</param>
    /// <param name="originalOverview">Original overview.</param>
    /// <returns>Overridden overview or original.</returns>
    public string? ApplyOverviewOverride(string contentId, string? originalOverview)
    {
        if (_overrides.TryGetValue(contentId, out var @override) && !string.IsNullOrEmpty(@override.Overview))
        {
            return @override.Overview;
        }

        return originalOverview;
    }

    /// <summary>
    /// Applies overrides to image URL.
    /// </summary>
    /// <param name="contentId">Content ID.</param>
    /// <param name="originalImageUrl">Original image URL.</param>
    /// <returns>Overridden image URL or original.</returns>
    public string? ApplyImageUrlOverride(string contentId, string? originalImageUrl)
    {
        if (_overrides.TryGetValue(contentId, out var @override) && !string.IsNullOrEmpty(@override.ImageUrl))
        {
            return @override.ImageUrl;
        }

        return originalImageUrl;
    }

    private void LoadOverrides()
    {
        var metadataFile = Path.Combine(_metadataPath, "overrides.json");
        if (File.Exists(metadataFile))
        {
            try
            {
                var json = File.ReadAllText(metadataFile);
                var overrides = JsonSerializer.Deserialize<List<MetadataOverride>>(json);

                if (overrides != null)
                {
                    foreach (var @override in overrides)
                    {
                        if (!string.IsNullOrEmpty(@override.ContentId))
                        {
                            _overrides[@override.ContentId] = @override;
                        }
                    }
                }

                _logger.LogInformation("Loaded {Count} metadata overrides", _overrides.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load metadata overrides");
            }
        }
    }

    private void SaveOverrides()
    {
        var metadataFile = Path.Combine(_metadataPath, "overrides.json");
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(_overrides.Values.ToList(), options);
            File.WriteAllText(metadataFile, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save metadata overrides");
        }
    }
}
