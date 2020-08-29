using RocketcadManagerLib;
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
        public MainForm()
        {
            InitializeComponent();
        }

        private Config config;

        private Dictionary<string, Part> parts = new Dictionary<string, Part>();
        private Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
        private List<Folder>cadFolders = new List<Folder>();

        private Assembly selectedAssembly = null;
        private Part selectedPart = null;
        private Folder selectedFolder = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigLoader.Open(out config);
            
            ImageList imageList = new ImageList();
            imageList.Images.Add("File", Icons.File);
            imageList.Images.Add("Folder", Icons.Folder);
            imageList.Images.Add("WarningFile", Icons.WarningFile);
            imageList.Images.Add("WarningFolder", Icons.WarningFolder);
            imageList.Images.Add("ErrorFile", Icons.ErrorFile);
            imageList.Images.Add("ErrorFolder", Icons.ErrorFolder);
            fileView.ImageList = imageList;

            LoadFiles();
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
                Folder cadFolder = new Folder(new DirectoryInfo(cadDir));
                cadFolders.Add(cadFolder);
                WalkDirectoryTree(cadFolder);
            }
            foreach (Folder cadFolder in cadFolders)
            {
                fileView.Nodes.Add(cadFolder.DirectoryTree());
            }
            // Expand top-level nodes
            foreach (TreeNode node in fileView.Nodes)
            {
                node.Expand();
            }
            foreach (Assembly assembly in assemblies.Values)
            {
                assembly.AddParts(parts, assemblies);
            }
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
                        Part newPart = new Part(fi);
                        parts.Add(fi.FullName, newPart);
                        rootFolder.parts.Add(newPart);
                    }
                    else if (extension == ".sldasm")
                    {
                        Assembly newAssembly = new Assembly(fi);
                        assemblies.Add(fi.FullName, newAssembly);
                        rootFolder.assemblies.Add(newAssembly);
                    }
                }
                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    if (dirInfo.Name == ".partstatus")
                        continue;
                    Folder subFolder = new Folder(dirInfo);
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
            textBoxNotes.Text = part.partInfo.Notes;
            textBoxStock.Text = part.partInfo.Stock.ToString();

            foreach (Tuple<Assembly, int> dependant in part.dependants)
            {
                string[] entry = { dependant.Item1.ComponentFileInfo.Name, dependant.Item2.ToString() };
                listView1.Items.Add(new ListViewItem(entry));
            }
        }

        private void DisplayAssembly(Assembly assembly)
        {
            textBoxNotes.Text = assembly.assemblyInfo.Notes;
            textBoxStock.Text = assembly.assemblyInfo.Stock.ToString();

            foreach (Tuple<Assembly, int> dependant in assembly.dependants)
            {
                string[] entry = { dependant.Item1.ComponentFileInfo.Name, dependant.Item2.ToString() };
                listView1.Items.Add(new ListViewItem(entry));
            }
            foreach (Tuple<Assembly, int> subAssembly in assembly.subAssemblies)
            {
                string[] entry = { subAssembly.Item1.ComponentFileInfo.Name, subAssembly.Item2.ToString() };
                listView2.Items.Add(new ListViewItem(entry));
            }
            foreach (Tuple<Part, int> part in assembly.parts)
            {
                string[] entry = { part.Item1.ComponentFileInfo.Name, part.Item2.ToString() };
                listView2.Items.Add(new ListViewItem(entry));
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
            EnableFileOpen(true);

            if (component.GetType() == typeof(Part))
                selectedPart = (Part)component;
            else if (component.GetType() == typeof(Assembly))
                selectedAssembly = (Assembly)component;

            if (component.HasInfo)
            {
                ClearBoxes();
                if (selectedPart != null)
                {
                    DisplayPart(selectedPart);
                }
                else if (selectedAssembly != null)
                {
                    DisplayAssembly(selectedAssembly);
                }
                OpenThumbnail(component.ComponentFileInfo);
                EnableBoxes(true);
            }
            else
            {
                EnableBoxes(false);
            }
        }

        private void OpenThumbnail(FileInfo cadFile)
        {
            if (CadInfoLoader.OpenImage(cadFile, out Image thumb))
                pictureBox1.Image = thumb;
        }

        private void ClearBoxes()
        {
            textBoxNotes.Text = "";
            textBoxStock.Text = "";
            textBoxRequired.Text = "";
            pictureBox1.Image = null;
            listView1.Items.Clear();
            listView2.Items.Clear();
        }

        private void EnableBoxes(bool enabled)
        {
            textBoxStock.Enabled = enabled;
            textBoxRequired.Enabled = enabled;
            textBoxNotes.Enabled = enabled;
            textBoxDescription.Enabled = enabled;

            listView1.Enabled = enabled;
            listView2.Enabled = enabled;

            if (enabled == false)
                ClearBoxes();
        }

        private void fileView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            object component = e.Node.Tag;

            // Save previous selection
            SaveSelected();
            // Deselect old selection
            selectedPart = null;
            selectedAssembly = null;
            selectedFolder = null;

            if (component.GetType() == typeof(Folder))
            {
                SelectFolder((Folder)component);
            }
            else if (component is CadComponent)
            {
                SelectComponent((CadComponent)component);
            }
        }

        private void textBoxStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void SaveSelected()
        {
            if (selectedPart == null && selectedAssembly == null)
                return;

            // TODO: Add text checking
            if (selectedPart != null && selectedPart.HasInfo)
            {
                toolStripStatusLabel1.Text = "Saving";
                selectedPart.partInfo.Notes = textBoxNotes.Text;
                selectedPart.partInfo.Stock = int.Parse(textBoxStock.Text);
                selectedPart.Save();
                Console.WriteLine("Saved");
            }
            else if (selectedAssembly != null && selectedAssembly.HasInfo)
            {
                toolStripStatusLabel1.Text = "Saving";
                selectedAssembly.assemblyInfo.Notes = textBoxNotes.Text;
                selectedAssembly.assemblyInfo.Stock = int.Parse(textBoxStock.Text);
                selectedAssembly.Save();
                Console.WriteLine("Saved");
            }
            toolStripStatusLabel1.Text = "Ready";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSelected();
        }

        private void toolStripSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settingsWindow = new SettingsForm(config);
            if(settingsWindow.ShowDialog(this) == DialogResult.OK)
            {
                ConfigLoader.Save(config);
                LoadFiles();
            }            
        }

        private void toolStripRefresh_Click(object sender, EventArgs e)
        {
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
            ProcessStartInfo solidworksProcess = new ProcessStartInfo();
            solidworksProcess.FileName = @"C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\SLDWORKS.exe";
            solidworksProcess.WindowStyle = ProcessWindowStyle.Minimized;
            
            if (selectedPart != null)
            {
                solidworksProcess.Arguments = selectedPart.ComponentFileInfo.FullName;
                Process.Start(solidworksProcess);
            }
            else if (selectedAssembly != null)
            {
                solidworksProcess.Arguments = selectedAssembly.ComponentFileInfo.FullName;
                Process.Start(solidworksProcess);
            }
        }

        private void OpenContainingFolder()
        {
            if (selectedPart != null)
            {
                System.Diagnostics.Process.Start(selectedPart.ComponentFileInfo.DirectoryName);
            }
            else if (selectedPart != null)
            {
                System.Diagnostics.Process.Start(selectedPart.ComponentFileInfo.DirectoryName);
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
    }
}
