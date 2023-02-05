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

namespace Kbg.NppPluginNET
{
    class Main
    {
        #region " Fields "
        internal const string PluginName = "H&ugeFiles";
        static string iniFilePath = null;
        static Chunker chunker = null;
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
            PluginBase.SetCommand(2, "---", null);
            PluginBase.SetCommand(3, "&First chunk", FirstChunk, 
                new ShortcutKey(false, true, false, Keys.Up));
            PluginBase.SetCommand(4, "&Previous chunk", PreviousChunk,
                new ShortcutKey(false, true, false, Keys.Left));
            PluginBase.SetCommand(5, "&Next chunk", NextChunk,
                new ShortcutKey(false, true, false, Keys.Right));
            PluginBase.SetCommand(6, "&Last chunk", LastChunk,
                new ShortcutKey(false, true, false, Keys.Down));
            PluginBase.SetCommand(7, "---", null);
            PluginBase.SetCommand(8, "&Open chunk form", OpenChunkForm,
                new ShortcutKey(false, true, true, Keys.H));
            idChunkForm = 8;
            PluginBase.SetCommand(9, "Searc&h for text in file", OpenFindReplaceForm);
            idFindReplaceForm = 9;
            PluginBase.SetCommand(10, "A&bout", OpenAboutForm);
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
            if (chunker != null)
                chunker.Dispose();
            if (chunkForm != null)
                chunkForm.Dispose();
            if (findReplaceForm != null)
                findReplaceForm.Dispose();
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
        }

        #endregion

        #region " Menu functions "

        static void OpenSettings()
        {
            settings.ShowDialog();
            if (!settings.changed)
                return;
            if (chunker != null)
            {
                chunker.Reset(Main.settings.delimiter, Main.settings.minChunk, Main.settings.maxChunk);
                chunker.buffName = "";
                if (chunkForm != null)
                    chunkForm.ChunkTreePopulate();
            }
        }

        static void ChooseFile()
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
                if (chunker == null)
                    chunker = new Chunker(fname, Main.settings.delimiter, Main.settings.minChunk, Main.settings.maxChunk);
                else
                    chunker.ChooseNewFile(fname);
                if (chunkForm != null)
                {
                    chunkForm.ChunkTreePopulate();
                }
                if (findReplaceForm != null)
                {
                    findReplaceForm.Hide();
                    findReplaceForm.Dispose();
                    findReplaceForm = new FindReplaceForm(chunker);
                    findReplaceForm.Show();
                }
            }
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
            findReplaceForm = new FindReplaceForm(chunker);
            findReplaceForm.Show();
            findReplaceForm.PatternBox.Focus();
        }

        static void OpenAboutForm()
        {
            new AboutForm().ShowDialog();
        }

        static void OpenChunkForm()
        {
            //    // Dockable Dialog Demo
            //    // 
            //    // This demonstration shows you how to do a dockable dialog.
            //    // You can create your own non dockable dialog - in this case you don't nedd this demonstration.
            if (WhineIfChunkerNull()) return;
            
            if (chunkForm == null)
            {
                chunkForm = new ChunkForm(chunker);

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
