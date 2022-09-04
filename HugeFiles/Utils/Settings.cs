using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HugeFiles.Utils
{
    /// <summary>
    /// Manages application settings
    /// </summary>
    public class Settings
    {
        private const int DEFAULT_WIDTH = 350;
        private const int DEFAULT_HEIGHT = 450;

        // private static readonly string IniFilePath;

        [Description("Delimiter for chunks."),
            Category("Chunker")]
        public string delimiter { get; set; } = "\\r\\n";
        // \r, \n, and \t are invisible by default, so this is a hack to let the user see them
        // this is undone elsewhere

        [Description("Minimum chunk size."),
            Category("Chunker")]
        public int minChunk { get; set; } = 180_000;

        [Description("Maximum chunk size."),
            Category("Chunker")]
        public int maxChunk { get; set; } = 220_000;

        [Description("Number of characters in preview of each chunk."),
            Category("Chunker")]
        public int previewLength { get; set; } = 0;

        public bool changed = false;

        /// <summary>
        /// Opens a window that edits all settings
        /// </summary>
        public void ShowDialog(bool debug=false)
        {
            // We bind a copy of this object and only apply it after they click "Ok"
            var copy = (Settings)MemberwiseClone();
            changed = false;
            
            var dialog = new Form
            {
                Text = "Settings - JSON Viewer plug-in",
                ClientSize = new Size(DEFAULT_WIDTH, DEFAULT_HEIGHT),
                MinimumSize = new Size(250, 250),
                ShowIcon = false,
                AutoScaleMode = AutoScaleMode.Font,
                AutoScaleDimensions = new SizeF(6F, 13F),
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.CenterParent,
                Controls =
                {
                    new Button
                    {
                        Name = "Cancel",
                        Text = "&Cancel",
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                        Size = new Size(75, 23),
                        Location = new Point(DEFAULT_WIDTH - 115, DEFAULT_HEIGHT - 36),
                        UseVisualStyleBackColor = true
                    },
                    new Button
                    {
                        Name = "Reset",
                        Text = "&Reset",
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                        Size = new Size(75, 23),
                        Location = new Point(DEFAULT_WIDTH - 212, DEFAULT_HEIGHT - 36),
                        UseVisualStyleBackColor = true
                    },
                    new Button
                    {
                        Name = "Ok",
                        Text = "&Ok",
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                        Size = new Size(75, 23),
                        Location = new Point(DEFAULT_WIDTH - 310, DEFAULT_HEIGHT - 36),
                        UseVisualStyleBackColor = true
                    },
                    new PropertyGrid
                    {
                        Name = "Grid",
                        Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                        Location = new Point(13, 13),
                        Size = new Size(DEFAULT_WIDTH - 13 - 13, DEFAULT_HEIGHT - 55),
                        AutoScaleMode = AutoScaleMode.Font,
                        AutoScaleDimensions = new SizeF(6F,13F),
                        SelectedObject = copy
                    },
                }
            };
            dialog.Controls["Cancel"].Click += (a, b) =>
            {
                dialog.Close();
            };
            dialog.Controls["Ok"].Click += (a, b) =>
            {
                // change the settings to whatever the user selected
                var changesEventArgs = new SettingsChangedEventArgs(this, copy);
                if (!changesEventArgs.Changed.Any())
                {
                    dialog.Close();
                    return;
                }
                changed = true;
                delimiter = copy.delimiter;
                minChunk = copy.minChunk;
                maxChunk = copy.maxChunk;
                previewLength = copy.previewLength;
                dialog.Close();
            };
            dialog.Controls["Reset"].Click += (a, b) =>
            {
                // reset the settings to defaults
                changed = true;
                delimiter = "\\r\\n";
                minChunk = 180_000;
                maxChunk = 220_000;
                previewLength = 0;
                dialog.Close();
            };
            dialog.ShowDialog();
        }
    }



    public class SettingsChangedEventArgs : CancelEventArgs
    {
        public SettingsChangedEventArgs(Settings oldSettings, Settings newSettings)
        {
            OldSettings = oldSettings;
            NewSettings = newSettings;
            Changed = new HashSet<string>();
            foreach (var propertyInfo in typeof(Settings).GetProperties())
            {
                var oldValue = propertyInfo.GetValue(oldSettings, null);
                var newValue = propertyInfo.GetValue(newSettings, null);
                if (!oldValue.Equals(newValue))
                {
                    Trace.TraceInformation($"Setting {propertyInfo.Name} has changed");
                    Changed.Add(propertyInfo.Name);
                }
            }
        }

        public HashSet<string> Changed { get; }
        public Settings OldSettings { get; }
        public Settings NewSettings { get; }
    }
}
