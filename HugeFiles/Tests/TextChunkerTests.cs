using System;
using System.Collections.Generic;
using System.Reflection;
using HugeFiles.HugeFiles;
using HugeFiles.Utils;
using Kbg.NppPluginNET;

namespace HugeFiles.Tests
{
    public class TextChunkerTester
    {
        public static void Test()
        {
            int ii = 0;
            int tests_failed = 0;

            var testcases = new (
                int minChunk, int maxChunk, string fname,
                string delim, bool autoInferDelimAndTolerance,
                List<Chunk> chunks)[]
            {
                (80, 120, "bad_json.json", "\r\n", false,
                // standard settings, small chunks, no auto-inference
                new List<Chunk>
                {
                    new Chunk(0, 96),
                    new Chunk(96, 192),
                    new Chunk(192, 297),
                    new Chunk(297, 401),
                    new Chunk(401, 493),
                    new Chunk(493, 589),
                    new Chunk(589, 599),
                }),
                (80, 120, "bad_json.json", "`", true,
                // standard settings, small chunks, use auto-inference
                new List<Chunk>
                {
                    new Chunk(0, 96),
                    new Chunk(96, 192),
                    new Chunk(192, 297),
                    new Chunk(297, 401),
                    new Chunk(401, 493),
                    new Chunk(493, 589),
                    new Chunk(589, 599),
                }),
                (150, 150, "bad_json.json", "\r\n", false,
                // force chunks to be exactly 150 chars long
                new List<Chunk>
                {
                    new Chunk(0, 150),
                    new Chunk(150, 300),
                    new Chunk(300, 450),
                    new Chunk(450, 599)
                }
                ),
                (150, 150, "bad_json.json", "\r\n", true,
                // force chunks to be exactly 150 chars long, even if auto-inference is on
                new List<Chunk>
                {
                    new Chunk(0, 150),
                    new Chunk(150, 300),
                    new Chunk(300, 450),
                    new Chunk(450, 599)
                }
                ),
                (160, 240, "bad_json.json", "", false,
                // force chunks to be exactly 200 chars long using empty delim
                new List<Chunk>
                {
                    new Chunk(0, 200),
                    new Chunk(200, 400),
                    new Chunk(400, 599)
                }
                ),
                (190, 210, "bad_json.json", "", true,
                // force chunks to be exactly 200 chars long with empty delim,
                // even if auto-inference is on
                new List<Chunk>
                {
                    new Chunk(0, 200),
                    new Chunk(200, 400),
                    new Chunk(400, 599)
                }
                ),
                (80, 120, "bad_json.json", "`", false,
                // standard settings, small chunks, no auto-inference,
                // delim not found in file
                new List<Chunk>
                {
                    new Chunk(0, 120),
                    new Chunk(120, 240),
                    new Chunk(240, 360),
                    new Chunk(360, 480),
                    new Chunk(480, 599),
                }),
                (8, 32, "num_array.json", "\r\n", false,
                // no auto-inference,
                // \r\n delim for file with \n newline
                new List<Chunk>
                {
                    new Chunk(0, 32),
                    new Chunk(32, 64),
                    new Chunk(64, 96),
                    new Chunk(96, 101),
                }),
                (8, 32, "num_array.json", "\r\n", true,
                // auto-inference on,
                // \r\n delim for file with \n newline
                new List<Chunk>
                {
                    new Chunk(0, 18),
                    new Chunk(18, 38),
                    new Chunk(38, 63),
                    new Chunk(63, 81),
                    new Chunk(81, 101),
                }
                ),
                (8, 32, "num_array.json", "\n", false,
                // no auto-inference,
                // correct delim for file with \n newline
                new List<Chunk>
                {
                    new Chunk(0, 18),
                    new Chunk(18, 38),
                    new Chunk(38, 63),
                    new Chunk(63, 81),
                    new Chunk(81, 101),
                }
                ),
                (8, 32, "unicode.json", "\r", false,
                // no auto-inference,
                // correct delim for file with \r newline
                new List<Chunk>
                {
                    new Chunk(0, 23),
                    new Chunk(23, 45),
                    new Chunk(45, 68),
                    new Chunk(68, 82),
                    new Chunk(82, 96),
                    new Chunk(96, 110),
                    new Chunk(110, 124),
                    new Chunk(124, 138),
                    new Chunk(138, 158),
                }
                ),
            };
            string curdir = "plugins/HugeFiles/testfiles/";
            TextChunker chunker = null;
            bool oldAutoInfer = Main.settings.autoInferBestDelimiterAndTolerance;
            foreach ((int minChunk, int maxChunk, string fname,
                string delim, bool autoInfer,
                List<Chunk> correctChunks) in testcases)
            {
                chunker?.Dispose();
                chunker = new TextChunker(curdir + fname, delim, minChunk, maxChunk);
                Main.settings.autoInferBestDelimiterAndTolerance = autoInfer;
                chunker.AddAllChunks();
                ii++;
                string delimStr = delim.Replace("\r", "\\r").Replace("\n", "\\n");
                string failureMessage = $"While chunking {fname} with minChunk={minChunk}, maxChunk={maxChunk}, delim={delimStr}, auto-infer={autoInfer}, ";
                int correctCount = correctChunks.Count;
                int chunkCount = chunker.chunks.Count;
                if (chunkCount != correctCount)
                {
                    tests_failed++;
                    Npp.AddLine(failureMessage + $"expected {correctCount} chunks but got {chunkCount}");
                    continue;
                }
                for (int jj = 0; jj < chunkCount; jj++)
                {
                    ii++;
                    Chunk gotChunk = chunker.chunks[jj];
                    Chunk correctChunk = correctChunks[jj];
                    if (gotChunk.start != correctChunk.start
                        || gotChunk.end != correctChunk.end)
                    {
                        tests_failed++;
                        Npp.AddLine(failureMessage + $"expected chunk {jj} to start at char {correctChunk.start} and end at {correctChunk.end}, but instead started at {gotChunk.start} and ended at {gotChunk.end}");
                    }
                }
            }
            Main.settings.autoInferBestDelimiterAndTolerance = oldAutoInfer;
            Npp.AddLine($"Failed {tests_failed} tests.");
            Npp.AddLine($"Passed {ii - tests_failed} tests.");
        }
    }
}
