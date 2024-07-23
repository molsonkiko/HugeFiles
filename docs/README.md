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

There are 7 settings you can change:
1. `autoInferBestDelimiterAndTolerance` __(new in 0.2.0)__
    - Default: `true`
    - If this is set to true, and the user does not manually set delimiter to empty or minChunk = maxChunk, the plugin will automatically do the following:
    - Determine what line separator the file is using, and automatically use that separator without changing the main settings.
    - Determine the length of the longest line in the first 8kb of the file, and set maxChunk - minChunk to be 32 * that max length. 
    - Sometimes you may find that setting this to `true` results in improperly cutting off long lines later in the file. If so, you should turn this setting off.
2. `delimiter`
    - Default: `\r\n`, the carriage return-linefeed that indicates a newline on Windows.
    - You can click on the `paragraph` symbol on the top menu bar in Notepad++.
    - If `\r\n` doesn't work for splitting lines, `\r` or `\n` might work.
    - If the delimiter is left blank, all the chunks will have size (minChunk + maxChunk) / 2. This will improve performance.
3. `minChunk` and `maxChunk`
    - Default: 180,000 and 220,000 characters, respectively.
    - These are the minimum and maximum lengths that a chunk can be.
    - If minChunk equals maxChunk, the delimiter doesn't matter and all the chunks will have size (minChunk + maxChunk) / 2. This can also improve performance.
5. `previewLength`
    - Default: 20 characters.
    - This is the size of the preview you get of each chunk.
    - If previewLength is 0, each chunk is labeled with the position in the document.
6.  `parseJsonAsJson`
    - Default: True
    - If true, files with the `.json` extension are automatically parsed as JSON. [See below](#chunking-json-files) for more.
    - Parsing *large* JSON files can be __quite slow__ (perhaps 0.1-0.2 seconds per megabyte) and will *temporarily* consume a lot of memory while the file is being parsed. Hopefully this upfront cost is justified because (a) the file doesn't stay in memory and (b) paging through the file is less likely to cause crazy lag.
7. `parseNonJsonAsJson`
    - Default: False
    - If true, *all files* will be parsed [as JSON](#chunking-json-files).
    - If you want to chunk a file that doesn't have the `.json` extension as JSON, you should turn this setting on. Otherwise, it should be left off.

Changing any of these settings will cause you to lose any progress you made in paging through the document.

## Chunking JSON files ##

*Introduced in version 0.3.0*

While it makes sense to break a text file like a big LOG or CSV into lines, this doesn't make much sense for JSON files, especially since they are frequently one-line documents.

This plugin can temporarily read a large JSON file into memory and parse it to find the best places to divide the file up into chunks such that each chunk is syntactically valid JSON. This process is slow, but hopefully the results are worth it.

To be chunked by this plugin, the JSON file must conform *exactly* to the [original JSON specification](https://json.org). That means no commas after the last element in an iterable, no leading decimal points, no singlequoted strings, no `NaN`, etc. The one exception (because it's actually faster to relax this requirement) is that a fraction (number, then `/`, then another number) can have any kind of number (including floating point) as the numerator or denominator.

Here's an example of a JSON file parsed using the JSON chunking functionality.

![JSON file chunked as JSON](/docs/JSON%20file%20chunked%20as%20JSON.PNG)

My [JsonTools](https://github.com/molsonkiko/JsonToolsNppPlugin) plugin is able to work with any chunk of this file and create a tree view.

![JSON chunk with JsonTools tree](/docs/JSON%20file%20chunked%20as%20JSON%20with%20treeview.PNG)

*NOTE:* prior to version [0.4.1](/CHANGELOG.md#041---2023-03-08), chunking one JSON file and then switching to another JSON file without choosing a non-JSON file in between would result in the second file being chunked incorrectly.

## Example of the impact of settings ##

A CSV file with default settings. Notice that the chunk boundary is mid-line because this file does not use `\r\n` as newlines.

![CSV file with default settings and divided line](/docs/csv%20file%20default%20settings.PNG)

Clicking on the paragraph icon in the menu bar reveals that this file has Macintosh `\r` as newline (rendered as a black __CR__ as opposed to __LF__ or __CR LF__), so we open the settings and change our delimiter to `\r`.

You may notice that Notepad++ misreports the newline as *Unix (LF)* in the bottom right corner. That's why it's important to double-check using the paragraph icon.

![See newlines and change delimiter in settings](/docs/csv%20file%20default%20settings%20newlines%20revealed.PNG)

Now when we try paging through the file, we see that the chunks are split on line boundaries.

![CSV file with CR delimiter and line-split chunks](/docs/csv%20file%20CR%20delim%20settings.PNG)

Let's look at the file one last time, this time with previews.

![CSV file with CR delimiter and 30-character preview](/docs/csv%20file%20CR%20delim%2030%20preview%20settings.PNG)

## Find/replace form ##

You can use a form to search for text in the huge file that you've chosen. This form will find matches for simple text or a regular expression. This plugin uses [.NET regular expressions](https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference), not the Boost regex engine used by Notepad++.

__WARNING:__ Because each find/replace operation is done chunk-by-chunk, this form *will not find or replace any matches that cross chunk boundaries.* Because of this limitation, __you should not use this form to search for regular expressions that can match multiple lines,__ and before doing any replacements, you should make sure that no chunk boundaries occur in unexpected locations.

__NOTE__: For all releases up to and including [0.4.0](/CHANGELOG.md#040---2023-02-24), a syntactically incorrect regular expression (e.g., unmatched parentheses, bad escape sequence) will cause the plugin, and possibly Notepad++ as a whole, to crash. [Consult this reference](https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference) to see what kinds of regular expressions can be used.

The search form caps out at 100 search results per chunk. This is in place to avoid excessive memory consumption when searching very large files. Starting in version [0.4.1](/CHANGELOG.md#041---2023-03-08), the form will show how many results were found in total, but it will still only show at most 100 results per chunk.

![Example usage of search form](/docs/search%20form%20example.PNG)

Clicking on a top-level node in the treeview (looks like `200053: 60 results`) causes Notepad++ to open up that chunk in a buffer.

This search form works whether the file was chunked as JSON or as text.

Starting in version `0.4`, the find/replace form can perform replacements. Each chunk is read in sequence and appended to a new file, which can either be retained as a separate file or used to overwrite the original file.

![Find/replace form performing a replacement](/docs/find%20replace%20form%20replace%20to%20other%20file.PNG)

## Write chunks to folder ##

The plugin can write each chunk of a file to a folder as a separate document with the same extension as the old file. The folder can be new or existing. Command is at `Plugins->HugeFiles->Chunks to folder`.


## Running the tests ##

This plugin has automated tests that can be run at will. You can also see them in `most_recent_errors.txt` in this repository.

If you try to run the tests in a *very old* version of Notepad++ (older than `8.0`, I think) that does not put each individual plugin into a separate subfolder of the `plugins` folder, the tests will cause an error because the paths won't exist to the files that are read to run the tests.