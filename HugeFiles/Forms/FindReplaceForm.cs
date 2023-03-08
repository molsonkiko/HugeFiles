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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

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

        private Regex PatternBoxToRegex()
        {
            string pattern = PatternBox.Text
                .Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t");
            if (!RegexCheckBox.Checked)
                pattern = Regex.Escape(pattern);
            if (IgnoreCaseCheckBox.Checked)
                return new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return new Regex(pattern, RegexOptions.Compiled);
        }

        private (int matchCount, Dictionary<int, string> results) SearchInOneChunk(Chunk chunk)
        {
            string text = chunk.Read(Main.chunker.fhand);
            Dictionary<int, string> results = new Dictionary<int, string>();
            Regex regex = PatternBoxToRegex();
            int matchCount = 0;
            MatchCollection matches = regex.Matches(text);
            foreach (Match match in matches)
            {
                if (matchCount++ >= 100)
                    break;
                results[match.Index] = match.Value;
            }
            return (matches.Count, results);
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            BaseChunker chunker = Main.chunker;
            chunker.AddAllChunks();
            if (Main.chunkForm != null)
                Main.chunkForm.ChunkTreePopulate();
            SearchResultTree.BeginUpdate();
            UseWaitCursor = true;
            SearchResultTree.Nodes.Clear();
            TreeNode node;
            int totalMatchCount = 0;
            int shownMatchCount = 0;
            for (int ii = 0; ii < chunker.chunks.Count; ii++)
            {
                Chunk chunk = chunker.chunks[ii];
                startsToChunkIndices[chunk.start] = ii;
                int chunkMatchCount;
                Dictionary<int, string> chunkResults;
                try
                {
                    (chunkMatchCount, chunkResults) = SearchInOneChunk(chunk);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"While trying to find/replace in a chunk, got the following error:\r\n{ex}",
                            "Error during find/replace in chunk",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (chunkResults.Count == 0)
                    continue;
                if (chunkResults.Count == chunkMatchCount)
                    node = SearchResultTree.Nodes.Add($"{chunk.start}: {chunkResults.Count} results");
                else
                    node = SearchResultTree.Nodes.Add($"{chunk.start}: showing {chunkResults.Count} of {chunkMatchCount} results");
                totalMatchCount += chunkMatchCount;
                shownMatchCount += chunkResults.Count;
                foreach (int resultIndex in chunkResults.Keys)
                {
                    string result = chunkResults[resultIndex];
                    node.Nodes.Add($"{resultIndex}: {result}");
                }
            }
            ResultCountLabel.Text = shownMatchCount < totalMatchCount
                ? $"Showing {shownMatchCount} of {totalMatchCount} results"
                : $"Found {totalMatchCount} results";
            SearchResultTree.EndUpdate();
            UseWaitCursor = false;
        }

        private void SearchResultsTree_NodeClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode parent = e.Node;
            int indexInChunk = 0;
            if (!parent.Text.EndsWith("results"))
            {
                // this is a node for a specific result; get the location of that result
                // so we can jump to it on click.
                string[] specificResultParts = parent.Text.Split(':');
                indexInChunk = int.Parse(specificResultParts[0]);
                parent = parent.Parent;
            }
            // now get the chunk number
            string[] parts = parent.Text.Split(':');
            long chunkStart = long.Parse(parts[0]);
            int chunkIdx = startsToChunkIndices[chunkStart];
            // if necessary, open the file
            string curbuff = Npp.notepad.GetCurrentFilePath();
            if (Main.chunker.buffName != "" && Main.chunker.buffName != curbuff)
                Npp.notepad.OpenFile(Main.chunker.buffName);
            if (Main.chunker.chunkSelected != chunkIdx)
            {
                // read and display the chunk if it's not already open
                ChunkForm.OpenChunk(Main.chunker, chunkIdx);
            }
            // jump to the target position to show the match
            Npp.editor.GotoPos(indexInChunk);
        }

        private string ReplaceInOneChunk(Chunk chunk)
        {
            string text = chunk.Read(Main.chunker.fhand);
            Regex regex = PatternBoxToRegex();
            return regex.Replace(text, ReplacementBox.Text);
        }

        /// <summary>
        /// Creates a new file, and performs find/replace in every chunk of the original file
        /// then appends the edited chunk to the new file.<br></br>
        /// If the user desires, the old file can then be removed and the new file can be renamed to match the old file's name.<br></br>
        /// The key to this algorithm is the fact that appending a chunk of length m to an existing file of length n takes O(m) time
        /// whereas inserting takes average-case O(n) time.
        /// </summary>
        private void ReplaceButton_Click(object sender, EventArgs e)
        {
            string originalName = Main.chunker.fname;
            string targetFname;
            if (OverwriteCheckBox.Checked)
            {
                targetFname = Path.GetTempFileName();
            }
            else
            {
                targetFname = TargetFnameBox.Text;
                if (targetFname.Length == 0)
                {
                    MessageBox.Show("Either the original file must be overwritten or you must enter a valid file name to write to",
                        "Invalid filename to replace to",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (File.Exists(targetFname))
                {
                    MessageBox.Show($"Can't replace to filename {targetFname}, because a file with that name already exists.",
                        "Can't replace to existing filename",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                } 
            }
            FileStream newFhand = null;
            Main.chunker.AddAllChunks();
            try
            {
                newFhand = new FileStream(targetFname, FileMode.Append, FileAccess.Write);
                int chunkCount = Main.chunker.chunks.Count;
                bool isJsonChunker = Main.chunker is JsonChunker;
                for (int ii = 0; ii < chunkCount; ii++)
                {
                    // do our find/replace in each chunk and append it to the file
                    Chunk chunk = Main.chunker.chunks[ii];
                    try
                    {
                        byte[] replaced = Encoding.UTF8.GetBytes(ReplaceInOneChunk(chunk));
                        newFhand.Write(replaced, 0, replaced.Length);
                        if (isJsonChunker && ii < chunkCount - 1)
                            newFhand.Write(Encoding.UTF8.GetBytes(","), 0, 1);
                            // the chunks don't end in commas by default, so we need to add commas after each
                            // intermediate chunk to ensure that the new file is valid json
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"While trying to find/replace in a chunk, got the following error:\r\n{ex}",
                            "Error during find/replace in chunk",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (OverwriteCheckBox.Checked)
                {
                    // disconnect from the file, delete our original file, and rename the temp file to the original name
                    try
                    {
                        Main.chunker.fhand.Dispose();
                        newFhand.Dispose();
                        File.Delete(originalName);
                        File.Move(targetFname, originalName);
                    }
                    catch
                    {
                        MessageBox.Show("Could not overwrite the original file. Make sure it's not being used by another process.",
                            "Could not overwrite original file",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    finally
                    {
                        if (Main.chunker is JsonChunker)
                        {
                            Main.chunker.Dispose();
                            Main.chunker = new JsonChunker(originalName, Main.settings.minChunk, Main.settings.maxChunk);
                        }
                        else Main.chunker.ChooseNewFile(originalName);
                        Main.chunkForm?.ChunkTreePopulate();
                    }
                }
            }
            finally
            {
                // free up references to our new/temp file
                newFhand?.Dispose();
                if (OverwriteCheckBox.Checked)
                {
                    File.Delete(targetFname);
                }
            }
            string fnameWrittenTo = OverwriteCheckBox.Checked ? originalName : targetFname;
            MessageBox.Show($"Succeeded in performing find/replace and writing results to {fnameWrittenTo}",
                    "Find/replace successful!",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// allow tabbing between controls<br></br>
        /// also allow user to perform a search by pressing enter<br></br>
        /// also if a button is selected, enter has same effect as clicking
        /// </summary>
        private void FindReplaceForm_KeyUp(object sender, KeyEventArgs e)
        {
            // enter presses button
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (sender is Button btn)
                {
                    // Enter has the same effect as clicking a selected button
                    btn.PerformClick();
                }
                if (sender is TextBox)
                {
                    SearchButton.PerformClick();
                }
            }
            // Escape -> go to editor
            else if (e.KeyData == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                Npp.editor.GrabFocus();
            }
            // Tab -> go through controls, Shift+Tab -> go through controls backward
            else if (e.KeyCode == Keys.Tab)
            {
                Control next = GetNextControl((Control)sender, !e.Shift);
                while ((next == null) || (!next.TabStop)) next = GetNextControl(next, !e.Shift);
                next.Focus();
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// make it so when user tabs out of a text box, there's no audible ding
        /// </summary>
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\t')
                e.Handled = true;
        }
    }
}
