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

### To Be Changed

### To Be Fixed

- Sometimes `ChunkForm.NextChunk` method causes Notepad++ to freeze (infinite loop, maybe?) and you have to quit.
- Clicking on a node in the form to navigate to a different chunk works, except it doesn't properly update the image for the newly selected node. Instead it only updates the image for the previously selected node.

## [0.1.0] - 2022-09-03

### Added

Everything!

### Bugfixes

### Changed