using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HugeFiles.HugeFiles;
using HugeFiles.Utils;
using Kbg.NppPluginNET;

namespace HugeFiles.Tests
{
    public class JsonChunkerTester
    {
        public static void Test()
        {
            int ii = 0;
            int tests_failed = 0;

            var testcases = new (
                int minChunk, int maxChunk, string fname,
                List<Chunk> chunks)[]
            {
                (80, 120, "bad_json.json",
                // nested object with pretty much everything, including some unicode
                new List<Chunk>
                {
                    new Chunk(0, 450),
                    new Chunk(453, 596),
                    new Chunk(596, 599),
                }),
                (8, 32, "num_array.json",
                // array of ints and floats
                new List<Chunk>
                {
                    new Chunk(0, 25),
                    new Chunk(31, 61),
                    new Chunk(67, 89),
                    new Chunk(95, 101)
                }),
                (18, 38, "unicode.json",
                // array of unicode strings
                new List<Chunk>
                {
                    new Chunk(0, 43),
                    new Chunk(49, 94),
                    new Chunk(100, 156),
                    new Chunk(156, 158),
                }),
                (8, 32, "simple_object.json",
                // object with non-iterable values, no unicode
                new List<Chunk>
                {
                    new Chunk(0, 38),
                    new Chunk(45, 83),
                    new Chunk(90, 110),
                    new Chunk(117, 128)
                }),
                (70, 110, "baseball2.json",
                // array of objects mapping to arrays
                new List<Chunk>
                {
                    new Chunk(0, 200),
                    new Chunk(204, 402),
                    new Chunk(402, 407)
                }),
                (8, 32, "array_of_arrays.json",
                // array of arrays of scalars
                new List<Chunk>
                {
                    new Chunk(0, 54),
                    new Chunk(61, 111),
                    new Chunk(118, 170),
                    new Chunk(170, 173),
                }),
                (6, 20, "num_array compressed.json",
                // array of numbers with no whitespace
                new List<Chunk>
                {
                    new Chunk(0, 14),
                    new Chunk(15, 34),
                    new Chunk(35, 50),
                    new Chunk(51, 61),
                }),
                (8, 32, "bad_json compressed.json",
                // bad_json.json, but with no whitespace
                new List<Chunk>
                {
                    new Chunk(0, 29),
                    new Chunk(30, 145),
                    new Chunk(146, 226),
                    new Chunk(226, 227)
                }),
            };
            string curdir = "plugins/HugeFiles/testfiles/";
            JsonChunker chunker = null;
            foreach ((int minChunk, int maxChunk, string fname,
                List<Chunk> correctChunks) in testcases)
            {
                if (chunker != null)
                {
                    chunker.Dispose();
                }
                chunker = new JsonChunker(curdir + fname, minChunk, maxChunk);
                string failureMessage = $"While JsonChunking {fname} with minChunk={minChunk}, maxChunk={maxChunk}, ";
                try
                {
                    chunker.Parse();
                }
                catch (Exception ex)
                {
                    ii++;
                    tests_failed++;
                    Npp.AddLine(failureMessage + $"got error:\r\n{ex}");
                    continue;
                }
                ii++;
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

            Npp.AddLine($"Failed {tests_failed} tests.");
            Npp.AddLine($"Passed {ii - tests_failed} tests.");
        }
    }
}
