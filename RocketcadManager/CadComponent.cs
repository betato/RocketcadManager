using RocketcadManagerLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketcadManager
{
    abstract class CadComponent
    {
        public FileInfo ComponentFileInfo { get; set; }
        public Folder ParentFolder { get; protected set; }
        public bool HasInfo { get; protected set; }
        public bool LoadingError { get; protected set; }
        public CadInfo CadInfo { get; protected set; }

        public List<Tuple<Assembly, int>> dependants = new List<Tuple<Assembly, int>>();

        public CadComponent(FileInfo fileInfo, Folder parentFolder)
        {
            ComponentFileInfo = fileInfo;
            ParentFolder = parentFolder;
        }

        public TreeNode GetNode()
        {
            TreeNode thisNode = new TreeNode();
            thisNode.Tag = this;
            thisNode.Text = ComponentFileInfo.Name;
            if (!NameOk())
            {
                if (!HasInfo)
                {
                    thisNode.ImageKey = "WarningErrorFile";
                    thisNode.SelectedImageKey = "WarningErrorFile";
                }
                else
                {
                    thisNode.ImageKey = "WarningFile";
                    thisNode.SelectedImageKey = "WarningFile";
                }
            }
            else if (!HasInfo)
            {
                thisNode.ImageKey = "ErrorFile";
                thisNode.SelectedImageKey = "ErrorFile";
            }
            else
            {
                thisNode.ImageKey = "File";
                thisNode.SelectedImageKey = "File";
            }
            return thisNode;
        }

        public virtual bool NameOk()
        {
            return Regex.IsMatch(ComponentFileInfo.Name, @"^([0-9]{2}-)+[0-9]{2}(\s.*)*\.(?i)(SLDASM|SLDPRT)(?-i)$");
        }

        public TreeNode DependancyTree()
        {
            TreeNode thisNode = GetNode();
            foreach (Tuple<Assembly, int> dependant in dependants)
            {
                // TODO: do something with the quantity
                thisNode.Nodes.Add(dependant.Item1.DependancyTree());
            }
            return thisNode;
        }

        public abstract void Save();
    }
}
