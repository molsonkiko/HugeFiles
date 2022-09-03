/*
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

namespace HugeFiles.HugeFiles
{
    public class Chunker : IDisposable
    {
        public string delimiter;
        public int minChunk;
        public int maxChunk;
        public int previewLength;
        public long position;
        public List<Chunk> chunks;
        public string fname;
        public FileStream fhand;
        /// <summary>
        /// indicates if the chunker has read every chunk in the file
        /// </summary>
        public bool finished;
        public int chunkSelected;
        /// <summary>
        /// the name of the buffer that chunks are viewed in.<br></br>
        /// This avoids the annoyance of opening up a new buffer
        /// every time the user wants to change to a different chunk.
        /// </summary>
        public string buffName;

        /// <summary>
        /// breaks a file into chunks
        /// </summary>
        public Chunker(string fname,
                       string delimiter = "\r\n",
                       int minChunk = 150_000,
                       int maxChunk = 250_000,
                       int previewLength = 0)
        {
            if (!File.Exists(fname))
            {
                throw new FileNotFoundException(fname);
            }
            this.fname = fname;
            fhand = new FileStream(fname, FileMode.Open, FileAccess.Read);
            this.delimiter = delimiter
                .Replace("\\t", "\t").Replace("\\r", "\r").Replace("\\n", "\n");
                // the user needs an easy way to specify what kind of line feed (if any)
                // they want to use as their delimiter, so they will input "\r" and the like
                // in the Settings box. This undoes that hack.
            this.minChunk = minChunk;
            this.maxChunk = maxChunk;
            this.previewLength = previewLength;
            position = 0;
            chunks = new List<Chunk>();
            finished = false;
            chunkSelected = -1;
            buffName = "";
        }

        public void Dispose()
        {
            // make sure to close the file when you're done chunking it
            fhand.Dispose();
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
            char cur_delim_char = delimiter[0];
            long flen = fhand.Length;
            int position_in_delimiter = 0;
            long begin = position + minChunk;
            long end = position + maxChunk;
            long desired = (end + begin) / 2;
            long closest_to_desired = end;
            long shortest_distance_to_desired = end - desired;
            Chunk chunk = new Chunk(position, end);
            chunks.Add(chunk);
            if (previewLength > 0)
            {
                // make a short preview of the current chunk
                long preview_end = position + previewLength > end ? end : position + previewLength;
                fhand.Seek(position, SeekOrigin.Begin);
                StringBuilder sb = new StringBuilder();
                for (long ii = position; ii < preview_end; ii++)
                    sb.Append((char)fhand.ReadByte());
                chunk.preview = sb.ToString();
                
            }
            if (desired >= flen)
            {
                // the remainder of the file is shorter than our desired chunk size, so we end with a short chunk
                chunk.end = flen;
                position = flen;
                finished = true;
                return;
            }
            fhand.Seek(begin, SeekOrigin.Begin);
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
                            // if we've passed the desired chunk size and we're further away than
                            // the closest-to-desired delimiter found, return the closest position
                            chunk.end = closest_to_desired;
                            position = closest_to_desired;
                            return;
                        }
                        continue;
                    }
                    position_in_delimiter++;
                    cur_delim_char = delimiter[position_in_delimiter];
                }
            }
            // no delimiters found between minChunk and maxChunk, so this chunk will be big
            position = end;
        }

        public void AddAllChunks()
        {
            while (!finished) AddChunk();
        }

        /// <summary>
        /// Get all text in the chunkNum^th chunk, and set chunkSelected to chunkNum
        /// </summary>
        /// <param name="chunkNum"></param>
        /// <returns></returns>
        public string ReadChunk(int chunkNum)
        {
            chunkSelected = chunkNum;
            if (chunkNum > chunks.Count - 1)
            {
                while (chunks.Count - 1 < chunkNum)
                    AddChunk();
            }
            Chunk chunk = chunks[chunkNum];
            fhand.Seek(chunk.start, SeekOrigin.Begin);
            StringBuilder sb = new StringBuilder();
            while (fhand.Position < chunk.end)
                sb.Append((char)fhand.ReadByte());
            string unedited = sb.ToString();
            if (chunk.diffs.Count > 0)
               return chunk.ApplyDiffs(unedited);
            return unedited;
        }

        /// <summary>
        /// change the settings on this chunker without changing the file,<br></br>
        /// then clear all chunks and set position to 0
        /// </summary>
        /// <param name="delimiter"></param>
        /// <param name="minChunk"></param>
        /// <param name="maxChunk"></param>
        public void Reset(string delimiter, int minChunk, int maxChunk, int previewLength)
        {
            this.delimiter = delimiter;
            this.minChunk = minChunk;
            this.maxChunk = maxChunk;
            this.previewLength = previewLength;
            chunks.Clear();
            position = 0;
            chunkSelected = -1;
        }

        /// <summary>
        /// clear all chunks and set position to 0, but change nothing else
        /// </summary>
        public void Reset()
        {
            chunks.Clear();
            position = 0;
            chunkSelected = -1;
        }

        public void ChooseNewFile(string fname)
        {
            fhand.Dispose();
            if (!File.Exists(fname))
            {
                throw new FileNotFoundException(fname);
            }
            this.fname = fname;
            fhand = new FileStream(fname, FileMode.Open, FileAccess.Read);
        }
    }
}
