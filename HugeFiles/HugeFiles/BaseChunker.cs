using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Windows.Forms;
using Kbg.NppPluginNET;

namespace HugeFiles.HugeFiles
{
    public abstract class BaseChunker : IDisposable
    {
        public List<Chunk> chunks;
        public FileStream fhand;
        public string fname;
        /// <summary>
        /// the name of the buffer that chunks are viewed in.<br></br>
        /// This avoids the annoyance of opening up a new buffer
        /// every time the user wants to change to a different chunk.
        /// </summary>
        public string buffName;
        public int chunkSelected;
        /// <summary>
        /// whether the chunking of the file is completed
        /// </summary>
        public bool finished;

        public BaseChunker(string fname)
        {
            if (!File.Exists(fname))
            {
                throw new FileNotFoundException(fname);
            }
            this.fname = fname;
            fhand = new FileStream(fname, FileMode.Open, FileAccess.Read);
            buffName = "";
            chunks = new List<Chunk>();
            finished = false;
            chunkSelected = -1;
        }

        public abstract void AddAllChunks();

        public abstract string ReadChunk(int chunkSelected);

        public void Dispose()
        {
            fhand?.Dispose();
        }

        /// <summary>
        /// clear all chunks and set position to 0, but change nothing else
        /// </summary>
        public abstract void Reset();

        public abstract void Reset(string delimiter, int minChunk, int maxChunk);

        /// <summary>
        /// choose a new file and clear all chunks, but keep settings
        /// </summary>
        /// <param name="fname"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public void ChooseNewFile(string fname)
        {
            fhand?.Dispose();
            if (!File.Exists(fname))
            {
                throw new FileNotFoundException(fname);
            }
            this.fname = fname;
            fhand = new FileStream(fname, FileMode.Open, FileAccess.Read);
            Reset();
        }

        /// <summary>
        /// Add a short preview of a chunk.
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="previewLength"></param>
        public void AddPreview(Chunk chunk, int previewLength)
        {
            if (chunk.start + previewLength > fhand.Length)
                previewLength = (int)(fhand.Length - chunk.start);
            fhand.Seek(chunk.start, SeekOrigin.Begin);
            byte[] bytes = new byte[previewLength];
            fhand.Read(bytes, 0, previewLength);
            // replace invisible whitespace chars with their string representations
            string preview = Encoding.UTF8.GetString(bytes)
                .Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");
            chunk.preview = preview;
        }

        const int LARGE_CHUNK_NUMBER = 5000;

        /// <summary>
        /// If the projected number of chunks in the file would be very large (currently the threshold is 5000),
        /// warn the user.<br></br>
        /// If the warning pops up and the user clicks No (they don't want to continue), return true.<br></br>
        /// If the user clicks Yes or no warning is displayed, return false.
        /// </summary>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public bool WarnTooManyChunks(int chunkSize)
        {
            long estimatedNumChunks = fhand.Length / chunkSize;
            if (estimatedNumChunks > LARGE_CHUNK_NUMBER)
            {
                return MessageBox.Show(
                    $"Adding all chunks of this file will result in the creation of at least {estimatedNumChunks} chunks. "
                    + "This could be very slow and memory-intensive. Do you want to proceed?",
                    "Warning: chunk size too small!",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                ) == DialogResult.No;
            }
            return false;
        }
    }
}
