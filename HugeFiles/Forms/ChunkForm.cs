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
        public Chunker chunker;

        public ChunkForm(Chunker chunker)
        {
            InitializeComponent();
            this.chunker = chunker;
        }

        public void ChunkTreePopulate()
        {
            if (ChunkTree.ImageList == null)
                ChunkTree.ImageList = ChunkIconList;
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
            if (start == chunker.chunkSelected || start < 0 || start >= root.Nodes.Count)
                return;
            // first two images are for non-viewed, last two are for viewed
            TreeNode startnode = root.Nodes[start];
            TreeNode endnode = root.Nodes[chunker.chunkSelected];
            startnode.ImageIndex -= 2;
            startnode.SelectedImageIndex -= 2;
            endnode.ImageIndex += 2;
            endnode.SelectedImageIndex += 2;
        }

        public static void OpenChunk(Chunker chunker, int chunkSelected)
        {
            if (chunker.buffName == "")
            {
                Npp.notepad.FileNew();
                chunker.buffName = Npp.GetCurrentPath();
            }
            if (chunker.buffName != Npp.GetCurrentPath())
                return;
            Npp.editor.SetText(chunker.ReadChunk(chunkSelected));
        }

        public static void FirstChunk(Chunker chunker, ChunkForm chunkForm)
        {
            int prev_selected = chunker.chunkSelected;
            ChunkForm.OpenChunk(chunker, 0);
            if (chunkForm != null)
            {
                if (chunker.chunks.Count > 1)
                    chunkForm.MoveSelectedImage(prev_selected);
                else if (prev_selected == -1)
                    chunkForm.ChunkTreePopulate();
            }
                
        }

        public static void PreviousChunk(Chunker chunker, ChunkForm chunkForm)
        {
            if (chunker.chunks.Count == 0)
                return;
            int prev_selected = chunker.chunkSelected;
            if (chunker.chunkSelected > 0)
                chunker.chunkSelected -= 1;
            ChunkForm.OpenChunk(chunker, chunker.chunkSelected);
            if (chunkForm != null)
                chunkForm.MoveSelectedImage(prev_selected);
        }

        public static void NextChunk(Chunker chunker, ChunkForm chunkForm)
        {
            int prev_selected = chunker.chunkSelected;
            if (!(chunker.finished && chunker.chunkSelected == chunker.chunks.Count - 1))
                chunker.chunkSelected++;
            ChunkForm.OpenChunk(chunker, chunker.chunkSelected);
            if (chunkForm != null)
            {
                if (prev_selected >= chunker.chunks.Count - 2)
                    chunkForm.ChunkTreePopulate();
                else
                    chunkForm.MoveSelectedImage(prev_selected);
            }
        }

        public static void LastChunk(Chunker chunker, ChunkForm chunkForm)
        {
            int prev_selected = chunker.chunkSelected;
            int prev_chunk_count = chunker.chunks.Count;
            chunker.AddAllChunks();
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
            int prev_selected = chunker.chunkSelected;
            ChunkForm.OpenChunk(chunker, e.Node.Index);
            MoveSelectedImage(prev_selected);
        }

        private void FirstButton_Click(object sender, EventArgs e)
        {
            ChunkForm.FirstChunk(chunker, this);
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            ChunkForm.PreviousChunk(chunker, this);
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            ChunkForm.NextChunk(chunker, this);
        }

        private void LastButton_Click(object sender, EventArgs e)
        {
            ChunkForm.LastChunk(chunker, this);
        }

        private void ChooseFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All files|*.*|Text files|*.txt";
            ofd.InitialDirectory = @"C:\";
            ofd.Title = "Open file for chunking with HugeFiles";
            ofd.CheckFileExists = true;
            string fname;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fname = ofd.FileName;
                if (!File.Exists(fname))
                {
                    return;
                }
                chunker.ChooseNewFile(fname);
                ChunkTreePopulate();
            }
        }
    }
}
