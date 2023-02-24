# HugeFiles

[![License](http://img.shields.io/badge/License-Apache_2-red.svg?style=flat)](http://www.apache.org/licenses/LICENSE-2.0)
[![Continuous Integration](https://github.com/molsonkiko/HugeFiles/actions/workflows/CI_build.yml/badge.svg)](https://github.com/molsonkiko/HugeFiles/actions/workflows/CI_build.yml)

Easily view very large files, one chunk at a time.

You can choose the size of chunks to view.

Any issues, feel free to email me at mjolsonsfca@gmail.com.

## Features ##
1. You can choose to break the chunk up by delimiters ("\r\n" by default, but you can choose any delimiter)
  or just have every chunk be the same size.
    - By default, the plugin will infer the line terminator of a file, so you don't need to do anything.
2. A nice form for moving between chunks.
3. A [form for finding and replacing text](/docs/README.md#findreplace-form) in the file.
4. [JSON files](/docs/README.md#chunking-json-files) can be broken into chunks that are all syntactically valid JSON.
5. Each chunk of the file can be [written to a separate file](/docs/README.md#write-chunks-to-folder), optionally in a new folder.

[Read the docs.](/docs/README.md)

[View changes and desired features.](/CHANGELOG.md)

![HugeFiles usage example](/hugefiles%20demo%20screenshot.PNG)

## Downloads and Installation ##

Go to the [Releases page](https://github.com/molsonkiko/HugeFiles/releases) to see past releases.

[Download latest 32-bit version](https://github.com/molsonkiko/HugeFiles/raw/main/Release_x86.zip)

You can unzip the 32-bit download to `.\Program Files (x86)\Notepad++\plugins\HugeFiles\HugeFiles.dll`.

[Download latest 64-bit version](https://github.com/molsonkiko/HugeFiles/raw/main/Release_x64.zip)

You can unzip the 64-bit download to `C:\Program Files\Notepad++\plugins\HugeFiles\HugeFiles.dll`.

## System Requirements ##

Every version of the plugin works for very old versions of Notepad++ (as far back as Notepad++ 7.3.3).

Every version up to and including [0.1.1](https://github.com/molsonkiko/JsonToolsNppPlugin/blob/main/CHANGELOG.md#3721---2022-10-20) should work natively on Windows 8 or later (note: this is untested), or you must install [.NET Framework 4.0](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net40). Every version beginning with [0.2.0](https://github.com/molsonkiko/JsonToolsNppPlugin/blob/main/CHANGELOG.md#400---2022-10-24) works on [Windows 10 May 2019 update](https://blogs.windows.com/windowsexperience/2019/05/21/how-to-get-the-windows-10-may-2019-update/) or later, or you must install [.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48).

## Acknowledgments ##

* [Kasper B. Graverson](https://github.com/kbilsted) for creating the [plugin pack](https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net) that this is based on.
* [Claudio Olmi](https://github.com/superolmo) for making the very useful [BigFiles](https://github.com/superolmo/BigFiles) plugin that inspired this.
* And of course, Don Ho for creating [Notepad++](https://notepad-plus-plus.org/)!