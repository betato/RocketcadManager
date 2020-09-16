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
    }
}
