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

        private CadComponent selectedComponent = null;
        private Folder selectedFolder = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigLoader.Open(out config);
            
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.Images.Add("File", Icons.File);
            imageList.Images.Add("Folder", Icons.Folder);
            imageList.Images.Add("WarningFile", Icons.WarningFile);
            imageList.Images.Add("WarningFolder", Icons.WarningFolder);
            imageList.Images.Add("ErrorFile", Icons.ErrorFile);
            imageList.Images.Add("ErrorFolder", Icons.ErrorFolder);
            imageList.Images.Add("WarningErrorFile", Icons.WarningErrorFile);
            imageList.Images.Add("WarningErrorFolder", Icons.WarningErrorFolder);
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
                Folder cadFolder = new Folder(new DirectoryInfo(cadDir), null);
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
                listView1.Items.Add(new ListViewItem(entry));
            }
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

        private void textBoxStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void SaveSelected()
        {
            // TODO: Add text checking
            if (selectedComponent != null)
            {
                toolStripStatusLabel1.Text = "Saving";
                selectedComponent.CadInfo.Notes = textBoxNotes.Text;
                selectedComponent.CadInfo.Description = textBoxDescription.Text;
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
            
            if (selectedComponent != null)
            {
                solidworksProcess.Arguments = selectedComponent.ComponentFileInfo.FullName;
                Process.Start(solidworksProcess);
            }
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
    }
}
