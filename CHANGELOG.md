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
2. Make the search form into a find/replace form.
3. Add a tool for creating a new directory and writing each chunk to a separate file.
4. Chunk big XML documents so that each chunk is also valid XML.

### To Be Changed

1. Make it so that changing only `previewLength` does not cause all the chunks to refresh, but only the form.

### To Be Fixed

1. The "eyeball" icon indicating the currently selected file doesn't properly track when a chunk is selected with the find/replace form.

## [0.3.0] - 2023-02-18

### Added

1. [Chunking of JSON files](/docs/README.md#chunking-json-files) that ensures that every chunk is syntactically valid JSON (although you may need to delete a stray char at the beginning of a chunk sometimes).
2. Test suite and performance benchmarking.
3. Settings persist between sessions.

### Fixed

1. Miscellaneous bugs with default text chunker.

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