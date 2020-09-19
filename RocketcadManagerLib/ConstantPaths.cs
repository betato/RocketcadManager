using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketcadManagerLib
{
    public static class ConstantPaths
    {
        private static readonly string dataFolder = 
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\RocketcadManager";

        public static DirectoryInfo LogDir { get { return new DirectoryInfo(dataFolder + @"\logs"); } }
        public static FileInfo ConfigFile { get { return new FileInfo(dataFolder + @"\config.json"); } }

        public static readonly string ValidPartRegex = @"^([0-9]{2}-)+(0[1-9]|[1-9][0-9])(\s.*)?\.(?i)SLDPRT(?-i)$";
        public static readonly string ValidAssemblyRegex = @"^([0-9]{2}-)+00(\s.*)?\.(?i)SLDASM(?-i)$";
        public static readonly string ValidFolderRegex = @"^([0-9]{2}-)+[0-9]{2}($|\s)";

        public static readonly string ParentFolderRegex = @"^(([0-9]{2}-)+[0-9]{2})";
        public static readonly string ChildFolderRegex = @"^([0-9]{2}(-[0-9]{2})*)-[0-9]{2}($|\s)";
        public static readonly string ChildFileRegex = @"^([0-9]{2}(-[0-9]{2})*)-[0-9]{2}(\s.*)?\.(?i)(SLDASM|SLDPRT)(?-i)$";
        public static readonly bool IgnoreTopLevelFolders = true;

        public static readonly string DefaultPipeName = "Q26J5ZO2Y5OQUWXMFK9P938JKXU8S6LO";
    }
}
