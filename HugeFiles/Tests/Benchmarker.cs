using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HugeFiles.HugeFiles;
using HugeFiles.Utils;
using Kbg.NppPluginNET;

namespace HugeFiles.Tests
{
    public class Benchmarker
    {
        public static void Benchmark(
            string text_fname,
            string delim, int minChunk, int maxChunk, bool autoInfer,
            string json_fname,
            int num_text_trials,
            int num_json_trials)
        {
            Stopwatch watch = new Stopwatch();
            string curdir = "plugins/Hugefiles/testfiles/";
            text_fname = curdir + text_fname;
            json_fname = curdir + json_fname;
            long[] text_times = new long[num_text_trials];
            TextChunker textChunker = null;
            bool oldAutoInfer = Main.settings.autoInferBestDelimiterAndTolerance;
            Main.settings.autoInferBestDelimiterAndTolerance = autoInfer;
            long textLength = 0;
            //************ time chunking for text files ************//
            for (int ii = 0; ii < num_text_trials; ii++)
            {
                watch.Reset();
                watch.Start();
                textChunker?.Dispose();
                try
                {
                    textChunker = new TextChunker(text_fname, delim, minChunk, maxChunk);
                    textChunker.AddAllChunks();
                }
                catch (Exception ex)
                {
                    Npp.AddLine($"Error while benchmarking text chunker:\r\n{ex}");
                    break;
                }
                watch.Stop();
                textLength = textChunker.fhand.Length;
                long t = watch.ElapsedTicks;
                text_times[ii] = t;
            }
            Main.settings.autoInferBestDelimiterAndTolerance = oldAutoInfer;
            // display text results
            (double mean, double sd) = GetMeanAndSd(text_times);
            string delimStr = delim.Replace("\r", "\\r").Replace("\n", "\\n");
            Npp.AddLine($"Chunking of text file of length {textLength / 1000} KB " +
                $"with delimiter={delimStr}, minChunk={minChunk}, maxChunk={maxChunk}, auto-infer={autoInfer} " +
                $"took {ConvertTicks(mean)} +/- {ConvertTicks(sd)} " +
                $"ms over {text_times.Length} trials");
            //********** time chunking for JSON files ***********//
            long[] json_times = new long[num_json_trials];
            JsonChunker jsonChunker = null;
            long jsonLength = 0;
            for (int ii = 0; ii < num_json_trials; ii++)
            {
                watch.Reset();
                watch.Start();
                jsonChunker?.Dispose();
                try
                {
                    jsonChunker = new JsonChunker(json_fname, minChunk, maxChunk);
                    jsonChunker.Parse();
                }
                catch (Exception ex)
                {
                    Npp.AddLine($"Error while benchmarking JSON chunker:\r\n{ex}");
                    break;
                }
                watch.Stop();
                jsonLength = jsonChunker.fhand.Length;
                long t = watch.ElapsedTicks;
                json_times[ii] = t;
            }
            // display JSON chunking results
            (mean, sd) = GetMeanAndSd(json_times);
            Npp.AddLine($"Chunking of JSON file of length {jsonLength / 1000} KB " +
                $"with minChunk={minChunk} and maxChunk={maxChunk} " +
                $"took {ConvertTicks(mean)} +/- {ConvertTicks(sd)} " +
                $"ms over {json_times.Length} trials");
        }

        public static (double mean, double sd) GetMeanAndSd(long[] times)
        {
            double mean = 0;
            foreach (long t in times) { mean += t; }
            mean /= times.Length;
            double sd = 0;
            foreach (long t in times)
            {
                double diff = t - mean;
                sd += diff * diff;
            }
            sd = Math.Sqrt(sd / times.Length);
            return (mean, sd);
        }

        public static double ConvertTicks(double ticks, string new_unit = "ms", int sigfigs = 3)
        {
            switch (new_unit)
            {
                case "ms": return Math.Round(ticks / 1e4, sigfigs);
                case "s": return Math.Round(ticks / 1e7, sigfigs);
                case "ns": return Math.Round(ticks * 100, sigfigs);
                case "mus": return Math.Round(ticks / 10, sigfigs);
                default: throw new ArgumentException("Time unit must be s, mus, ms, or ns");
            }
        }
    }
}
