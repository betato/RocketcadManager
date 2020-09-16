using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketcadManagerLib
{
    public class Config
    {
        public static void Open(out Config config)
        {
            config = new Config();
            try
            {
                if (ConstantPaths.ConfigFile.Exists)
                    config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConstantPaths.ConfigFile.FullName));
            }
            catch (JsonException e)
            {
                // Json parsing error, overwrite the old file with the default config and log an error
                LogWriter.Write(LogType.ConfigLoaderError, e.StackTrace);
                Save(config);
            }
        }

        public static void Save(Config config)
        {
            ConstantPaths.ConfigFile.Directory.Create();
            File.WriteAllText(ConstantPaths.ConfigFile.FullName, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        private class FolderNode
        {
            public FolderNode sibling;
            public FolderNode child;

            public string Name { get; private set; }
            public bool Expanded { get; private set; }

            public FolderNode(string name, bool expanded, FolderNode child)
            {
                Name = name;
                Expanded = expanded;
                this.child = child;
            }

            public void Clear()
            {
                sibling = null;
                child = null;
            }
        }

        public List<string> CadDirectories { get; set; } = new List<string>();
        [JsonProperty]
        private FolderNode folderViewExpansion;

        public void SaveTreeViewExpansion(TreeNodeCollection treeNodes)
        {
            folderViewExpansion = SaveTreeViewNodes(treeNodes);
        }

        private FolderNode SaveTreeViewNodes(TreeNodeCollection treeNodes)
        {
            // Add all nodes that are expanded or have expanded children
            FolderNode currentSibling = null;
            FolderNode firstSibling = null;
            foreach (TreeNode treeNode in treeNodes)
            {
                // For all sibling nodes in the collection
                TreeNodeCollection subNodes = treeNode.Nodes;
                if (subNodes.Count <= 0) 
                    continue; // Node does not have children and is therefore not expandable

                FolderNode childNode = SaveTreeViewNodes(subNodes);
                // Add a node if it is expanded or if it has a child (the last child will be expanded)
                if (treeNode.IsExpanded || childNode != null)
                {
                    if (firstSibling == null)
                    {
                        // Set first sibling
                        currentSibling = new FolderNode(treeNode.Text, treeNode.IsExpanded, childNode);
                        firstSibling = currentSibling;
                    }
                    else
                    {
                        // Set next sibling
                        currentSibling.sibling = new FolderNode(treeNode.Text, treeNode.IsExpanded, childNode);
                        currentSibling = currentSibling.sibling;
                    }
                }
            }
            return firstSibling;
        }
        
        public void LoadTreeViewExpansion(TreeNodeCollection treeNodes)
        {
            LoadTreeViewNodes(treeNodes, folderViewExpansion);
        }

        private void LoadTreeViewNodes(TreeNodeCollection treeNodes, FolderNode folderNode)
        {
            // Loop through all sibling nodes
            while (folderNode != null)
            {
                // TODO: Don't reiterate over already expanded nodes
                foreach (TreeNode treeNode in treeNodes)
                {
                    if (treeNode.Text == folderNode.Name)
                    {
                        if (folderNode.Expanded)
                            treeNode.Expand();
                        if (folderNode.child != null)
                            LoadTreeViewNodes(treeNode.Nodes, folderNode.child);
                        break;
                    }
                }
                folderNode = folderNode.sibling;
            }
        }
    }
}
