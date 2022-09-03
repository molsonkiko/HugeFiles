using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HugeFiles.HugeFiles
{
    /// <summary>
    /// stores a single deletion or replacement operation made on a file
    /// </summary>
    public struct Diff
    {
        public int start;
        public int length;
        public string replacement;

        public Diff(int start, int length,string replacement)
        {
            this.start = start;
            this.length = length;
            this.replacement = replacement;
        }
    }
}
