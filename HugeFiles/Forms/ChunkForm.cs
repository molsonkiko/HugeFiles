using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HugeFiles.HugeFiles;
using HugeFiles.Utils;
using Kbg.NppPluginNET;

namespace HugeFiles.Forms
{
    public partial class ChunkForm : Form
    {
        public ChunkForm()
        {
            InitializeComponent();
        }

        public void ChunkTreePopulate()
        {
            if (ChunkTree.ImageList == null)
                ChunkTree.ImageList = ChunkIconList;
            var chunker = Main.chunker;
            ChunkTree.BeginUpdate();
            ChunkTree.Nodes.Clear();
            TreeNode root = new TreeNode(chunker.fname);
            root.ImageIndex = 4;
            for (int ii = 0; ii < chunker.chunks.Count; ii++)
            {
                Chunk chunk = chunker.chunks[ii];
                TreeNode node = new TreeNode();
                if (Main.settings.previewLength > 0)
                    node.Text = $"{chunk.start}: {chunk.preview}...";
                else
                    node.Text = $"{chunk.start}";
                /* images are as follows:
                saved
                unsaved
                viewing saved
                viewing unsaved
                root icon
                */
                int img_idx;
                if (ii == chunker.chunkSelected)
                {
                    img_idx = chunk.diffs.Count > 0 ? 3 : 2;
                    node.ImageIndex = node.SelectedImageIndex = img_idx;
                }
                else
                {
                    img_idx = chunk.diffs.Count > 0 ? 1 : 0;
                    node.ImageIndex = node.SelectedImageIndex = img_idx;
                }
                root.Nodes.Add(node);
            }
            ChunkTree.Nodes.Add(root);
            root.Expand();
            ChunkTree.EndUpdate();
        }

        /// <summary>
        /// change the images of the previously selected node
        /// and the current selected node<br></br>
        /// to reflect the change in selection
        /// </summary>
        /// <param name="start"></param>
        public void MoveSelectedImage(int start)
        {
            TreeNode root = ChunkTree.Nodes[0];
            if (start == Main.chunker.chunkSelected || start < 0 || start >= root.Nodes.Count)
                return;
            // first two images are for non-viewed, last two are for viewed
            TreeNode startnode = root.Nodes[start];
            TreeNode endnode = root.Nodes[Main.chunker.chunkSelected];
            startnode.ImageIndex -= 2;
            startnode.SelectedImageIndex -= 2;
            endnode.ImageIndex += 2;
            endnode.SelectedImageIndex += 2;
            endnode.EnsureVisible(); // scroll it into view if it's not visible
        }

        public static void OpenChunk(BaseChunker chunker, int chunkSelected)
        {
            if (chunker.buffName == "") // no file yet, so make one and open it
            {
                Npp.notepad.FileNew();
                chunker.buffName = Npp.GetCurrentPath();
            }
            // now check current file again to see if the form's file is open
            if (chunker.buffName != Npp.GetCurrentPath())
                return;
            Npp.editor.SetText(chunker.ReadChunk(chunkSelected));
        }

        public static void FirstChunk(BaseChunker chunker, ChunkForm chunkForm)
        {
            int prev_selected = chunker.chunkSelected;
            int prev_chunk_count = chunker.chunks.Count;
            if (prev_chunk_count == 0)
            {
                if (chunker is TextChunker tchunker)
                    tchunker.AddChunk();
                else
                    chunker.AddAllChunks();
            }
            if (prev_selected == 0)
                return;
            ChunkForm.OpenChunk(chunker, 0);
            if (chunkForm != null)
            {
                if (chunker.chunks.Count > 1 && prev_chunk_count != 0)
                    chunkForm.MoveSelectedImage(prev_selected);
                else if (prev_selected == -1)
                    chunkForm.ChunkTreePopulate();
            }
                
        }

        public static void PreviousChunk(BaseChunker chunker, ChunkForm chunkForm)
        {
            int prev_selected = chunker.chunkSelected;
            if (prev_selected <= 0)
                return;
            chunker.chunkSelected -= 1;
            ChunkForm.OpenChunk(chunker, chunker.chunkSelected);
            chunkForm?.MoveSelectedImage(prev_selected);
        }

        public static void NextChunk(BaseChunker chunker, ChunkForm chunkForm)
        {
            int prev_selected = chunker.chunkSelected;
            // if the user is on the last chunk, reveal another chunk
            int prev_chunk_count = chunker.chunks.Count;
            if (prev_selected == chunker.chunks.Count - 1)
            {
                if (chunker.finished)
                    return;
                if (chunker is TextChunker tchunker)
                    tchunker.AddChunk();
                else
                    chunker.AddAllChunks();
            }
            chunker.chunkSelected += 1;
            ChunkForm.OpenChunk(chunker, chunker.chunkSelected);
            int cur_chunk_count = chunker.chunks.Count;
            if (chunkForm != null)
            {
                if (prev_selected >= cur_chunk_count - 2 || cur_chunk_count > prev_chunk_count)
                    chunkForm.ChunkTreePopulate();
                else
                    chunkForm.MoveSelectedImage(prev_selected);
            }
        }

        public static void LastChunk(BaseChunker chunker, ChunkForm chunkForm)
        {
            int prev_selected = chunker.chunkSelected;
            int prev_chunk_count = chunker.chunks.Count;
            chunker.AddAllChunks();
            if (chunker.chunks.Count == 0) return;
            ChunkForm.OpenChunk(chunker, chunker.chunks.Count - 1);
            if (chunkForm != null)
            {
                if (prev_chunk_count == chunker.chunks.Count)
                    chunkForm.MoveSelectedImage(prev_selected);
                else
                    chunkForm.ChunkTreePopulate();
            }
        }

        private void ChunkTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            int prev_selected = Main.chunker.chunkSelected;
            ChunkForm.OpenChunk(Main.chunker, e.Node.Index);
            MoveSelectedImage(prev_selected);
        }

        private void FirstButton_Click(object sender, EventArgs e)
        {
            ChunkForm.FirstChunk(Main.chunker, this);
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            ChunkForm.PreviousChunk(Main.chunker, this);
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            ChunkForm.NextChunk(Main.chunker, this);
        }

        private void LastButton_Click(object sender, EventArgs e)
        {
            ChunkForm.LastChunk(Main.chunker, this);
        }

        private void ChooseFileButton_Click(object sender, EventArgs e)
        {
            Main.ChooseFile();
        }
    }
}
