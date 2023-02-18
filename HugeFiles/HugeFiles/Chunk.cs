using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HugeFiles.HugeFiles
{
    public class Chunk
    {
        public long start;
        public long end;
        public List<Diff> diffs;
        public string preview;

        public Chunk(long start, long end)
        {
            this.start = start;
            this.end = end;
            diffs = new List<Diff>();
            preview = string.Empty;
        }

        public string ApplyDiffs(string inp)
        {
            foreach (Diff diff in diffs)
            {
                StringBuilder sb = new StringBuilder();
                int lengthDecrease = diff.replacement.Length - diff.length;
                for (int ii = 0; ii < diff.start; ii++)
                {
                    sb.Append(inp[ii]);
                }
                for (int ii = 0; ii < diff.replacement.Length; ii++)
                {
                    sb.Append(diff.replacement[ii]);
                }
                for (int ii = diff.length + diff.start; ii < inp.Length; ii++)
                {
                    sb.Append(inp[ii - lengthDecrease]);
                }
                inp = sb.ToString();
            }
            return inp;
        }

        public string Read(FileStream fhand)
        {
            int bytelen = (int)(end - start);
            byte[] bytes = new byte[bytelen];
            fhand.Seek(start, SeekOrigin.Begin);
            fhand.Read(bytes, 0, bytelen);
            string unedited = Encoding.UTF8.GetString(bytes);
            if (diffs.Count > 0)
                return ApplyDiffs(unedited);
            return unedited;
        }
    }
}
