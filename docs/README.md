HugeFiles Overview
====================

This documentation will walk you through a typical use case of this application.

## Opening a file ##

The first thing to do with this plugin is to open a file. You can select `Choose file` option from the drop-down menu or the keyboard shortcut (`Alt+Shift+F` unless there's a conflict).

Once you open the file you can begin paging through the file.

## Paging through the file ##

There are five ways to move through the file: forward by one chunk, back by one chunk, all the way to the last chunk of the file, and all the way to the first chunk of the file, and clicking on a chunk in the form.

The first time you begin paging, a new buffer will open containing the text in the selected chunk. Each time you move to a new chunk, the same buffer is overwritten by the chunk's text.

__NOTE:__ If you want to page all the way to the end of a *very large file* (say, multiple GB), you probably want to change the settings to ignore the delimiter as shown below. If you don't, the plugin will read a significant fraction of the file as it pages to the end.

## Using the form ##

The form (which you can open with `Alt+Shift+H` or the drop-down menu) lets you see how many chunks you've already opened, and also lets you open a new chunk by clicking. The *eye* icon indicates the chunk that is currently in view.

## Changing the settings ##

There are four settings you can change:
1. The delimiter
    - Default: `\r\n`, the carriage return-linefeed that indicates a newline on Windows.
    - You can click on the `paragraph` symbol on the top menu bar in Notepad++.
    - If `\r\n` doesn't work for splitting lines, `\r` or `\n` might work.
    - If the delimiter is left blank, all the chunks will have size (minChunk + maxChunk) / 2. This will improve performance.
2. minChunk and maxChunk
    - Default: 180,000 and 220,000 characters, respectively.
    - These are the minimum and maximum lengths that a chunk can be.
    - If minChunk equals maxChunk, the delimiter doesn't matter and all the chunks will have size (minChunk + maxChunk) / 2. This can also improve performance.
3. previewLength
    - Default: 0 characters.
    - This is the size of the preview you get of each chunk.
    - If previewLength is 0, each chunk is labeled with the position in the document.

Changing any of these settings will cause you to lose any progress you made in paging through the document.

### Example of the impact of settings ###

A CSV file with default settings. Notice that the chunk boundary is mid-line because this file does not use `\r\n` as newlines.

![CSV file with default settings and divided line](/docs/csv%20file%20default%20settings.PNG)

Clicking on the paragraph icon in the menu bar reveals that this file has Macintosh `\r` as newline (rendered as a black __CR__ as opposed to __LF__ or __CR LF__), so we open the settings and change our delimiter to `\r`.

You may notice that Notepad++ misreports the newline as *Unix (LF)* in the bottom right corner. That's why it's important to double-check using the paragraph icon.

![See newlines and change delimiter in settings](/docs/csv%20file%20default%20settings%20newlines%20revealed.PNG)

Now when we try paging through the file, we see that the chunks are split on line boundaries.

![CSV file with CR delimiter and line-split chunks](/docs/csv%20file%20CR%20delim%20settings.PNG)

Let's look at the file one last time, this time with previews.

![CSV file with CR delimiter and 30-character preview](/docs/csv%20file%20CR%20delim%2030%20preview%20settings.PNG)