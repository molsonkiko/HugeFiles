namespace NppPluginNET.Forms
{
    partial class FindReplaceForm
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
            this.SearchResultTree = new System.Windows.Forms.TreeView();
            this.Title = new System.Windows.Forms.Label();
            this.NotAllResultsShownFlag = new System.Windows.Forms.Label();
            this.PatternBox = new System.Windows.Forms.TextBox();
            this.PatternBoxLabel = new System.Windows.Forms.Label();
            this.RegexCheckBox = new System.Windows.Forms.CheckBox();
            this.IgnoreCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.SearchButton = new System.Windows.Forms.Button();
            this.ReplacementBoxLabel = new System.Windows.Forms.Label();
            this.ReplacementBox = new System.Windows.Forms.TextBox();
            this.TargetFnameBoxLabel = new System.Windows.Forms.Label();
            this.TargetFnameBox = new System.Windows.Forms.TextBox();
            this.OverwriteCheckBox = new System.Windows.Forms.CheckBox();
            this.ReplaceButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SearchResultTree
            // 
            this.SearchResultTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchResultTree.Location = new System.Drawing.Point(209, 74);
            this.SearchResultTree.Name = "SearchResultTree";
            this.SearchResultTree.Size = new System.Drawing.Size(234, 430);
            this.SearchResultTree.TabIndex = 14;
            this.SearchResultTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.SearchResultsTree_NodeClick);
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new System.Drawing.Point(66, 9);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(294, 25);
            this.Title.TabIndex = 1;
            this.Title.Text = "Find/replace text in Huge File";
            // 
            // NotAllResultsShownFlag
            // 
            this.NotAllResultsShownFlag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NotAllResultsShownFlag.AutoSize = true;
            this.NotAllResultsShownFlag.Location = new System.Drawing.Point(206, 46);
            this.NotAllResultsShownFlag.Name = "NotAllResultsShownFlag";
            this.NotAllResultsShownFlag.Size = new System.Drawing.Size(177, 16);
            this.NotAllResultsShownFlag.TabIndex = 13;
            this.NotAllResultsShownFlag.Text = "Only some results are shown";
            this.NotAllResultsShownFlag.Visible = false;
            // 
            // PatternBox
            // 
            this.PatternBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PatternBox.Location = new System.Drawing.Point(15, 103);
            this.PatternBox.Name = "PatternBox";
            this.PatternBox.Size = new System.Drawing.Size(188, 22);
            this.PatternBox.TabIndex = 3;
            this.PatternBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
            this.PatternBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FindReplaceForm_KeyUp);
            // 
            // PatternBoxLabel
            // 
            this.PatternBoxLabel.AutoSize = true;
            this.PatternBoxLabel.Location = new System.Drawing.Point(12, 74);
            this.PatternBoxLabel.Name = "PatternBoxLabel";
            this.PatternBoxLabel.Size = new System.Drawing.Size(147, 16);
            this.PatternBoxLabel.TabIndex = 2;
            this.PatternBoxLabel.Text = "Text/regex to search for";
            // 
            // RegexCheckBox
            // 
            this.RegexCheckBox.AutoSize = true;
            this.RegexCheckBox.Location = new System.Drawing.Point(15, 217);
            this.RegexCheckBox.Name = "RegexCheckBox";
            this.RegexCheckBox.Size = new System.Drawing.Size(182, 20);
            this.RegexCheckBox.TabIndex = 6;
            this.RegexCheckBox.Text = "Use regular expressions?";
            this.RegexCheckBox.UseVisualStyleBackColor = true;
            this.RegexCheckBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FindReplaceForm_KeyUp);
            // 
            // IgnoreCaseCheckBox
            // 
            this.IgnoreCaseCheckBox.AutoSize = true;
            this.IgnoreCaseCheckBox.Location = new System.Drawing.Point(15, 259);
            this.IgnoreCaseCheckBox.Name = "IgnoreCaseCheckBox";
            this.IgnoreCaseCheckBox.Size = new System.Drawing.Size(134, 20);
            this.IgnoreCaseCheckBox.TabIndex = 7;
            this.IgnoreCaseCheckBox.Text = "Case insensitive?";
            this.IgnoreCaseCheckBox.UseVisualStyleBackColor = true;
            this.IgnoreCaseCheckBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FindReplaceForm_KeyUp);
            // 
            // SearchButton
            // 
            this.SearchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SearchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SearchButton.Location = new System.Drawing.Point(15, 295);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(144, 35);
            this.SearchButton.TabIndex = 8;
            this.SearchButton.Text = "Search for text";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            this.SearchButton.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FindReplaceForm_KeyUp);
            // 
            // ReplacementBoxLabel
            // 
            this.ReplacementBoxLabel.AutoSize = true;
            this.ReplacementBoxLabel.Location = new System.Drawing.Point(12, 146);
            this.ReplacementBoxLabel.Name = "ReplacementBoxLabel";
            this.ReplacementBoxLabel.Size = new System.Drawing.Size(159, 16);
            this.ReplacementBoxLabel.TabIndex = 4;
            this.ReplacementBoxLabel.Text = "Text/regex to replace with";
            // 
            // ReplacementBox
            // 
            this.ReplacementBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReplacementBox.Location = new System.Drawing.Point(15, 175);
            this.ReplacementBox.Name = "ReplacementBox";
            this.ReplacementBox.Size = new System.Drawing.Size(188, 22);
            this.ReplacementBox.TabIndex = 5;
            this.ReplacementBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
            this.ReplacementBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FindReplaceForm_KeyUp);
            // 
            // TargetFnameBoxLabel
            // 
            this.TargetFnameBoxLabel.AutoSize = true;
            this.TargetFnameBoxLabel.Location = new System.Drawing.Point(12, 349);
            this.TargetFnameBoxLabel.Name = "TargetFnameBoxLabel";
            this.TargetFnameBoxLabel.Size = new System.Drawing.Size(140, 16);
            this.TargetFnameBoxLabel.TabIndex = 9;
            this.TargetFnameBoxLabel.Text = "Filename to replace to";
            // 
            // TargetFnameBox
            // 
            this.TargetFnameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TargetFnameBox.Location = new System.Drawing.Point(15, 382);
            this.TargetFnameBox.Name = "TargetFnameBox";
            this.TargetFnameBox.Size = new System.Drawing.Size(188, 22);
            this.TargetFnameBox.TabIndex = 10;
            this.TargetFnameBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
            this.TargetFnameBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FindReplaceForm_KeyUp);
            // 
            // OverwriteCheckBox
            // 
            this.OverwriteCheckBox.AutoSize = true;
            this.OverwriteCheckBox.Location = new System.Drawing.Point(15, 420);
            this.OverwriteCheckBox.Name = "OverwriteCheckBox";
            this.OverwriteCheckBox.Size = new System.Drawing.Size(159, 20);
            this.OverwriteCheckBox.TabIndex = 11;
            this.OverwriteCheckBox.Text = "Overwrite original file?";
            this.OverwriteCheckBox.UseVisualStyleBackColor = true;
            this.OverwriteCheckBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FindReplaceForm_KeyUp);
            // 
            // ReplaceButton
            // 
            this.ReplaceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ReplaceButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReplaceButton.Location = new System.Drawing.Point(15, 458);
            this.ReplaceButton.Name = "ReplaceButton";
            this.ReplaceButton.Size = new System.Drawing.Size(144, 35);
            this.ReplaceButton.TabIndex = 12;
            this.ReplaceButton.Text = "Replace";
            this.ReplaceButton.UseVisualStyleBackColor = true;
            this.ReplaceButton.Click += new System.EventHandler(this.ReplaceButton_Click);
            this.ReplaceButton.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FindReplaceForm_KeyUp);
            // 
            // FindReplaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 516);
            this.Controls.Add(this.ReplaceButton);
            this.Controls.Add(this.OverwriteCheckBox);
            this.Controls.Add(this.TargetFnameBoxLabel);
            this.Controls.Add(this.TargetFnameBox);
            this.Controls.Add(this.ReplacementBoxLabel);
            this.Controls.Add(this.ReplacementBox);
            this.Controls.Add(this.SearchButton);
            this.Controls.Add(this.IgnoreCaseCheckBox);
            this.Controls.Add(this.RegexCheckBox);
            this.Controls.Add(this.PatternBoxLabel);
            this.Controls.Add(this.PatternBox);
            this.Controls.Add(this.NotAllResultsShownFlag);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.SearchResultTree);
            this.Name = "FindReplaceForm";
            this.Text = "Find text in huge file";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FindReplaceForm_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView SearchResultTree;
        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.Label NotAllResultsShownFlag;
        private System.Windows.Forms.Label PatternBoxLabel;
        private System.Windows.Forms.CheckBox RegexCheckBox;
        private System.Windows.Forms.CheckBox IgnoreCaseCheckBox;
        private System.Windows.Forms.Button SearchButton;
        internal System.Windows.Forms.TextBox PatternBox;
        private System.Windows.Forms.Label ReplacementBoxLabel;
        internal System.Windows.Forms.TextBox ReplacementBox;
        private System.Windows.Forms.Label TargetFnameBoxLabel;
        internal System.Windows.Forms.TextBox TargetFnameBox;
        private System.Windows.Forms.CheckBox OverwriteCheckBox;
        private System.Windows.Forms.Button ReplaceButton;
    }
}