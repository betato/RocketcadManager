using RocketcadManagerLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketcadManager
{
    class Assembly : CadComponent
    {
        public AssemblyInfo assemblyInfo;
        public List<Tuple<Assembly, int>> dependants = new List<Tuple<Assembly, int>>();
        public List<Tuple<Assembly, int>> subAssemblies = new List<Tuple<Assembly, int>>();
        public List<Tuple<Part, int>> parts = new List<Tuple<Part, int>> ();

        public Assembly(FileInfo fileInfo) : base(fileInfo)
        {
            HasInfo = CadInfoLoader.OpenJson(ComponentFileInfo, out assemblyInfo);
            if (!HasInfo)
                assemblyInfo = new AssemblyInfo();
        }

        public void AddParts(Dictionary<string, Part> allParts, Dictionary<string, Assembly> allAssemblies)
        {
            if (!HasInfo)
                return;

            // Add parts and subassemblies
            foreach (KeyValuePair<string, int> partStr in assemblyInfo.Parts)
            {
                string absolutePath = Path.GetFullPath(ComponentFileInfo.DirectoryName + @"\" + partStr.Key);
                Part part = allParts[absolutePath];

                // Add part to self and self to part dependants
                parts.Add(new Tuple<Part, int>(part, partStr.Value));
                part.dependants.Add(new Tuple<Assembly, int>(this, partStr.Value));
            }
            foreach (KeyValuePair<string, int> assemblyStr in assemblyInfo.SubAssemblies)
            {
                string absolutePath = Path.GetFullPath(ComponentFileInfo.DirectoryName + @"\" + assemblyStr.Key);
                Assembly assembly = allAssemblies[absolutePath];

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
    }
}
