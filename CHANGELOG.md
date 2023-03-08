# Change Log
All notable changes to this project will be documented in this file.
 
The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).
 
## [Unreleased] - yyyy-mm-dd
 
### To Be Added

1. Add the ability to edit the underlying file *by editing a chunk in the editor* (not just using the find/replace form)
    * this will require tracking Scintilla notifications and adding Diffs to the cuurrently selected Chunk whenever the chunk buffer is changed.
    * To make this really useful, we also want a different kind of Diff that tracks find/replace actions (those have a separate Scintilla notification).
    * To make the new file:
        1. Create an empty new file.
        2. For each chunk:
            1. If the chunk is unedited, append it to the new file.
            2. Otherwise, apply the diffs, then append the edited chunk to the file.
        3. Delete the original file.
        4. Rename the new file to have the same name as the old one.
2. Chunk big XML documents so that each chunk is also valid XML.
3. __Allow user to select which chunks to edit when using find/replace form (maybe checkboxes in F/R treeview?)__
4. Make it so the find/replace form's tree view can be used even if `Main.chunkForm` does not exist.

### To Be Changed

1. Make it so that changing only `previewLength` does not cause all the chunks to refresh, but only the form.

### To Be Fixed

1. The "eyeball" icon indicating the currently selected file doesn't properly track when a chunk is selected with the find/replace form.
2. Entering an invalid regex causes the find/replace form's tree view to be permanently deleted.

## [0.4.1] - 2023-03-08

### Fixed

1. Entering an invalid regex (e.g., unbalanced parentheses) in the [find/replace form](/docs/README.md#findreplace-form) no longer causes plugin crash and instead simply causes a message box to pop up and an early return. However, the find/replace form's tree view is lost as a result, and so the find/replace form will need to be closed and reopened afterwards.
2. `Next chunk` and `First chunk` plugin commands (and associated buttons on chunk form) now add a new chunk if you were already on the last chunk (for `Next chunk`) or no chunks had been added.
3. Reduced flickering when moving between find results in the find/replace form.
4. Fixed bug where chunking a JSON file and then chunking another JSON file would cause the second file to be chunked incorrectly.

#### Added

1. `First chunk`, `Last chunk`, and `Next chunk` plugin commands all scroll the selected tree node into view on the chunk form.
2. Show total match count on find/replace form, and indicate if fewer matches are shown than the total number found. Do the same for each chunk.

## [0.4.0] - 2023-02-24

### Added

1. [Find and replace text](/docs/README.md#findreplace-form) or regexes in a file. The results of the find/replace can either overwrite the original file or be written to a new file.
2. [Write each chunk to a separate file](/docs/README.md#write-chunks-to-folder) (optionally in a new folder created for this purpose).
3. Plugin command to close connection to file and close all forms.
4. Clicking on a specific result in the find/replace form now jumps to the location of the matched text rather than just opening up the correct chunk.
5. Tab navigation of the find/replace form.

## [0.3.0] - 2023-02-18

### Added

1. [Chunking of JSON files](/docs/README.md#chunking-json-files) that ensures that every chunk is syntactically valid JSON (although you may need to delete a stray char at the beginning of a chunk sometimes).
2. Test suite and performance benchmarking.
3. Settings persist between sessions.

### Fixed

1. Miscellaneous bugs with default text chunker.

## [0.2.0] - 2023-02-04

### Added

1. [Form](/docs/README.md#findreplace-form) for searching for text in chunked file. Can navigate to chunks where search results were found.
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