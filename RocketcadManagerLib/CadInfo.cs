﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketcadManagerLib
{
    public abstract class CadInfo
    {
        public string Notes { get; set; } = "";
        public string Description { get; set; } = "";
        public int Stock { get; set; } = 0;
        public int AdditionalRequired { get; set; } = 0;
    }
}
