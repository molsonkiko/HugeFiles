namespace NppPluginNET.Forms
{
    partial class AboutForm
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
            this.Title = new System.Windows.Forms.Label();
            this.Description = new System.Windows.Forms.Label();
            this.DocsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new System.Drawing.Point(116, 9);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(121, 24);
            this.Title.TabIndex = 0;
            this.Title.Text = "placeholder";
            // 
            // Description
            // 
            this.Description.AutoSize = true;
            this.Description.Location = new System.Drawing.Point(21, 64);
            this.Description.Name = "Description";
            this.Description.Size = new System.Drawing.Size(322, 48);
            this.Description.TabIndex = 1;
            this.Description.Text = "HugeFiles is a plugin for viewing and editing\r\nvery large files that would crash " +
    "Notepad++\r\nor make it unresponsive if you opened them normally.";
            // 
            // DocsButton
            // 
            this.DocsButton.Location = new System.Drawing.Point(126, 159);
            this.DocsButton.Name = "DocsButton";
            this.DocsButton.Size = new System.Drawing.Size(111, 23);
            this.DocsButton.TabIndex = 2;
            this.DocsButton.Text = "Documentation";
            this.DocsButton.UseVisualStyleBackColor = true;
            this.DocsButton.Click += new System.EventHandler(this.AboutButton_Click);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 214);
            this.Controls.Add(this.DocsButton);
            this.Controls.Add(this.Description);
            this.Controls.Add(this.Title);
            this.Name = "AboutForm";
            this.Text = "About HugeFiles";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.Label Description;
        private System.Windows.Forms.Button DocsButton;
    }
}