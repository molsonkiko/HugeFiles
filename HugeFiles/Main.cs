// NPP plugin platform for .Net v0.91.57 by Kasper B. Graversen etc.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Kbg.NppPluginNET.PluginInfrastructure;
using static Kbg.NppPluginNET.PluginInfrastructure.Win32;
using HugeFiles.Forms;
using HugeFiles.HugeFiles;
using HugeFiles.Utils;
using NppPluginNET.Forms;
using HugeFiles.Tests;
using System.Linq;

namespace Kbg.NppPluginNET
{
    class Main
    {
        #region " Fields "
        internal const string PluginName = "H&ugeFiles";
        static string iniFilePath = null;
        public static BaseChunker chunker = null;
        static FindReplaceForm findReplaceForm = null;
        internal static ChunkForm chunkForm = null;

        public static Settings settings = new Settings();
        static internal int idChunkForm = -1;
        static internal int idFindReplaceForm = -1;

        // toolbar icons
        //static Bitmap tbBmp = Resources.star;
        //static Bitmap tbBmp_tbTab = Resources.star_bmp;
        //static Icon tbIco = Resources.star_black_ico;
        //static Icon tbIcoDM = Resources.star_white_ico;
        //static Icon tbIcon = null;
        #endregion

        #region " Startup/CleanUp "

        static internal void CommandMenuInit()
        {
            // Initialization of your plugin commands
            // You should fill your plugins commands here
 
            //
            // Firstly we get the parameters from your plugin config file (if any)
            //

            // get path of plugin configuration
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();

            // if config path doesn't exist, we create it
            if (!Directory.Exists(iniFilePath))
            {
                Directory.CreateDirectory(iniFilePath);
            }

            // make your plugin config file full file path name
            iniFilePath = Path.Combine(iniFilePath, PluginName + ".ini");

            // with function :
            // SetCommand(int index,                            // zero based number to indicate the order of command
            //            string commandName,                   // the command name that you want to see in plugin menu
            //            NppFuncItemDelegate functionPointer,  // the symbol of function (function pointer) associated with this command. The body should be defined below. See Step 4.
            //            ShortcutKey *shortcut,                // optional. Define a shortcut to trigger this command
            //            bool check0nInit                      // optional. Make this menu item be checked visually
            //            );
            PluginBase.SetCommand(0, "&Choose file", ChooseFile,
                new ShortcutKey(false, true, true, Keys.F));
            PluginBase.SetCommand(1, "&Settings", OpenSettings,
                new ShortcutKey(false, true, true, Keys.T));
            PluginBase.SetCommand(2, "E&xit file", ExitFile);
            PluginBase.SetCommand(3, "---", null);
            PluginBase.SetCommand(4, "&First chunk", FirstChunk, 
                new ShortcutKey(false, true, false, Keys.Up));
            PluginBase.SetCommand(5, "&Previous chunk", PreviousChunk,
                new ShortcutKey(false, true, false, Keys.Left));
            PluginBase.SetCommand(6, "&Next chunk", NextChunk,
                new ShortcutKey(false, true, false, Keys.Right));
            PluginBase.SetCommand(7, "&Last chunk", LastChunk,
                new ShortcutKey(false, true, false, Keys.Down));
            PluginBase.SetCommand(8, "---", null);
            PluginBase.SetCommand(9, "&Open chunk form", OpenChunkForm,
                new ShortcutKey(false, true, true, Keys.H));
            idChunkForm = 9;
            PluginBase.SetCommand(10, "Searc&h for text in file", OpenFindReplaceForm);
            idFindReplaceForm = 10;
            PluginBase.SetCommand(11, "Chunks to folde&r", ChunksToFolder);
            PluginBase.SetCommand(12, "---", null);
            PluginBase.SetCommand(13, "A&bout", OpenAboutForm);
            PluginBase.SetCommand(14, "Run &tests", TestRunner.RunAll);

            // fix most common validation problem with settings
            if (settings.minChunk > settings.maxChunk)
            {
                settings.minChunk = settings.maxChunk;
            }
        }

        static internal void SetToolBarIcon()
        {
            // create struct
            toolbarIcons tbIcons = new toolbarIcons();
			
            // add bmp icon
            //tbIcons.hToolbarBmp = tbBmp.GetHbitmap();
            //tbIcons.hToolbarIcon = tbIco.Handle;            // icon with black lines
            //tbIcons.hToolbarIconDarkMode = tbIcoDM.Handle;  // icon with light grey lines

            // convert to c++ pointer
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);

            // call Notepad++ api
            //Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON_FORDARKMODE, PluginBase._funcItems.Items[idChunkForm]._cmdID, pTbIcons);

            // release pointer
            Marshal.FreeHGlobal(pTbIcons);
        }

        static internal void PluginCleanUp()
        {
            //Win32.WritePrivateProfileString(sectionName, keyName, doCloseTag ? "1" : "0", iniFilePath);
            chunker?.Dispose();
            chunkForm?.Dispose();
            findReplaceForm?.Dispose();
        }

        public static void OnNotification(ScNotification notification)
        {
            uint code = notification.Header.Code;
            // This method is invoked whenever something is happening in notepad++
            // use eg. as
            // if (code == (uint)NppMsg.NPPN_xxx)
            // { ... }
            // or
            //
            // if (code == (uint)SciMsg.SCNxxx)
            // { ... }
            //// when closing a file
            if (code == (uint)NppMsg.NPPN_FILEBEFORECLOSE)
            {
                // make sure to always open a new buffer for chunks
                // if the existing chunk buffer closes
                if (chunker != null && Npp.GetCurrentPath() == chunker.buffName)
                {
                    chunker.buffName = "";
                }
            }
            if (code == (uint)NppMsg.NPPN_BUFFERACTIVATED)
            {
                // make sure the editor tracks whatever view the user is actually currently working with
                Npp.editor = new ScintillaGateway(PluginBase.GetCurrentScintilla());
            }
        }

        #endregion

        #region " Menu functions "

        static void OpenSettings()
        {
            settings.ShowDialog();
            if (!settings.changed)
                return;
            if (settings.minChunk > settings.maxChunk)
            {
                MessageBox.Show("minChunk can't be greater than maxChunk. Setting minChunk equal to maxChunk.",
                    "Settings validation error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                settings.minChunk = settings.maxChunk;
            }
            if (chunker != null)
            {
                if (chunker is JsonChunker oldChunker)
                {
                    // json chunkers don't handle resetting well for some reason
                    string fname = oldChunker.fname;
                    oldChunker.Dispose();
                    chunker = new JsonChunker(fname, settings.minChunk, settings.maxChunk);
                }
                else
                {
                    chunker.Reset(settings.delimiter, settings.minChunk, settings.maxChunk);
                    chunker.buffName = "";
                }
                chunkForm?.ChunkTreePopulate();
            }
        }

        public static void ChooseFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All files|*.*|Text files|*.txt|CSV files|*.csv|JSON files|*.json";
            ofd.InitialDirectory = @"C:\";
            ofd.Title = "Open file for chunking with HugeFiles";
            ofd.CheckFileExists = true;
            string fname;
            // open a new file
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fname = ofd.FileName;
                bool isJson = (settings.parseJsonAsJson && fname.EndsWith(".json"))
                    || settings.parseNonJsonAsJson;
                if (!File.Exists(fname))
                {
                    return;
                }
                // if the file is of a type that the current chunker isn't equipped to handle,
                // create a new chunker of the appropriate type.
                // also create a new chunker if the chunker was a JsonChunker, because those
                // chunkers don't handle resetting well.
                if (chunker == null
                    || (chunker is JsonChunker && !isJson)
                    || (chunker is TextChunker && isJson))
                {
                    chunker?.Dispose();
                    if (isJson)
                        chunker = new JsonChunker(fname, settings.minChunk, settings.maxChunk);
                    else chunker = new TextChunker(fname, settings.delimiter, settings.minChunk, settings.maxChunk);
                }
                else
                    // if the new file is a text file and the current chunker is a text chunker,
                    // just switch the chunker to the new file
                    chunker.ChooseNewFile(fname);
                chunkForm?.ChunkTreePopulate();
                if (findReplaceForm != null)
                {
                    findReplaceForm.Hide();
                    findReplaceForm.Dispose();
                    findReplaceForm = new FindReplaceForm();
                    findReplaceForm.Show();
                }
            }
        }

        static void ExitFile()
        {
            if (chunkForm != null)
            {
                chunkForm.Hide();
                Win32.SendMessage(PluginBase.nppData._nppHandle,
                    (uint)(NppMsg.NPPM_DMMHIDE),
                    0, chunkForm.Handle); // hide the docking form
                chunkForm.Dispose();
                chunkForm = null;
            }
            chunker?.Dispose();
            chunker = null;
            findReplaceForm?.Hide();
            findReplaceForm?.Dispose();
            findReplaceForm = null;
        }

        static void FirstChunk()
        {
            if (WhineIfChunkerNull()) return;
            ChunkForm.FirstChunk(chunker, chunkForm);
        }

        static void PreviousChunk()
        {
            if (WhineIfChunkerNull()) return;
            ChunkForm.PreviousChunk(chunker, chunkForm);
        }

        static void NextChunk()
        {
            if (WhineIfChunkerNull()) return;
            ChunkForm.NextChunk(chunker, chunkForm);
        }

        static void LastChunk()
        {
            if (WhineIfChunkerNull()) return;
            ChunkForm.LastChunk(chunker, chunkForm);
        }

        /// <summary>
        /// make a message box and return true if no chunker. Else return false.
        /// </summary>
        /// <returns></returns>
        static bool WhineIfChunkerNull()
        {
            if (chunker == null)
            {
                MessageBox.Show("No open file to chunk!", "No file selected for HugeFiles",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            return false;
        }

        static void OpenFindReplaceForm()
        {
            if (WhineIfChunkerNull()) return;
            if (findReplaceForm != null)
            {
                findReplaceForm.Hide();
                findReplaceForm.Dispose();
            }
            findReplaceForm = new FindReplaceForm();
            findReplaceForm.Show();
            findReplaceForm.PatternBox.Focus();
        }

        static void OpenAboutForm()
        {
            new AboutForm().ShowDialog();
        }

        static void ChunksToFolder()
        {
            if (WhineIfChunkerNull()) return;
            chunker.AddAllChunks();
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = Path.GetDirectoryName(chunker.fname);
            string extension = Path.GetExtension(chunker.fname);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                for (int ii = 0; ii < chunker.chunks.Count; ii++)
                {
                    Chunk chunk = chunker.chunks[ii];
                    FileStream stream = null;
                    try
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(chunker.ReadChunk(ii));
                        stream = new FileStream($"{dlg.SelectedPath}/chunk_{chunk.start}{extension}", FileMode.CreateNew);
                        stream.Write(bytes, 0, bytes.Length);

                    }
                    catch
                    {
                        MessageBox.Show($"Failed to create a file for chunk starting at position {chunk.start}. No more chunks will be written to file.",
                            "Failed to write chunk to file",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    finally
                    {
                        stream?.Dispose();
                    }
                }
                MessageBox.Show($"Succeeded in writing chunks to files in folder {dlg.SelectedPath}",
                    "New folder created!",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        static void OpenChunkForm()
        {
            if (WhineIfChunkerNull()) return;
            
            if (chunkForm == null)
            {
                chunkForm = new ChunkForm();

                using (Bitmap newBmp = new Bitmap(16, 16))
                {
                    Graphics g = Graphics.FromImage(newBmp);
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap();
                    colorMap[0].OldColor = Color.Fuchsia;
                    colorMap[0].NewColor = Color.FromKnownColor(KnownColor.ButtonFace);
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);
                    //g.DrawImage(tbBmp_tbTab, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                    //tbIcon = Icon.FromHandle(newBmp.GetHicon());
                }

                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = chunkForm.Handle;
                _nppTbData.pszName = "Open chunk";
                // the dlgDlg should be the index of funcItem where the current function pointer is in
                // this case is 15.. so the initial value of funcItem[15]._cmdID - not the updated internal one !
                _nppTbData.dlgID = idChunkForm;
                // define the default docking behaviour
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                //_nppTbData.hIconTab = (uint)tbIcon.Handle;
                _nppTbData.pszModuleName = PluginName;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
                // Following message will toogle both menu item state and toolbar button
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, PluginBase._funcItems.Items[idChunkForm]._cmdID, 1);
            }
            else
            {
                // toggle existing form between visible and invisible
                if (!chunkForm.Visible)
                {
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, chunkForm.Handle);
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, PluginBase._funcItems.Items[idChunkForm]._cmdID, 1);
                }
                else
                {
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, chunkForm.Handle);
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, PluginBase._funcItems.Items[idChunkForm]._cmdID, 0);
                }
            }
            chunkForm.ChunkTreePopulate();
            chunkForm.Focus();
        }
    #endregion
}
}   
