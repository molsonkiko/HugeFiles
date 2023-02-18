using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HugeFiles.Forms;
using HugeFiles.Utils;
using HugeFiles.HugeFiles;
using Kbg.NppPluginNET;

namespace NppPluginNET.Forms
{
    public partial class FindReplaceForm : Form
    {
        private Dictionary<long, int> startsToChunkIndices;

        public FindReplaceForm()
        {
            InitializeComponent();
            this.Text = $"Find text in {Main.chunker.fname}";
            startsToChunkIndices = new Dictionary<long, int>();
        }

        private Dictionary<int, string> SearchInOneChunk(long chunkStart, long chunkEnd)
        {
            FileStream fhand = Main.chunker.fhand;
            fhand.Seek(chunkStart, SeekOrigin.Begin);
            StringBuilder sb = new StringBuilder((int)(chunkEnd - chunkStart));
            var results = new Dictionary<int, string>();
            while (fhand.Position < chunkEnd)
            {
                sb.Append((char)Main.chunker.fhand.ReadByte());
            }
            string text = sb.ToString();
            int matchCount = 0;
            Regex regex;
            string pattern = PatternBox.Text;
            if (!RegexCheckBox.Checked)
                pattern = Regex.Escape(pattern);
            if (IgnoreCaseCheckBox.Checked)
                regex = new Regex(pattern, RegexOptions.IgnoreCase);
            else regex = new Regex(pattern);
            foreach (Match match in regex.Matches(text))
            {
                if (matchCount++ >= 100)
                {
                    NotAllResultsShownFlag.Visible = true;
                    break;
                }
                results[match.Index] = match.Value;
            }
            return results;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            BaseChunker chunker = Main.chunker;
            var results = new Dictionary<long, Dictionary<int, string>>();
            chunker.AddAllChunks();
            if (Main.chunkForm != null)
                Main.chunkForm.ChunkTreePopulate();
            NotAllResultsShownFlag.Visible = false;
            for (int ii = 0; ii < chunker.chunks.Count; ii++)
            {
                Chunk chunk = chunker.chunks[ii];
                startsToChunkIndices[chunk.start] = ii;
                results[chunk.start] = SearchInOneChunk(chunk.start, chunk.end);
            }
            SearchResultTree.BeginUpdate();
            UseWaitCursor = true;
            SearchResultTree.Nodes.Clear();
            TreeNode node;
            foreach (long chunkStart in results.Keys)
            {
                var chunkResults = results[chunkStart];
                if (chunkResults.Count == 0)
                    continue;
                node = SearchResultTree.Nodes.Add($"{chunkStart}: {chunkResults.Count} results");
                foreach (int resultIndex in chunkResults.Keys)
                {
                    string result = chunkResults[resultIndex];
                    node.Nodes.Add($"{resultIndex}: {result}");
                }
            }
            SearchResultTree.EndUpdate();
            UseWaitCursor = false;
        }

        private void SearchResultsTree_NodeClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode node = e.Node;
            if (!node.Text.EndsWith("results"))
                return;
            string[] parts = node.Text.Split(':');
            long chunkStart = long.Parse(parts[0]);
            int chunkIdx = startsToChunkIndices[chunkStart];
            if (Main.chunkForm != null)
            {
                if (Main.chunker.buffName != "")
                    Npp.notepad.OpenFile(Main.chunker.buffName);
                ChunkForm.OpenChunk(Main.chunker, chunkIdx);
            }
        }
    }
}
