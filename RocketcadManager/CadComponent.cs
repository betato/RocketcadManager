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
        public bool HasInfo { get; protected set; } // No cad info file found
        public bool LoadingError { get; protected set; } // Error reading cad info file
        public bool MissingComponentError { get; protected set; } // Referenced components have invalid paths
        public CadInfo CadInfo { get; protected set; }

        public List<Tuple<Assembly, int>> dependants = new List<Tuple<Assembly, int>>();

        public CadComponent(FileInfo fileInfo, Folder parentFolder)
        {
            ComponentFileInfo = fileInfo;
            ParentFolder = parentFolder;
        }

        public T Load<T>() where T : CadInfo, new()
        {
            T cadInfo = default(T);
            try
            {
                HasInfo = CadInfoLoader.OpenJson(ComponentFileInfo, out cadInfo);
            }
            catch (Exception)
            {
                LoadingError = true;
            }
            
            if (!HasInfo)
                cadInfo = new T();

            CadInfo = cadInfo;
            return cadInfo;
        }

        public string GetImageKey()
        {
            if (!NameOk() || !LocationOk())
            {
                if (MissingComponentError || LoadingError)
                    return "WarningErrorFile";
                else if (!HasInfo)
                    return "WarningQuestionFile";
                else
                    return "WarningFile";
            }
            else if (MissingComponentError || LoadingError)
                return "ErrorFile";
            else if (!HasInfo)
                return "QuestionFile";
            else
                return "File";
        }

        public TreeNode GetNode()
        {
            TreeNode thisNode = new TreeNode();
            string imageKey = GetImageKey();

            thisNode.Tag = this;
            thisNode.Text = ComponentFileInfo.Name;
            thisNode.ImageKey = imageKey;
            thisNode.SelectedImageKey = imageKey;

            return thisNode;
        }

        public int UsageCount()
        {
            int count = 0;
            foreach (Tuple<Assembly, int> dependant in dependants)
            {
                // Add dependancy and additional requirements
                Assembly dependantAssem = dependant.Item1;
                count += dependant.Item2 * (dependantAssem.UsageCount() + dependantAssem.CadInfo.AdditionalRequired);
            }
            return count;
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

        public abstract bool NameOk();

        public bool LocationOk()
        {
            return Regex.Match(ComponentFileInfo.Name, ConstantPaths.ParentFolderRegex).Value
                == Regex.Match(ComponentFileInfo.Name, ConstantPaths.ChildFileRegex).Value;
        }
    }
}
