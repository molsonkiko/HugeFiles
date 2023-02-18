using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CsvQuery.PluginInfrastructure;

namespace HugeFiles.Utils
{
    /// <summary>
    /// Manages application settings
    /// </summary>
    public class Settings : SettingsBase
    {
        [Description("Delimiter for chunks. Leave blank to make all chunks the same length."),
            Category("Chunker"), DefaultValue("\\r\\n")]
        public string delimiter { get; set; }
        // \r, \n, and \t are invisible by default, so this is a hack to let the user see them
        // that is undone elsewhere

        [Description("Minimum chunk size."),
            Category("Chunker"), DefaultValue(180_000)]
        public int minChunk { get; set; }

        [Description("Maximum chunk size."),
            Category("Chunker"), DefaultValue(220_000)]
        public int maxChunk { get; set; }

        [Description("Number of characters in preview of each chunk."),
            Category("Chunker"), DefaultValue(20)]
        public int previewLength { get; set; }

        [Description("Whether to automatically set minChunk, maxChunk and delimiter to inferred best settings for the file"),
            Category("Inference"), DefaultValue(true)]
        public bool autoInferBestDelimiterAndTolerance { get; set; }

        [Description("Parse a file as JSON even if it doesn't have a .json extension"),
            Category("JSON"), DefaultValue(false)]
        public bool parseNonJsonAsJson { get; set; }

        [Description("Parse JSON as JSON if it DOES have the .json extension. " +
                     "Setting this to False can improve performance, but chunks will not be valid JSON."),
            Category("JSON"), DefaultValue(true)]
        public bool parseJsonAsJson { get; set; }

        //[Description("Parse a file as XML even if it doesn't have a .xml extension"),
        //    Category("Chunker"), DefaultValue(false)]
        //public bool parseNonXMLAsXML { get; set; }
    }
}
