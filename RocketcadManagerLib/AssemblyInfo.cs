using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketcadManagerLib
{
    public class AssemblyInfo : CadInfo
    {
        // Local paths
        public Dictionary<string, int> Parts { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> SubAssemblies { get; set; } = new Dictionary<string, int>();
    }
}
