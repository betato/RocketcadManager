using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketcadManager
{
    abstract class CadComponent
    {
        public FileInfo ComponentFileInfo { get; set; }
        public bool HasInfo { get; protected set; }
        public bool LoadingError { get; protected set; }

        public CadComponent(FileInfo fileInfo)
        {
            ComponentFileInfo = fileInfo;
        }

        public TreeNode GetNode()
        {
            TreeNode thisNode = new TreeNode();
            thisNode.Tag = this;
            thisNode.Text = ComponentFileInfo.Name;
            if (HasInfo)
            {
                thisNode.ImageKey = "File";
                thisNode.SelectedImageKey = "File";
            }
            else
            {
                thisNode.ImageKey = "ErrorFile";
                thisNode.SelectedImageKey = "ErrorFile";
            }
            return thisNode;
        }

        public abstract void Save();
    }
}
