using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketcadManagerLib
{
    public class Config
    {
        public Config()
        {
            CadDirectories = new List<string>();
        }

        public Config(Config config) : this()
        {
            // Copy constructor
            foreach (string cadDirectory in config.CadDirectories)
            {
                CadDirectories.Add(cadDirectory);
            }
        }

        public List<string> CadDirectories { get; set; }
    }
}
