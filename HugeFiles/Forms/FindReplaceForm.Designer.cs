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
            this.SuspendLayout();
            // 
            // SearchResultTree
            // 
            this.SearchResultTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchResultTree.Location = new System.Drawing.Point(209, 74);
            this.SearchResultTree.Name = "SearchResultTree";
            this.SearchResultTree.Size = new System.Drawing.Size(234, 371);
            this.SearchResultTree.TabIndex = 0;
            this.SearchResultTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.SearchResultsTree_NodeClick);
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new System.Drawing.Point(110, 9);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(216, 25);
            this.Title.TabIndex = 1;
            this.Title.Text = "Find text in Huge File";
            // 
            // NotAllResultsShownFlag
            // 
            this.NotAllResultsShownFlag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NotAllResultsShownFlag.AutoSize = true;
            this.NotAllResultsShownFlag.Location = new System.Drawing.Point(206, 46);
            this.NotAllResultsShownFlag.Name = "NotAllResultsShownFlag";
            this.NotAllResultsShownFlag.Size = new System.Drawing.Size(177, 16);
            this.NotAllResultsShownFlag.TabIndex = 2;
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
            // 
            // PatternBoxLabel
            // 
            this.PatternBoxLabel.AutoSize = true;
            this.PatternBoxLabel.Location = new System.Drawing.Point(12, 74);
            this.PatternBoxLabel.Name = "PatternBoxLabel";
            this.PatternBoxLabel.Size = new System.Drawing.Size(147, 16);
            this.PatternBoxLabel.TabIndex = 4;
            this.PatternBoxLabel.Text = "Text/regex to search for";
            // 
            // RegexCheckBox
            // 
            this.RegexCheckBox.AutoSize = true;
            this.RegexCheckBox.Location = new System.Drawing.Point(15, 143);
            this.RegexCheckBox.Name = "RegexCheckBox";
            this.RegexCheckBox.Size = new System.Drawing.Size(182, 20);
            this.RegexCheckBox.TabIndex = 5;
            this.RegexCheckBox.Text = "Use regular expressions?";
            this.RegexCheckBox.UseVisualStyleBackColor = true;
            // 
            // IgnoreCaseCheckBox
            // 
            this.IgnoreCaseCheckBox.AutoSize = true;
            this.IgnoreCaseCheckBox.Location = new System.Drawing.Point(15, 185);
            this.IgnoreCaseCheckBox.Name = "IgnoreCaseCheckBox";
            this.IgnoreCaseCheckBox.Size = new System.Drawing.Size(134, 20);
            this.IgnoreCaseCheckBox.TabIndex = 6;
            this.IgnoreCaseCheckBox.Text = "Case insensitive?";
            this.IgnoreCaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // SearchButton
            // 
            this.SearchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SearchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SearchButton.Location = new System.Drawing.Point(15, 227);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(144, 35);
            this.SearchButton.TabIndex = 7;
            this.SearchButton.Text = "Search for text";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // FindReplaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 457);
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
    }
}