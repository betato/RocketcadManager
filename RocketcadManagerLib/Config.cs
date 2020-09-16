using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketcadManagerLib
{
    public class Config
    {
        private class FolderNode
        {
            public FolderNode sibling;
            public FolderNode child;

            public string Name { get; private set; }
            public bool Expanded { get; private set; }

            public FolderNode(string name, bool expanded)
            {
                Name = name;
                Expanded = expanded;
            }

            public void Clear()
            {
                sibling = null;
                child = null;
            }
        }

        public List<string> CadDirectories { get; set; }
        private FolderNode folderViewExpansion;

        public Config()
        {
            CadDirectories = new List<string>();
        }

        public void SaveTreeViewExpansion(TreeNodeCollection treeNodes)
        {

            folderViewExpansion = SaveTreeViewNodes(treeNodes);
        }

        private FolderNode SaveTreeViewNodes(TreeNodeCollection treeNodes)
        {
            FolderNode firstSibling = null;
            FolderNode currentSibling = null;
            foreach (TreeNode treeNode in treeNodes)
            {
                TreeNodeCollection subNodes = treeNode.Nodes;
                if (subNodes.Count > 0) // Node has children and is expandable
                {
                    if (firstSibling == null)
                    {
                        firstSibling = new FolderNode(treeNode.Name, treeNode.IsExpanded);
                        currentSibling = firstSibling;
                    }
                    else
                    {
                        currentSibling.sibling = new FolderNode(treeNode.Name, treeNode.IsExpanded);
                        currentSibling = currentSibling.sibling;
                        currentSibling.child = SaveTreeViewNodes(subNodes);
                    }
                }
            }
            return firstSibling;
        }
    }
}
