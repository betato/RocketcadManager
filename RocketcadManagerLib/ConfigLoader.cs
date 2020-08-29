using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketcadManagerLib
{
    public static class ConfigLoader
    {
        private static readonly FileInfo configFile = new FileInfo(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\RocketcadManager\config.json");

        public static void Open(out Config config)
        {
            config = new Config();
            try
            {
                if (configFile.Exists)
                    config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile.FullName));
            }
            catch (Exception e)
            {
                // TODO: Add log file
                Console.WriteLine(e.Message);
            }
           
        }

        public static void Save(Config config)
        {
            try
            {
                configFile.Directory.Create();
                File.WriteAllText(configFile.FullName, JsonConvert.SerializeObject(config));
            }
            catch (Exception e)
            {
                // TODO: Add log file
                Console.WriteLine(e.Message);
            }
        }
    }
}
