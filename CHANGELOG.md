# Change Log
All notable changes to this project will be documented in this file.
 
The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).
 
## [Unreleased] - yyyy-mm-dd
 
### To Be Added

1. Add the ability to edit the underlying file.
    * this will require tracking Scintilla notifications and adding Diffs to the cuurrently selected Chunk whenever the chunk buffer is changed.
    * To make this really useful, we also want a different kind of Diff that tracks find/replace actions (those have a separate Scintilla notification).
    * To make the new file:
        1. Create an empty new file.
        2. For each chunk:
            1. If the chunk is unedited, append it to the new file.
            2. Otherwise, apply the diffs, then append the edited chunk to the file.
        3. Delete the original file.
        4. Rename the new file to have the same name as the old one.
2. Add the ability to use other kinds of delimiters, like maybe somehow chunk big JSON documents so that each chunk is also valid JSON.
    * this will not be done in HugeFiles, but I plan to implement it for JSON in [JsonTools](https://github.com/molsonkiko/JsonToolsNppPlugin).

### To Be Changed

1. Make it so that changing only `previewLength` does not cause all the chunks to refresh, but only the form.

### To Be Fixed

1. The "eyeball" icon indicating the currently selected file doesn't properly track when a chunk is selected with the find/replace form.

## [0.2.0] - 2023-02-04

### Added

1. [Form](/docs/README.md#text-search-form) for searching for text in chunked file. Can navigate to chunks where search results were found.
2. Automatic inference of line terminator. Can be turned off in plugin settings.

## [0.1.1] - 2022-09-03

### Bugfixes

Added logic to `Settings.cs` to ensure that minChunk is never greater than maxChunk, and also minChunk and maxChunk must be positive.

If the user enters a minChunk greater than maxChunk in the settings, the minChunk will be set equal to maxChunk instead.

## [0.1.0] - 2022-09-03

### Added

Everything!

### Bugfixes

### Changed