using RocketcadManagerLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketcadManager
{
    class Folder
    {
        public DirectoryInfo Path { get; set; }
        public Folder ParentFolder { get; protected set; }
        public List<Folder> subFolders = new List<Folder>();
        public List<Assembly> assemblies = new List<Assembly>();
        public List<Part> parts = new List<Part>();

        public Folder(DirectoryInfo di, Folder parentFolder)
        {
            Path = di;
            ParentFolder = parentFolder;
        }

        public TreeNode DirectoryTree()
        {
            TreeNode thisNode = new TreeNode();
            thisNode.Tag = this;
            thisNode.Text = Path.Name;

            if (NameOk() && LocationOk())
                SetImageKey(thisNode, "Folder");
            else
                SetImageKey(thisNode, "WarningFolder");

            foreach (Folder folder in subFolders)
                thisNode.Nodes.Add(folder.DirectoryTree());

            foreach (Assembly assembly in assemblies)
                thisNode.Nodes.Add(assembly.GetNode());

            foreach (Part part in parts)
                thisNode.Nodes.Add(part.GetNode());

            return thisNode;
        }

        private void SetImageKey(TreeNode node, string imageKey)
        {
            node.ImageKey = imageKey;
            node.SelectedImageKey = imageKey;
        }

        public bool IsEmpty()
        {
            // Contains no parts or assemblies
            return assemblies.Count <= 0 && parts.Count <= 0 && subFolders.Count <= 0;
        }

        public bool NameOk()
        {
            return Regex.IsMatch(Path.Name, ConstantPaths.ValidFolderRegex);
        }

        public bool LocationOk()
        {
            return Regex.Match(Path.Name, ConstantPaths.ParentFolderRegex).Value
                == Regex.Match(Path.Name, ConstantPaths.ChildFolderRegex).Value;
        }
    }
}
