﻿using RocketcadManagerLib;
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
    class Part : CadComponent
    {
        public PartInfo partInfo;
        public List<Tuple<Assembly, int>> dependants = new List<Tuple<Assembly, int>>();

        public Part(FileInfo fileInfo) : base(fileInfo)
        {
            HasInfo = CadInfoLoader.OpenJson(ComponentFileInfo, out partInfo);
            if (!HasInfo)
                partInfo = new PartInfo();
        }

        public override void Save()
        {
            CadInfoLoader.SaveJson(ComponentFileInfo, partInfo);
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
