﻿using RocketcadManagerLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketcadManager
{
    public partial class MainForm : Form
    {
        private int showWindowMsg;
        public MainForm(int showWindowMsg)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            this.showWindowMsg = showWindowMsg;
            InitializeComponent();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            string logFile = LogWriter.Write(LogType.ManagerCrash, ex.StackTrace);
            MessageBox.Show(ex.Message + (e.IsTerminating ? " The program will now close." : "") +
                "\n\nStack trace written to: " + logFile, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private Config config;
        private ImageList iconList = new ImageList();

        private Dictionary<string, Part> parts = new Dictionary<string, Part>();
        private Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
        private List<Folder>cadFolders = new List<Folder>();

        private CadComponent selectedComponent = null;
        private Folder selectedFolder = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            Config.Open(out config);
            
            iconList.ColorDepth = ColorDepth.Depth32Bit;
            iconList.Images.Add("File", Icons.File);
            iconList.Images.Add("Folder", Icons.Folder);
            iconList.Images.Add("WarningFile", Icons.WarningFile);
            iconList.Images.Add("WarningFolder", Icons.WarningFolder);
            iconList.Images.Add("ErrorFile", Icons.ErrorFile);
            iconList.Images.Add("ErrorFolder", Icons.ErrorFolder);
            iconList.Images.Add("WarningErrorFile", Icons.WarningErrorFile);
            iconList.Images.Add("WarningErrorFolder", Icons.WarningErrorFolder);
            iconList.Images.Add("QuestionFile", Icons.QuestionFile);
            iconList.Images.Add("QuestionFolder", Icons.QuestionFolder);
            iconList.Images.Add("WarningQuestionFile", Icons.WarningQuestionFile);
            iconList.Images.Add("WarningQuestionFolder", Icons.WarningQuestionFolder);
            fileView.ImageList = iconList;

            LoadFiles();
        }

        protected override void WndProc(ref Message m)
        {
            if(m.Msg == showWindowMsg)
            {
                if (WindowState == FormWindowState.Minimized)
                    WindowState = FormWindowState.Normal;
                TopMost = true;
                TopMost = false;
            }
            base.WndProc(ref m);
        }

        private void LoadFiles()
        {
            toolStripStatusLabel1.Text = "Loading Files";
            EnableBoxes(false);

            parts.Clear();
            assemblies.Clear();
            cadFolders.Clear();
            fileView.Nodes.Clear();

            foreach (string cadDir in config.CadDirectories)
            {
                Folder cadFolder = new Folder(new DirectoryInfo(cadDir), null);
                cadFolders.Add(cadFolder);
                WalkDirectoryTree(cadFolder);
            }
            
            foreach (Assembly assembly in assemblies.Values)
            {
                assembly.AddDependencies(parts, assemblies);
            }

            // Add files and folders to fileView
            foreach (Folder cadFolder in cadFolders)
            {
                fileView.Nodes.Add(cadFolder.DirectoryTree());
            }
            // Expand nodes
            config.LoadTreeViewExpansion(fileView.Nodes);

            toolStripStatusLabel1.Text = "Ready";
        }

        private void WalkDirectoryTree(Folder rootFolder)
        {
            DirectoryInfo rootDir = rootFolder.Path;
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;

            try
            {
                files = rootDir.GetFiles("*.*");
                subDirs = rootDir.GetDirectories();
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (FileInfo fi in files)
                {
                    // Ignore hidden files
                    if (fi.Attributes.HasFlag(FileAttributes.Hidden))
                        continue;

                    string extension = fi.Extension.ToLower();
                    string name = fi.Name.ToLower();
                    if (extension == ".sldprt")
                    {
                        Part newPart = new Part(fi, rootFolder);
                        parts.Add(fi.FullName, newPart);
                        rootFolder.parts.Add(newPart);
                    }
                    else if (extension == ".sldasm")
                    {
                        Assembly newAssembly = new Assembly(fi, rootFolder);
                        assemblies.Add(fi.FullName, newAssembly);
                        rootFolder.assemblies.Add(newAssembly);
                    }
                }
                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    if (dirInfo.Name == ".partstatus")
                        continue;
                    Folder subFolder = new Folder(dirInfo, rootFolder);
                    WalkDirectoryTree(subFolder);
                    // Only add non-empty folders
                    if (!subFolder.IsEmpty())
                    {
                        rootFolder.subFolders.Add(subFolder);
                    }
                }
            }
        }

        private void EnableFileOpen(bool enabled)
        {
            toolStripOpen.Enabled = enabled;
            openFileToolStripMenuItem.Visible = enabled;
        }

        private void DisplayPart(Part part)
        {
            foreach (Tuple<Assembly, int> dependant in part.dependants)
            {
                string[] entry = { dependant.Item1.ComponentFileInfo.Name, dependant.Item2.ToString() };
                listViewDependants.Items.Add(new ListViewItem(entry));
            }
            ResizeListView(listViewDependants);
        }

        private void DisplayAssembly(Assembly assembly)
        {
            foreach (Tuple<Assembly, int> dependant in assembly.dependants)
            {
                string[] entry = { dependant.Item1.ComponentFileInfo.Name, dependant.Item2.ToString() };
                listViewDependants.Items.Add(new ListViewItem(entry));
            }
            foreach (Tuple<Assembly, int> subAssembly in assembly.subAssemblies)
            {
                string[] entry = { subAssembly.Item1.ComponentFileInfo.Name, subAssembly.Item2.ToString() };
                listViewDependancies.Items.Add(new ListViewItem(entry));
            }
            foreach (Tuple<Part, int> part in assembly.parts)
            {
                string[] entry = { part.Item1.ComponentFileInfo.Name, part.Item2.ToString() };
                listViewDependancies.Items.Add(new ListViewItem(entry));
            }
            ResizeListView(listViewDependants);
            ResizeListView(listViewDependancies);
        }

        private void ResizeListView(ListView listView)
        {
            if (listView.Items.Count > 0)
            {
                foreach (ColumnHeader column in listView.Columns)
                {
                    column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                    column.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                }
            }
        }

        private void SelectFolder(Folder folder)
        {
            selectedFolder = folder;

            EnableBoxes(false);
            EnableFileOpen(false);
        }

        private void SelectComponent(CadComponent component)
        {
            // TODO: Re-enable this when file opening works
            //EnableFileOpen(true);

            selectedComponent = component;
            
            if (component.HasInfo)
            {
                ClearBoxes();

                // Display common part and assembly info
                textBoxNotes.Text = selectedComponent.CadInfo.Notes;
                textBoxDescription.Text = selectedComponent.CadInfo.Description;

                numericStock.Value = selectedComponent.CadInfo.Stock;

                numericRequiredAssembly.Value = selectedComponent.UsageCount();
                numericRequiredAdditional.Value = selectedComponent.CadInfo.AdditionalRequired;
                numericRequiredTotal.Value = numericRequiredAssembly.Value + numericRequiredAdditional.Value;

                OpenThumbnail(component.ComponentFileInfo);

                // Display part-specific and assembly specific info
                if (component.GetType() == typeof(Part))
                    DisplayPart((Part)component);
                else if (component.GetType() == typeof(Assembly))
                    DisplayAssembly((Assembly)component);

                EnableBoxes(true);
            }
            else
            {
                EnableBoxes(false);
            }
        }

        private void OpenThumbnail(FileInfo cadFile)
        {
            // TODO: Add error handling for inaccesible files
            // TODO: Open images async
            if (CadInfoLoader.OpenImage(cadFile, out Image thumb))
                pictureBox1.Image = thumb;
        }

        private void ClearBoxes()
        {
            textBoxNotes.Clear();
            textBoxDescription.Clear();

            numericStock.Value = 0;
            numericRequiredAssembly.Value = 0;
            numericRequiredAdditional.Value = 0;
            numericRequiredTotal.Value = 0;

            pictureBox1.Image = null;
            listViewDependants.Items.Clear();
            listViewDependancies.Items.Clear();
        }

        private void EnableBoxes(bool enabled)
        {
            LabelDescription.Enabled = enabled;
            textBoxNotes.Enabled = enabled;
            textBoxDescription.Enabled = enabled;

            groupBoxStock.Enabled = enabled;
            groupBoxRequired.Enabled = enabled;

            listViewDependants.Enabled = enabled;
            listViewDependancies.Enabled = enabled;

            if (enabled)
            {
                //pictureBox1.BackColor = SystemColors.Window;
            }
            else
            {
                ClearBoxes();
                //pictureBox1.BackColor = SystemColors.Control;
            }
        }

        private void fileView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            object component = e.Node.Tag;

            // Save previous selection
            SaveSelected();

            // Deselect old selection
            selectedComponent = null;
            selectedFolder = null;

            if (component.GetType() == typeof(Folder))
                SelectFolder((Folder)component);
            else if (component is CadComponent)
                SelectComponent((CadComponent)component);
        }

        private void SaveSelected()
        {
            // TODO: Add text checking
            if (selectedComponent != null && selectedComponent.HasInfo)
            {
                toolStripStatusLabel1.Text = "Saving";
                selectedComponent.CadInfo.Notes = textBoxNotes.Text;
                selectedComponent.CadInfo.Stock = Convert.ToInt32(numericStock.Value);
                selectedComponent.CadInfo.AdditionalRequired = Convert.ToInt32(numericRequiredAdditional.Value);
                selectedComponent.Save();
                Console.WriteLine("Saved");
                toolStripStatusLabel1.Text = "Ready";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSelected();
            config.SaveTreeViewExpansion(fileView.Nodes);

            Config.Save(config);
        }

        private void toolStripSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settingsWindow = new SettingsForm(config);
            if (settingsWindow.ShowDialog(this) == DialogResult.OK)
            {
                config.SaveTreeViewExpansion(fileView.Nodes);
                Config.Save(config);
                LoadFiles();
            }
        }

        private void toolStripRefresh_Click(object sender, EventArgs e)
        {
            config.SaveTreeViewExpansion(fileView.Nodes);
            LoadFiles();
        }


        bool mouseDown = false;
        Point mouseDownPos;

        private void fileView_MouseDown(object sender, MouseEventArgs e)
        {
            fileView.SelectedNode = fileView.GetNodeAt(e.Location);

            if (e.Button == MouseButtons.Right)
            {
                mouseDown = true;
                mouseDownPos = Cursor.Position;
            }
        }

        private void fileView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && 
                mouseDown && fileView.SelectedNode != null &&
                fileView.SelectedNode.Bounds.Contains(e.Location))
            {

                
                contextMenuStrip1.Show(mouseDownPos);
            }
            mouseDown = false;
        }

        private void OpenFile()
        {
            // TODO: Open the file through the solidworks plugin

            /*
            ProcessStartInfo solidworksProcess = new ProcessStartInfo();
            solidworksProcess.FileName = @"C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\SLDWORKS.exe";
            solidworksProcess.WindowStyle = ProcessWindowStyle.Minimized;
            
            if (selectedComponent != null)
            {
                solidworksProcess.Arguments = selectedComponent.ComponentFileInfo.FullName;
                Process.Start(solidworksProcess);
            }
            */
        }

        private void OpenContainingFolder()
        {
            if (selectedComponent != null)
            {
                System.Diagnostics.Process.Start(selectedComponent.ComponentFileInfo.DirectoryName);
            }
            else if (selectedFolder != null)
            {
                System.Diagnostics.Process.Start(selectedFolder.Path.Parent.FullName);
            }
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void openContainingFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenContainingFolder();
        }

        private void toolStripOpen_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void toolStripOpenFolder_Click(object sender, EventArgs e)
        {
            OpenContainingFolder();
        }

        private void numericRequiredAdditional_ValueChanged(object sender, EventArgs e)
        {
            numericRequiredTotal.Value = numericRequiredAssembly.Value + numericRequiredAdditional.Value;
        }

        private void toolStripWarnings_Click(object sender, EventArgs e)
        {
            List<CadComponent> cadComponents = new List<CadComponent>();
            cadComponents.AddRange(parts.Values.ToList());
            cadComponents.AddRange(assemblies.Values.ToList());
            WarningsListForm warningsList = new WarningsListForm(cadComponents, iconList);
            warningsList.ShowDialog(this);
        }
    }
}
