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
    class Part : CadComponent
    {
        public PartInfo partInfo;

        public Part(FileInfo fileInfo, Folder parentFolder) : base(fileInfo, parentFolder)
        {
            partInfo = Load<PartInfo>();
        }

        public override void Save()
        {
            CadInfoLoader.SaveJson(ComponentFileInfo, partInfo);
        }

        public override bool NameOk()
        {
            return Regex.IsMatch(ComponentFileInfo.Name, ConstantPaths.ValidPartRegex);
        }
    }
}
