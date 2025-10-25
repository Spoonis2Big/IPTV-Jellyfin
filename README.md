# Jellyfin Xtream Codes Plugin

A Jellyfin plugin that integrates with Xtream Codes API to provide IPTV services including:
- Live TV channels
- VOD (Video on Demand / Movies)
- Series (TV Shows with episodes)

## Features

- Multiple server support - configure multiple Xtream Codes servers
- Live TV integration with channel listings
- VOD library with metadata
- Series library with seasons and episodes
- **DVR (Digital Video Recorder)** functionality:
  - Schedule recordings for live TV programs
  - One-time and series recording support
  - Configurable pre/post padding for recordings
  - Recording management (view, delete)
  - Custom recording path support
- Easy configuration through Jellyfin's plugin interface
- Support for standard Xtream Codes API endpoints

## Installation

1. Build the plugin:
   ```bash
   dotnet build Jellyfin.Plugin.XtreamCodes/Jellyfin.Plugin.XtreamCodes.csproj -c Release
   ```

2. Copy the compiled DLL to your Jellyfin plugins directory:
   ```bash
   # Linux/macOS
   cp Jellyfin.Plugin.XtreamCodes/bin/Release/net8.0/Jellyfin.Plugin.XtreamCodes.dll ~/.local/share/jellyfin/plugins/XtreamCodes/

   # Windows
   copy Jellyfin.Plugin.XtreamCodes\bin\Release\net8.0\Jellyfin.Plugin.XtreamCodes.dll %AppData%\Jellyfin\plugins\XtreamCodes\
   ```

3. Restart Jellyfin

## Configuration

1. Navigate to Dashboard → Plugins → Xtream Codes
2. Configure DVR settings (optional):
   - **Enable DVR**: Toggle DVR functionality on/off
   - **Custom Recording Path**: Specify where recordings should be saved (leave empty for default)
   - **Pre-Padding**: Seconds to start recording before program starts (default: 60)
   - **Post-Padding**: Seconds to continue recording after program ends (default: 300)
3. Click "Add Server" to configure your Xtream Codes provider
4. Enter the following information:
   - **Server Name**: A friendly name for this server
   - **Server URL**: The base URL (e.g., `http://example.com:8080`)
   - **Username**: Your Xtream Codes username
   - **Password**: Your Xtream Codes password
   - **Import Options**: Select which content types to import (Live TV, VOD, Series)
5. Save the configuration
6. Restart Jellyfin to apply changes

## Requirements

- Jellyfin 10.9.0 or higher
- .NET 8.0 SDK (for building)
- Valid Xtream Codes API credentials
- **FFmpeg** (required for DVR recording functionality)

## Project Structure

```
Jellyfin.Plugin.XtreamCodes/
├── Configuration/
│   ├── PluginConfiguration.cs   # Plugin configuration models
│   └── configPage.html           # Configuration UI
├── XtreamAPI/
│   ├── XtreamApiClient.cs        # API client implementation
│   └── Models.cs                 # API response models
├── Providers/
│   ├── XtreamLiveTvService.cs    # Live TV provider with DVR support
│   ├── XtreamMovieProvider.cs    # VOD/Movie metadata provider
│   └── XtreamSeriesProvider.cs   # Series/Episode metadata provider
├── DVR/
│   ├── RecordingManager.cs       # DVR recording management
│   └── RecordingStatusChangedEventArgs.cs
├── Plugin.cs                     # Main plugin class
└── Jellyfin.Plugin.XtreamCodes.csproj
```

## API Endpoints Used

The plugin uses the standard Xtream Codes API endpoints:

- `player_api.php` - Authentication and server info
- `get_live_categories` - Live TV categories
- `get_live_streams` - Live TV channels
- `get_vod_categories` - VOD categories
- `get_vod_streams` - VOD content
- `get_vod_info` - VOD metadata
- `get_series_categories` - Series categories
- `get_series` - Series listings
- `get_series_info` - Series details with episodes

## Development

To build the plugin for development:

```bash
dotnet build
```

To build for release:

```bash
dotnet build -c Release
```

## Using DVR Features

### Recording Live TV

1. Navigate to Live TV → Guide in Jellyfin
2. Select a program you want to record
3. Click "Record" to create a one-time recording
4. Or click "Record Series" to record all episodes

### Managing Recordings

- View recordings: Live TV → Recordings
- Delete recordings: Select a recording and click Delete
- Recordings are stored as .ts files in the configured recording path

### Timer Management

- View scheduled recordings: Live TV → Scheduled
- Edit or cancel timers as needed

## Troubleshooting

If channels or content aren't appearing:

1. Check your Xtream Codes credentials are correct
2. Verify the server URL is accessible from your Jellyfin server
3. Check Jellyfin logs for any API errors
4. Ensure you have selected the appropriate import options (Live TV, VOD, Series)

If DVR recordings fail:

1. Ensure FFmpeg is installed and accessible in your system PATH
2. Check that the recording path has write permissions
3. Verify sufficient disk space for recordings
4. Check Jellyfin logs for FFmpeg errors

## License

This project is provided as-is for educational and personal use.

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.
