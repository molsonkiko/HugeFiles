/*
A parser and linter for JSON, with the aim of finding the characters
at which elements of a top-level iterable begin.

For example, the JSON
[1, 2, 3, [1, 2, 3], 4]
has top-level elements beginning at characters [0, 4, 7, 10, 21].
*/
using Kbg.NppPluginNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace HugeFiles.HugeFiles
{
    /// <summary>
    /// An exception thrown when the parser encounters syntactically invalid JSON.
    /// Subclasses FormatException.
    /// </summary>
    public class JsonChunkerException : FormatException
    {
        public new string Message { get; set; }
        public char Cur_char { get; set; }
        public long Pos { get; set; }

        public JsonChunkerException(string Message, char cur_char, long pos)
        {
            this.Message = Message;
            this.Cur_char = cur_char;
            this.Pos = pos;
        }

        public JsonChunkerException(string Message)
        {
            this.Message = Message;
            this.Cur_char = '\x00';
            this.Pos = 0;
        }

        public override string ToString()
        {
            return $"{Message} at position {Pos} (char '{Cur_char}')";
        }
    }

    /// <summary>
    /// Parses a JSON document into a <seealso cref="JNode"/> tree.
    /// </summary>
    public class JsonChunker : BaseChunker
    {
        // need to track recursion depth because stack overflow causes a panic that makes Notepad++ crash
        public const int MAX_RECURSION_DEPTH = 512;

        /// <summary>
        /// number of extra bytes due to unicode chars
        /// </summary>
        public int unicodeExtraBytes;

        public int chunkSize;

        /// <summary>
        /// the start char of the current chunk
        /// </summary>
        public long lastBoundary;

        /// <summary>
        /// whether we are currently between two chunks.<br></br>
        /// This is true after the comma between the end of a chunk
        /// and the beginning of the next chunk.
        /// </summary>
        public bool chunkJustEnded;

        /// <summary>
        /// Whether the top-level root element is an object (as opposed to an array or non-iterable)
        /// </summary>
        public bool isObject;
        public bool isArray;

        /// <summary>
        /// breaks a file into chunks
        /// </summary>
        public JsonChunker(string fname, int minChunk, int maxChunk) : base(fname)
        {
            chunkSize = (minChunk + maxChunk) / 2;
            lastBoundary = 0;
            unicodeExtraBytes = 0;
            isObject = false;
        }

        #region HELPER_METHODS
        /// <summary>
        /// parse any number of consecutive whitespace characters and return the next non-whitespace char
        /// </summary>
        /// <param name="inp">json string</param>
        /// <returns>position of first non-whitespace char after (or at) start position</returns>
        private int ConsumeWhiteSpace(string inp, int ii)
        {
            char c;
            while (ii < inp.Length)
            {
                c = inp[ii];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                    ii++;
                else break;
            }
            return ii;
        }

        /// <summary>
        /// read a hexadecimal integer representation of length `length` at position `index` in `inp`.
        /// Throws an JsonChunkerException of the integer is not valid hexadecimal
        /// or if `index` is less than `length` from the end of `inp`.
        /// </summary>
        /// <returns>the end position, and the hexadecimal number parsed</returns>
        /// <exception cref="JsonChunkerException"></exception>
        private int ParseHexadecimal(string inp, int length, int ii)
        {
            int end = ii + length > inp.Length ? inp.Length : ii + length;
            char c;
            for (; ii < end; ii++)
            {
                c = inp[ii];
                if (!(('0' <= c  && c <= '9') || ('a' <= c && c <= 'f') || ('A' <= c && c <= 'F')))
                    throw new JsonChunkerException("Could not find valid hexadecimal of length " + length,
                        c, ii);
            }
            return ii;
        }

        public static Dictionary<char, char> ESCAPE_MAP = new Dictionary<char, char>
        {
            { '\\', '\\' },
            { 'n', '\n' },
            { 'r', '\r' },
            { 'b', '\b' },
            { 't', '\t' },
            { 'f', '\f' },
        };

        #endregion
        #region PARSER_FUNCTIONS

        /// <summary>
        /// Parse anything (a scalar, null, an object, or an array) in a JSON string.<br></br>
        /// May raise a JsonChunkerException for any of the following reasons:<br></br>
        /// 1. Whatever reasons ParseObject, ParseArray, or ParseString might throw an error.<br></br>
        /// 2. An unquoted string other than true, false, null, NaN, Infinity, -Infinity.<br></br>
        /// 3. The JSON string contains only blankspace or is empty.
        /// </summary>
        /// <param name="chunks">the start positions of all the top-level elements found so far</param>
        /// <returns>the position after the end of this element</returns>
        /// <exception cref="JsonChunkerException"></exception>
        public int ParseSomething(string inp, int recursion_depth, int ii)
        {
            while (ii < inp.Length)
            {
                char c = inp[ii];
                bool alreadySeenComma;
                int elementCount;
                bool recDepth1;
                switch (c)
                {
                    case '"': // STRING
                        int start = ii++;
                        while (true)
                        {
                            if (ii == inp.Length)
                            {
                                throw new JsonChunkerException("Unterminated string literal starting at position " + start, inp[ii - 1], ii - 1);
                            }
                            c = inp[ii];
                            if (c == '\n')
                            {
                                // internal newlines are not allowed in JSON strings
                                throw new JsonChunkerException("Unterminated string literal starting at position " + start, c, ii);
                            }
                            if (c == '"')
                            {
                                break;
                            }
                            else if (c == '\\')
                            {
                                if (ii >= inp.Length - 2)
                                {
                                    throw new JsonChunkerException("Unterminated string literal starting at position " + start, c, ii);
                                }
                                char next_char = inp[ii + 1];
                                if (next_char == '"')
                                {
                                    ii += 2;
                                    continue;
                                }
                                if (ESCAPE_MAP.TryGetValue(next_char, out _))
                                {
                                    ii += 2;
                                    continue;
                                }
                                if (next_char == 'u')
                                {
                                    // 2-byte unicode of the form \uxxxx
                                    // \x and \U escapes are not part of the JSON standard
                                    ii = ParseHexadecimal(inp, 4, ii + 2) - 1;
                                }
                                else
                                {
                                    throw new JsonChunkerException("Invalidly escaped char", next_char, ii + 1);
                                }
                            }
                            else if (c > 127)
                            {
                                if (c > 2047)
                                {
                                    // check if it's in the surrogate pair region
                                    // each member of a surrogate pair counts as 2 bytes
                                    // for a total of 4 bytes for the unicode characters over 65535
                                    if (c >= 0xd800 && c <= 0xdfff)
                                        unicodeExtraBytes += 1;
                                    // all other character bigger than 2047 take up 3 bytes
                                    else unicodeExtraBytes += 2;
                                }
                                // non-ascii chars less than 2048 take up 2 bytes
                                else unicodeExtraBytes += 1;
                            }
                            ii++;
                        }
                        return ii + 1;
                    case '-':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9': // NUMBER
                        // parsed tracks which portions of a number have been parsed.
                        // So if the int part has been parsed, it will be 1.
                        // If the int and decimal point parts have been parsed, it will be 3.
                        // If the int, decimal point, and scientific notation parts have been parsed, it will be 7
                        int parsed = 1;
                        if (c == '-')
                        {
                            ii++;
                        }
                        while (ii < inp.Length)
                        {
                            c = inp[ii];
                            if (c >= '0' && c <= '9')
                            {
                                ii++;
                            }
                            else if (c == '.')
                            {
                                if (parsed != 1)
                                {
                                    throw new JsonChunkerException("Number with two decimal points", c, ii);
                                }
                                parsed = 3;
                                ii++;
                            }
                            else if (c == 'e' || c == 'E')
                            {
                                if ((parsed & 4) != 0)
                                {
                                    break;
                                }
                                parsed += 4;
                                if (ii < inp.Length - 1)
                                {
                                    c = inp[++ii];
                                    if (c == '+' || c == '-')
                                    {
                                        ii++;
                                    }
                                }
                            }
                            else if (c == '/')
                            {
                                // fractions are part of the JSON language specification
                                // to reduce call overhead we wil deal with fractions by resetting the number-parsing state
                                // this is equivalent to saying "we're ready to parse a new number"
                                parsed = 1;
                                ii++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        return ii;
                    case '[': // ARRAY
                        recursion_depth++;
                        alreadySeenComma = false;
                        recDepth1 = recursion_depth == 1;
                        c = inp[++ii];
                        // need to do this to avoid stack overflow when presented with unreasonably deep nesting
                        // stack overflow causes an unrecoverable panic, and we would rather fail gracefully
                        if (recursion_depth == MAX_RECURSION_DEPTH)
                            throw new JsonChunkerException($"Maximum recursion depth ({MAX_RECURSION_DEPTH}) reached",
                                c, ii);
                        elementCount = 0;
                        while (ii < inp.Length)
                        {
                            ii = ConsumeWhiteSpace(inp, ii);
                            c = inp[ii];
                            if (c == ',')
                            {
                                if (alreadySeenComma)
                                {
                                    throw new JsonChunkerException($"Two consecutive commas after element {elementCount - 1} of array",
                                        c, ii);
                                }
                                alreadySeenComma = true;
                                if (elementCount == 0)
                                {
                                    throw new JsonChunkerException("Comma before first value in array",
                                        c, ii);
                                }
                                ii++;
                                continue;
                            }
                            else if (c == ']')
                            {
                                if (alreadySeenComma)
                                {
                                    throw new JsonChunkerException("Comma after last element of array",
                                        c, ii);
                                }
                                return ii + 1;
                            }
                            else
                            {
                                if (elementCount > 0 && !alreadySeenComma)
                                {
                                    throw new JsonChunkerException("No comma between array members",
                                        c, ii);
                                }
                                // the next chunk boundary comes before the next item
                                // (assuming this array is the top-level iterable)
                                if (recDepth1 && chunkJustEnded)
                                {
                                    lastBoundary = ii + unicodeExtraBytes;
                                    chunkJustEnded = false;
                                }
                                // a new array member of some sort
                                ii = ParseSomething(inp, recursion_depth, ii);
                                elementCount++;
                                alreadySeenComma = false;
                                // chunks should end immediately after an element in the array
                                if (recDepth1 && ii >= lastBoundary + chunkSize)
                                {
                                    chunkJustEnded = true;
                                    isArray = true;
                                    chunks.Add(new Chunk(lastBoundary, ii + unicodeExtraBytes));
                                }
                            }
                        }
                        throw new JsonChunkerException("Unterminated array", c, ii);
                    case '{': // OBJECT
                        recursion_depth++;
                        alreadySeenComma = false;
                        recDepth1 = recursion_depth == 1;
                        c = inp[++ii];
                        if (recursion_depth == MAX_RECURSION_DEPTH)
                            throw new JsonChunkerException($"Maximum recursion depth ({MAX_RECURSION_DEPTH}) reached",
                                c, ii);
                        elementCount = 0;
                        while (ii < fhand.Length)
                        {
                            ii = ConsumeWhiteSpace(inp, ii);
                            c = inp[ii];
                            if (c == ',')
                            {
                                if (alreadySeenComma)
                                {
                                    throw new JsonChunkerException($"Two consecutive commas after key-value pair {elementCount - 1} of object",
                                        c, ii);
                                }
                                alreadySeenComma = true;
                                if (elementCount == 0)
                                {
                                    throw new JsonChunkerException("Comma before first value in object", c, ii);
                                }
                                ii++;
                                continue;
                            }
                            else if (c == '}')
                            {
                                if (alreadySeenComma)
                                {
                                    throw new JsonChunkerException("Comma after last key-value pair of object",
                                        c, ii);
                                }
                                return ii + 1;
                            }
                            else if (c == '"')
                            {
                                if (elementCount > 0 && !alreadySeenComma)
                                {
                                    throw new JsonChunkerException($"No comma after key-value pair {elementCount - 1} in object",
                                        c, ii);
                                }
                                // the next chunk boundary comes before the key
                                // (assuming this is the top-level iterable)
                                if (recDepth1 && chunkJustEnded)
                                {
                                    lastBoundary = ii + unicodeExtraBytes;
                                    chunkJustEnded = false;
                                }
                                // a new key-value pair
                                if (inp[ii] != '"')
                                    throw new JsonChunkerException("Object keys must be string", c, ii);
                                ii = ParseSomething(inp, recursion_depth, ii);
                                if (inp[ii] != ':')
                                {
                                    // avoid call overhead in most likely case where colon comes
                                    // immediately after key
                                    ii = ConsumeWhiteSpace(inp, ii);
                                }
                                if (inp[ii] != ':')
                                {
                                    throw new JsonChunkerException($"No ':' between key {elementCount} and value {elementCount} of object",
                                        c, ii);
                                }
                                ii = ConsumeWhiteSpace(inp, ii + 1);
                                ii = ParseSomething(inp, recursion_depth, ii);
                                elementCount++;
                                alreadySeenComma = false;
                                // chunks should end just after the value in a key-value pair
                                if (recDepth1 && ii >= lastBoundary + chunkSize)
                                {
                                    chunkJustEnded = true;
                                    isObject = true;
                                    chunks.Add(new Chunk(lastBoundary, ii + unicodeExtraBytes));
                                }
                            }
                            else throw new JsonChunkerException($"Key in object (would be key {elementCount}) must be string",
                                c, ii);
                        }
                        throw new JsonChunkerException("Unterminated object", c, ii);
                    case 'n': // null
                        if (inp.Substring(ii + 1, 3) == "ull")
                        {
                            return ii + 4;
                        }
                        throw new JsonChunkerException("Expected literal starting with 'n' to be 'null'", inp[ii + 1], ii + 1);
                    case 't': // true
                        if (inp.Substring(ii + 1, 3) == "rue")
                        {
                            return ii + 4;
                        }
                        throw new JsonChunkerException("Expected literal starting with 't' to be 'true'", inp[ii + 1], ii + 1);
                    case 'f': // false
                        if (inp.Substring(ii + 1, 4) == "alse")
                        {
                            return ii + 5;
                        }
                        throw new JsonChunkerException("Expected literal starting with 'f' to be 'false'", inp[ii + 1], ii + 1);
                }
                throw new JsonChunkerException("Badly located character", c, ii);
            }
            return ii;
        }

        public void Parse()
        {
            if (finished) return;
            isObject = false;
            isArray = false;
            unicodeExtraBytes = 0;
            fhand.Seek(0, SeekOrigin.Begin);
            string inp = new StreamReader(fhand).ReadToEnd();
            int length = inp.Length;
            if (length == 0)
            {
                throw new JsonChunkerException("no input");
            }
            int ii = ConsumeWhiteSpace(inp, 0);
            if (ii >= length)
            {
                throw new JsonChunkerException("Json string is only whitespace");
            }
            try
            {
                ii = ParseSomething(inp, 0, ii);
                ii = ConsumeWhiteSpace(inp, ii);
                if (ii < length)
                {
                    char last_c = inp[length - 1];
                    throw new JsonChunkerException($"At end of valid JSON document, got {last_c} instead of EOF", last_c, length - 1);
                }
                // add one more chunk after the last chunk that was added normally
                long startOfLastChunk = 0;
                if (chunks.Count > 0)
                {
                    startOfLastChunk = chunks[chunks.Count - 1].end - unicodeExtraBytes;
                    // find the beginning of the first element after the end of
                    // the current last chunk; the last chunk starts there
                    for (int start = (int)startOfLastChunk; start < inp.Length; start++)
                    {
                        char c = inp[start];
                        if (c == '[' || c == '{' // start of iterable
                            || c == '"' // string
                            || c == 't' || c == 'f' || c == 'n' // true/false/null
                            || c == '-' || ('0' <= c && c <= '9')) // number
                        {
                            startOfLastChunk = start;
                            break;
                        }
                    }
                }
                chunks.Add(new Chunk(startOfLastChunk + unicodeExtraBytes, length + unicodeExtraBytes));
                finished = true;
                return;
            }
            catch (Exception e)
            {
                if (e is IndexOutOfRangeException)
                {
                    char last_c = inp[length - 1];
                    throw new JsonChunkerException("Unexpected end of JSON", last_c, length - 1);
                }
                throw;
            }
        }

        /// <summary>
        /// Parse a JSON file and make a list of the start and end characters for
        /// each chunk in the document.
        /// </summary>
        /// <returns>the start positions of the top-level elements</returns>
        /// <exception cref="JsonChunkerException"></exception>
        public override void AddAllChunks()
        {
            if (finished) return;
            if (WarnTooManyChunks(chunkSize)) return;
            try
            {
                Parse();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error while parsing JSON. Defaulting to same-size chunks.\r\nError message:\r\n:{e}", "Error while parsing JSON",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                // make all same-size chunks
                chunks = new List<Chunk>();
                for (long chunkBegin = 0; chunkBegin < fhand.Length; chunkBegin += chunkSize)
                {
                    long chunkEnd = chunkBegin + chunkSize;
                    if (chunkEnd > fhand.Length) chunkEnd = fhand.Length;
                    Chunk chunk = new Chunk(chunkBegin, chunkEnd);
                    chunks.Add(chunk);
                }
            }
            if (Main.settings.previewLength > 0)
            {
                foreach (Chunk chunk in chunks)
                {
                    AddPreview(chunk, Main.settings.previewLength);
                }
            }
        }

        /// <summary>
        /// Get all text in the chunkNum^th chunk, and set chunkSelected to chunkNum
        /// </summary>
        /// <param name="chunkNum"></param>
        /// <returns></returns>
        public override string ReadChunk(int chunkNum)
        {
            chunkSelected = chunkNum;
            if (chunkNum > chunks.Count - 1)
            {
                throw new ArgumentException($"This JSON file has only {chunks.Count} chunks, can't go to {chunkNum + 1}^th chunk");
            }
            Chunk chunk = chunks[chunkNum];
            string openBracket = "";
            string closeBracket = "";
            if (isObject)
            {
                if (chunkNum != 0) // the first chunk naturally has the open bracket
                    openBracket = "{";
                if (chunkNum != chunks.Count - 1)
                    closeBracket = "}";
            }
            else if (isArray)
            {
                if (chunkNum != 0) // the first chunk naturally has the open bracket
                    openBracket = "[";
                if (chunkNum != chunks.Count - 1)
                    closeBracket = "]";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(openBracket);
            sb.Append(chunk.Read(fhand));
            sb.Append(closeBracket);
            string result = sb.ToString();
            return result;
        }

        /// <summary>
        /// clear all chunks and set position to 0, but change nothing else
        /// </summary>
        public override void Reset()
        {
            chunks.Clear();
            finished = false;
            chunkSelected = -1;
        }

        public override void Reset(string delimiter, int minChunk, int maxChunk)
        {
            chunkSize = (minChunk + maxChunk) / 2;
            Reset();
        }
    }
    #endregion
}