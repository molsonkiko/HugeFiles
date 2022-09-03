using System;
using System.Collections.Generic;
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
    }
}
