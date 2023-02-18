using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HugeFiles.HugeFiles;
using HugeFiles.Utils;

namespace HugeFiles.Tests
{
    public class TestRunner
    {
        public static void RunAll()
        {
            Npp.notepad.FileNew();
            Npp.AddLine(@"=========================
Testing default chunker for normal text files
=========================
");
            ChunkerTester.Test();

            Npp.AddLine(@"=========================
Testing JSON Chunker
=========================
");
            JsonChunkerTester.Test();

            Npp.AddLine(@"=========================
Performance tests for normal Chunker and JSON Chunker
=========================
");
            Benchmarker.Benchmark(
                "big_random.json",
                "\r\n", 18_000, 22_000, true,
                "big_random.json",
                30,
                30
            );
            //because Visual Studio runs a whole bunch of other things in the background
            //     when I build my project, the benchmarking suite
            //     makes my code seem way slower than it actually is when it's running unhindered.
            //     * *To see how fast the code actually is, you need to run the executable outside of Visual Studio.**
        }
    }
}
