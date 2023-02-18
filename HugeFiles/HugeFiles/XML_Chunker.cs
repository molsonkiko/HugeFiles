using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HugeFiles.Utils;

namespace HugeFiles.HugeFiles
{
    /// <summary>
    /// Breaks up a large XML file such that the start character of each
    /// chunk comes right before a different child of the root.<br></br>
    /// For example, the file<br></br>
    /// &lt;foo&gt;<br></br>
    ///     &lt;bar&gt;
    ///         &lt;baz zug="fun"&gt;hello&lt;/baz&gt;
    ///     &lt;/bar&gt;<br></br>
    ///     &lt;bar&gt;
    ///         &lt;baz zug="dog"&gt;world&lt;/baz&gt;
    ///     &lt;/bar&gt;<br></br>
    ///     &lt;bar&gt;
    ///         &lt;baz zug="cat"&gt;!!&lt;/baz&gt;
    ///     &lt;/bar&gt;<br></br>
    /// &lt;/foo&gt;<br></br>
    /// would be chunked such that each chunk would be at the beginning of
    /// each &lt;bar&gt; element.
    /// </summary>
    public class XML_Chunker
    {
        public static void chunk()
        {

        }
    }
}
