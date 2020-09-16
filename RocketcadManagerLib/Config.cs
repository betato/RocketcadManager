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
            File.WriteAllText(ConstantPaths.ConfigFile.FullName, JsonConvert.SerializeObject(config));
        }

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

        public List<string> CadDirectories { get; set; } = new List<string>();
        [JsonProperty]
        private FolderNode folderViewExpansion;

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
                        firstSibling = new FolderNode(treeNode.Text, treeNode.IsExpanded);
                        currentSibling = firstSibling;
                    }
                    else
                    {
                        currentSibling.sibling = new FolderNode(treeNode.Name, treeNode.IsExpanded);
                        currentSibling = currentSibling.sibling;
                    }
                    currentSibling.child = SaveTreeViewNodes(subNodes);
                }
            }
            return firstSibling;
        }
    }
}
