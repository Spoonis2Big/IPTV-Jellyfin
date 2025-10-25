# Catalog Editor Guide

The Catalog Editor is a powerful feature that allows you to customize the IPTV content metadata from your Xtream Codes servers.

## Overview

The Catalog Editor provides a web interface to override and customize:
- Channel names and numbers
- Program descriptions and overviews
- Images and artwork
- Genres and tags
- Ratings and production years
- Content visibility (hide unwanted content)

## Features

### Metadata Overrides

All customizations are stored locally as "metadata overrides" that supersede the information from your Xtream Codes server. This means:
- Your edits persist even when the IPTV provider updates their catalog
- Changes are specific to your Jellyfin installation
- You can export and import your customizations

### Content Types Supported

1. **Live TV Channels**
   - Custom channel names
   - Custom channel numbers for EPG sorting
   - Channel logos/images
   - Hide unwanted channels

2. **VOD (Movies)**
   - Custom titles
   - Descriptions and plot summaries
   - Poster images
   - Genres and ratings

3. **Series (TV Shows)**
   - Series titles
   - Descriptions
   - Cover art
   - Genre tags

## Using the Catalog Editor

### Accessing the Editor

1. Open Jellyfin web interface
2. Navigate to **Dashboard** → **Plugins**
3. Find **Xtream Codes** in the plugin list
4. Click on the **Catalog Editor** tab

### Browsing Content

- **Filter by Type**: Use the filter buttons to show only Live TV, VOD, or Series
- **Search**: Use the search box to find content by name or ID
- **Modified Filter**: Show only items you've customized
- **Refresh Catalog**: Reload the catalog from your servers

### Editing Metadata

1. Click **Edit** on any content item
2. Modify any fields you want to customize:
   - **Name**: The display name in Jellyfin
   - **Description/Overview**: Full text description
   - **Image URL**: Direct URL to poster/thumbnail image
   - **Channel Number**: (Live TV only) Custom channel number
   - **Genres**: Comma-separated list (e.g., "Action, Drama")
   - **Production Year**: Release year
   - **Rating**: 0-10 scale
   - **Tags**: Custom tags for filtering
   - **Hide**: Check to remove from Jellyfin catalog
3. Click **Save** to apply changes
4. Click **Reset to Original** to remove all overrides for this item

### Import/Export

#### Exporting Overrides

1. Click **Export Overrides** button
2. A JSON file will download with all your customizations
3. Save this file for backup or transfer

#### Importing Overrides

1. Click **Import Overrides** button
2. Select a previously exported JSON file
3. Confirm the import
4. Your customizations will be restored

## Use Cases

### Organizing Channels

Renumber channels to group them by category:
```
Sports Channels: 100-199
News Channels: 200-299
Entertainment: 300-399
```

### Fixing Metadata

- Correct misspelled channel names
- Add missing descriptions
- Replace low-quality or missing images
- Add proper genre tags for better filtering

### Hiding Content

- Hide adult content channels
- Remove duplicate streams
- Hide inactive or broken channels
- Filter out unwanted VOD categories

### Localization

- Translate channel names to your language
- Replace descriptions with localized text
- Add local genre classifications

## Technical Details

### Storage Location

Metadata overrides are stored in:
```
{jellyfin-data-path}/xtream-metadata/overrides.json
```

### Data Format

Overrides are stored as JSON with the following structure:
```json
{
  "ContentId": "xtream_live_MyServer_12345",
  "ContentType": 0,
  "Name": "Custom Channel Name",
  "Overview": "Custom description",
  "ImageUrl": "https://example.com/image.png",
  "ChannelNumber": "101",
  "Genres": ["Sports", "Live"],
  "ProductionYear": 2024,
  "Rating": 8.5,
  "IsHidden": false,
  "Tags": ["Favorites", "HD"],
  "ModifiedDate": "2024-01-01T12:00:00Z"
}
```

### Content Types

- `0` = Live TV
- `1` = VOD (Movies)
- `2` = Series (TV Shows)

## Best Practices

1. **Backup Regularly**: Export your overrides periodically
2. **Consistent Naming**: Use a consistent naming convention for channels
3. **Test Changes**: Verify changes in Jellyfin before doing bulk edits
4. **Document Custom Numbers**: Keep a record of your channel numbering scheme
5. **Use Tags**: Create meaningful tags for better organization

## Troubleshooting

### Changes Not Appearing

- Refresh the Jellyfin Live TV guide
- Restart the Jellyfin server
- Check that the content ID matches exactly

### Lost Customizations

- Check the `overrides.json` file exists
- Verify JSON file permissions
- Import from a backup if available

### Import Fails

- Ensure JSON file is properly formatted
- Check for special characters in file
- Verify file encoding is UTF-8

## Integration with Jellyfin

Metadata overrides integrate seamlessly with Jellyfin:
- Overrides apply during content refresh
- Hidden content is filtered out automatically
- Custom channel numbers affect EPG sorting
- Changes are reflected immediately in the UI

## Advanced Usage

### Bulk Editing

For large-scale changes:
1. Export overrides to JSON
2. Edit the JSON file with a text editor or script
3. Import the modified JSON file

### Sharing Configurations

Share your catalog customizations with others:
1. Export your overrides
2. Share the JSON file
3. Others can import to replicate your setup

### Version Control

Keep track of changes:
- Store override files in version control (git)
- Track changes over time
- Revert to previous configurations easily

## Support

For issues or questions:
- Check the main README.md
- Review Jellyfin logs for errors
- Report bugs on GitHub issues page
