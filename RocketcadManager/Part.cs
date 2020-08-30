﻿using RocketcadManagerLib;
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
            HasInfo = CadInfoLoader.OpenJson(ComponentFileInfo, out partInfo);
            if (!HasInfo)
                partInfo = new PartInfo();
            CadInfo = partInfo;
        }

        public override void Save()
        {
            CadInfoLoader.SaveJson(ComponentFileInfo, partInfo);
        }

        public override bool NameOk()
        {
            // Part names cannot end with 00
            return base.NameOk() && !Regex.IsMatch(ComponentFileInfo.Name, @"^([0-9]{2}-)+00(\s.*)*\.(?i)SLDASM(?-i)$");
        }
    }
}
