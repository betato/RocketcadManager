using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RocketcadManagerLib
{
    public static class LogWriter
    {
        private static readonly DirectoryInfo logFolder = new DirectoryInfo(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\RocketcadManager\logs");

        public static string Write(string tag, string[] message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string namePrefix = Regex.Replace(tag, "[<>:\\\"/\\\\\\|\\?\\*\\t\\r\\n\\s]", "-");
            logFolder.Create();
            string logFile = logFolder.FullName + @"\" + namePrefix + "-" + timestamp + ".txt";
            File.WriteAllLines(logFile, message);
            return logFile;
        }
    }
}
