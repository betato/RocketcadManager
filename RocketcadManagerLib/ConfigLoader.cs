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
        public static void Open(out Config config)
        {
            config = new Config();
            try
            {
                if (ConstantPaths.ConfigFile.Exists)
                    config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConstantPaths.ConfigFile.FullName));
            }
            catch (JsonException e)
            {
                // Json parsing error, overwrite the old file with the default config and log an error
                LogWriter.Write(LogType.ConfigLoaderError, e.StackTrace);
                Save(config);
            }
        }

        public static void Save(Config config)
        {
            ConstantPaths.ConfigFile.Directory.Create();
            File.WriteAllText(ConstantPaths.ConfigFile.FullName, JsonConvert.SerializeObject(config));
        }
    }
}
