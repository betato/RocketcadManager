using RocketcadManagerLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketcadManager
{
    class Assembly : CadComponent
    {
        public AssemblyInfo assemblyInfo;
        public List<Tuple<Assembly, int>> subAssemblies = new List<Tuple<Assembly, int>>();
        public List<Tuple<Part, int>> parts = new List<Tuple<Part, int>> ();

        public Assembly(FileInfo fileInfo, Folder parentFolder) : base(fileInfo, parentFolder)
        {
            HasInfo = CadInfoLoader.OpenJson(ComponentFileInfo, out assemblyInfo);
            if (!HasInfo)
                assemblyInfo = new AssemblyInfo();
            CadInfo = assemblyInfo;
        }

        public void AddDependencies(Dictionary<string, Part> allParts, Dictionary<string, Assembly> allAssemblies)
        {
            if (!HasInfo)
                return;

            // Add parts and subassemblies
            foreach (KeyValuePair<string, int> partStr in assemblyInfo.Parts)
            {
                string absolutePath;
                try
                {
                    absolutePath = Path.GetFullPath(ComponentFileInfo.DirectoryName + @"\" + partStr.Key);
                }
                catch (Exception)
                {
                    // Invalid path path in assemblyInfo file
                    MissingComponentError = true;
                    continue;
                }

                if (!allParts.TryGetValue(absolutePath, out Part part))
                {
                    // Part with specified path not found
                    MissingComponentError = true;
                    continue;
                }

                // Add part to self and self to part dependants
                parts.Add(new Tuple<Part, int>(part, partStr.Value));
                part.dependants.Add(new Tuple<Assembly, int>(this, partStr.Value));
            }
            foreach (KeyValuePair<string, int> assemblyStr in assemblyInfo.SubAssemblies)
            {
                string absolutePath;
                try
                {
                    absolutePath = Path.GetFullPath(ComponentFileInfo.DirectoryName + @"\" + assemblyStr.Key);
                }
                catch (Exception)
                {
                    // Invalid path path in assemblyInfo file
                    MissingComponentError = true;
                    continue;
                }

                if (!allAssemblies.TryGetValue(absolutePath, out Assembly assembly))
                {
                    // Assembly with specified path not found
                    MissingComponentError = true;
                    continue;
                }

                // Add assembly to self and self to assembly dependants
                subAssemblies.Add(new Tuple<Assembly, int>(assembly, assemblyStr.Value));
                assembly.dependants.Add(new Tuple<Assembly, int>(this, assemblyStr.Value));
            }
        }

        public override void Save()
        {
            CadInfoLoader.SaveJson(ComponentFileInfo, assemblyInfo);
        }

        public TreeNode SubComponentTree()
        {
            TreeNode thisNode = GetNode();
            foreach (Tuple<Assembly, int> subAssembly in subAssemblies)
            {
                // TODO: do something with the quantity
                thisNode.Nodes.Add(subAssembly.Item1.SubComponentTree());
            }
            foreach (Tuple<Part, int> part in parts)
            {
                // TODO: do something with the quantity
                thisNode.Nodes.Add(part.Item1.GetNode());
            }
            return thisNode;
        }

        public override bool NameOk()
        {
            // Assembly names must end with 00
            return Regex.IsMatch(ComponentFileInfo.Name, @"^([0-9]{2}-)+00(\s.*)*\.(?i)SLDASM(?-i)$");
        }
    }
}
