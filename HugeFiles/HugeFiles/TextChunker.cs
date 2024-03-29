﻿/*
Scans through a file looking for chunk boundaries.

The simplest chunk boundary condition is simply chunk length.
That is, you might choose length = 100,000 chars, in which case
every chunk except the last will have 100,000 chars and the last
will have len(file) % 100,000 chars.

The disadvantage of length-based chunks is that you can split up
files in the middle of lines or other meaningful semantic blocks.
We call the substring that we want to split the file by the
*delimiter*.

We may not want to insist on dividing the file by delimiters,
because then we would be unable to chunk very large one-line files.

One way to get around this is to allow the algorithm to divide on non-delimiters,
but only if that is the only way to avoid very large chunks.
We can formalize this by creating a chunker that has four parameters:

string delimiter
int minChunk
int desiredChunk
int maxChunk

We will simplify by always making desiredChunk = (minChunk + maxChunk) / 2.

The algorithm proceeds as follows:
ii = chunk start
ii += minChunk // we won't co
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Kbg.NppPluginNET;
using System.Windows.Forms;

namespace HugeFiles.HugeFiles
{
    public class TextChunker : BaseChunker
    {
        public string delimiter;
        public int minChunk;
        public int maxChunk;
        public long position;

        /// <summary>
        /// breaks a file into chunks
        /// </summary>
        public TextChunker(string fname,
                       string delimiter = "\r\n",
                       int minChunk = 180_000,
                       int maxChunk = 220_000) : base(fname)
        {
            this.delimiter = delimiter
                .Replace("\\t", "\t").Replace("\\r", "\r").Replace("\\n", "\n");
            // the user needs an easy way to specify what kind of line feed (if any)
            // they want to use as their delimiter, so they will input "\r" and the like
            // in the Settings box. This undoes that hack.
            this.minChunk = minChunk;
            this.maxChunk = maxChunk;
            position = 0;
        }

        /// <summary>
        /// find the best location for the next chunk end,<br></br>
        /// add the new chunk to the chunk list,<br></br>
        /// and move position to the end of that chunk.
        /// </summary>
        public void AddChunk()
        {
            if (finished)
                return;
            if (position == 0 &&
                Main.settings.autoInferBestDelimiterAndTolerance
                && !(delimiter.Length == 0 || maxChunk - minChunk == 0))
            {
                // try to find the best delimiter for this file
                // but don't bother if the user has made it clear that they don't
                // want to use a delimiter
                InferDelimiterAndTolerance();
            }
            long flen = fhand.Length;
            long begin = position + minChunk;
            long end = position + maxChunk;
            long desired = (end + begin) / 2;
            long closest_to_desired = end;
            long shortest_distance_to_desired = end - desired;
            int previewLength = Main.settings.previewLength;
            Chunk chunk = new Chunk(position, end);
            chunks.Add(chunk);
            if (previewLength > 0)
            {
                if (position + previewLength > flen)
                    previewLength = (int)(flen - position);
                AddPreview(chunk, previewLength);
            }
            if (desired >= flen)
            {
                // the remainder of the file is shorter than our desired chunk size, so we end with a short chunk
                chunk.end = flen;
                position = flen;
                finished = true;
                return;
            }
            // if no delimiter is specified or there's no wiggle room in chunk size,
            // we don't need to bother looking for the closest delimiter to the desired
            // length. We just always choose the desired length.
            if (minChunk == maxChunk || delimiter.Length == 0)
            {
                chunk.end = desired;
                position = desired;
                return;
            }
            fhand.Seek(begin, SeekOrigin.Begin);
            char cur_delim_char = delimiter[0];
            int position_in_delimiter = 0;
            if (end > flen)
            {
                end = flen;
                closest_to_desired = flen;
            }
            while (fhand.Position < end)
            {
                char c = (char)fhand.ReadByte();
                if (c == cur_delim_char)
                {
                    if (position_in_delimiter == delimiter.Length - 1)
                    {
                        position_in_delimiter = 0;
                        cur_delim_char = delimiter[0];
                        long distance_to_desired = Math.Abs(fhand.Position - desired);
                        if (distance_to_desired < shortest_distance_to_desired)
                        {
                            shortest_distance_to_desired = distance_to_desired;
                            closest_to_desired = fhand.Position;
                        }
                        else if (fhand.Position > desired)
                        {
                            // if we've passed the desired chunk size
                            // and we're further away than
                            // the closest-to-desired delimiter found,
                            // stop looking
                            break;
                        }
                        continue;
                    }
                    position_in_delimiter++;
                    cur_delim_char = delimiter[position_in_delimiter];
                }
            }
            position = closest_to_desired;
            chunk.end = closest_to_desired;
            if (position >= flen) finished = true;
        }

        public override void AddAllChunks()
        {
            if (WarnTooManyChunks(maxChunk)) return;
            while (!finished) AddChunk();
        }

        /// <summary>
        /// Get all text in the chunkNum^th chunk, and set chunkSelected to chunkNum
        /// </summary>
        /// <param name="chunkNum"></param>
        /// <returns></returns>
        public override string ReadChunk(int chunkNum)
        {
            chunkSelected = chunkNum;
            if (chunkNum > chunks.Count - 1)
            {
                while (chunks.Count - 1 < chunkNum)
                    AddChunk();
            }
            Chunk chunk = chunks[chunkNum];
            return chunk.Read(fhand);
        }

        public override void Reset()
        {
            chunks.Clear();
            chunkSelected = -1;
            finished = false;
            position = 0;
        }

        /// <summary>
        /// change the settings on this chunker without changing the file,<br></br>
        /// then clear all chunks and set position to 0
        /// </summary>
        /// <param name="delimiter"></param>
        /// <param name="minChunk"></param>
        /// <param name="maxChunk"></param>
        public override void Reset(string delimiter, int minChunk, int maxChunk)
        {
            this.delimiter = delimiter
                .Replace("\\t", "\t").Replace("\\r", "\r").Replace("\\n", "\n");
            this.minChunk = minChunk;
            this.maxChunk = maxChunk;
            Reset();
        }

        /// <summary>
        /// reads the first 8 kb of a file, counting lengths of lines
        /// and number of each line sep ('\r', '\n', or '\r\n').<br></br>
        /// If at least three lines were found, sets the delimiter to
        /// whichever one occurred the most times.<br></br>
        /// Also sets the minChunk and maxChunk to avg(old minChunk, old maxChunk) -/+ 16*max(line length in preview),
        /// unless minChunk and maxChunk were closer together than that.<br></br>
        /// EXAMPLE:<br></br>
        /// The first 8kb have a max line length of 400 characters,
        /// and the line sep counts were {"\r": 15", "\r\n": 40}.<br></br>
        /// "\r\n" occurred the most times, so it is the delimiter.<br></br>
        /// minChunk and maxChunk were previously 199000 and 201000.<br></br>
        /// Since 16 * 400 = 6400 and this is greater than 1/2 the separation between minChunk and maxChunk,
        /// minChunk and maxChunk are not changed.
        /// </summary>
        public void InferDelimiterAndTolerance()
        {
            fhand.Seek(0, SeekOrigin.Begin);
            int maxLineLength = 0;
            Dictionary<string, int> delimCount = new Dictionary<string, int>
            {
                { "\r\n", 0},
                { "\r", 0 },
                { "\n", 0 }
            };
            int lineLength = 0;
            int lineCount = 0;
            long checkTo = fhand.Length < 8192 ? fhand.Length : 8192;
            while (fhand.Position < checkTo)
            {
                char c = (char)fhand.ReadByte();
                if (c < 0)
                    break;
                if (c == '\r')
                {
                    c = (char)fhand.ReadByte();
                    if (c == '\n')
                    {
                        delimCount["\r\n"]++;
                    }
                    else
                    {
                        delimCount["\r"]++;
                    }
                    if (lineLength > maxLineLength)
                    {
                        maxLineLength = lineLength;
                    }
                    lineLength = -1;
                    lineCount++;
                }
                else if (c == '\n')
                {
                    delimCount["\n"]++;
                    if (lineLength > maxLineLength)
                    {
                        maxLineLength = lineLength;
                    }
                    lineLength = -1;
                    lineCount++;
                }
                lineLength++;
            }
            if (lineCount >= 3)
            {
                int avgChunk = (minChunk + maxChunk) / 2;
                if (32 * maxLineLength < maxChunk - minChunk)
                {
                    minChunk = avgChunk - 16 * maxLineLength;
                    maxChunk = avgChunk + 16 * maxLineLength;
                }
                int mostPopularDelimCount = 0;
                string mostPopularDelim = "";
                foreach (string delim in delimCount.Keys)
                {
                    int ct = delimCount[delim];
                    if (ct > mostPopularDelimCount)
                    {
                        mostPopularDelimCount = ct;
                        mostPopularDelim = delim;
                    }
                }
                delimiter = mostPopularDelim;
            }
        }
    }
}
