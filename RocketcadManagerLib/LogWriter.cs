using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RocketcadManagerLib
{
    public class LogType
    {
        private LogType(string tag) { Tag = tag; }
        public string Tag { get; private set; }

        public override string ToString() { return Tag; }
        public static implicit operator string(LogType logType) { return logType.Tag; }
        
        public static LogType ManagerCrash { get { return new LogType("manager-crash"); } }
        public static LogType AddinCrash { get { return new LogType("addin-crash"); } }
        public static LogType AddinSaveError { get { return new LogType("addin-save-error"); } }
        public static LogType ConfigLoaderError { get { return new LogType("config-loader-error"); } }
    }

    public static class LogWriter
    {
        private static readonly DirectoryInfo logFolder = new DirectoryInfo(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\RocketcadManager\logs");

        public static string Write(LogType logType, string[] message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            if (Regex.IsMatch(logType, "[<>:\\\"/\\\\\\|\\?\\*\\t\\r\\n\\s]"))
                throw new FormatException("Invalid characters in file name");
            logFolder.Create();
            string logFile = logFolder.FullName + @"\" + logType + "-" + timestamp + ".txt";
            File.WriteAllLines(logFile, message);
            return logFile;
        }
    }
}
