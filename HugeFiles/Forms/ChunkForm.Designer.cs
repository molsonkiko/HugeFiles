namespace HugeFiles.Forms
{
    partial class ChunkForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChunkForm));
            this.ChunkTree = new System.Windows.Forms.TreeView();
            this.ChooseFileButton = new System.Windows.Forms.Button();
            this.PreviousButton = new System.Windows.Forms.Button();
            this.FirstButton = new System.Windows.Forms.Button();
            this.NextButton = new System.Windows.Forms.Button();
            this.LastButton = new System.Windows.Forms.Button();
            this.ChunkIconList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // ChunkTree
            // 
            this.ChunkTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChunkTree.Location = new System.Drawing.Point(3, 36);
            this.ChunkTree.Name = "ChunkTree";
            this.ChunkTree.Size = new System.Drawing.Size(337, 575);
            this.ChunkTree.TabIndex = 0;
            this.ChunkTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ChunkTree_NodeMouseClick);
            // 
            // ChooseFileButton
            // 
            this.ChooseFileButton.Location = new System.Drawing.Point(3, 1);
            this.ChooseFileButton.Name = "ChooseFileButton";
            this.ChooseFileButton.Size = new System.Drawing.Size(97, 29);
            this.ChooseFileButton.TabIndex = 1;
            this.ChooseFileButton.Text = "Choose file...";
            this.ChooseFileButton.UseVisualStyleBackColor = true;
            this.ChooseFileButton.Click += new System.EventHandler(this.ChooseFileButton_Click);
            // 
            // PreviousButton
            // 
            this.PreviousButton.Location = new System.Drawing.Point(156, 1);
            this.PreviousButton.Name = "PreviousButton";
            this.PreviousButton.Size = new System.Drawing.Size(71, 29);
            this.PreviousButton.TabIndex = 2;
            this.PreviousButton.Text = "Previous";
            this.PreviousButton.UseVisualStyleBackColor = true;
            this.PreviousButton.Click += new System.EventHandler(this.PreviousButton_Click);
            // 
            // FirstButton
            // 
            this.FirstButton.Location = new System.Drawing.Point(106, 1);
            this.FirstButton.Name = "FirstButton";
            this.FirstButton.Size = new System.Drawing.Size(44, 29);
            this.FirstButton.TabIndex = 3;
            this.FirstButton.Text = "First";
            this.FirstButton.UseVisualStyleBackColor = true;
            this.FirstButton.Click += new System.EventHandler(this.FirstButton_Click);
            // 
            // NextButton
            // 
            this.NextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NextButton.Location = new System.Drawing.Point(244, 1);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(46, 29);
            this.NextButton.TabIndex = 4;
            this.NextButton.Text = "Next";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // LastButton
            // 
            this.LastButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LastButton.Location = new System.Drawing.Point(296, 1);
            this.LastButton.Name = "LastButton";
            this.LastButton.Size = new System.Drawing.Size(44, 29);
            this.LastButton.TabIndex = 5;
            this.LastButton.Text = "Last";
            this.LastButton.UseVisualStyleBackColor = true;
            this.LastButton.Click += new System.EventHandler(this.LastButton_Click);
            // 
            // ChunkIconList
            // 
            this.ChunkIconList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ChunkIconList.ImageStream")));
            this.ChunkIconList.TransparentColor = System.Drawing.Color.Transparent;
            this.ChunkIconList.Images.SetKeyName(0, "Saved.ico");
            this.ChunkIconList.Images.SetKeyName(1, "Unsaved.ico");
            this.ChunkIconList.Images.SetKeyName(2, "Viewing_saved.ico");
            this.ChunkIconList.Images.SetKeyName(3, "Viewing_unsaved.ico");
            this.ChunkIconList.Images.SetKeyName(4, "root icon.PNG");
            // 
            // ChunkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 615);
            this.Controls.Add(this.LastButton);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.FirstButton);
            this.Controls.Add(this.PreviousButton);
            this.Controls.Add(this.ChooseFileButton);
            this.Controls.Add(this.ChunkTree);
            this.Name = "ChunkForm";
            this.Text = "Load a chunk of file";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView ChunkTree;
        private System.Windows.Forms.Button ChooseFileButton;
        private System.Windows.Forms.Button PreviousButton;
        private System.Windows.Forms.Button FirstButton;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.Button LastButton;
        private System.Windows.Forms.ImageList ChunkIconList;
    }
}