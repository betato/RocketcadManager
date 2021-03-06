﻿using System;
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
        public static LogType AddinError { get { return new LogType("addin-error"); } }
        public static LogType AddinSaveError { get { return new LogType("addin-save-error"); } }
        public static LogType ConfigLoaderError { get { return new LogType("config-loader-error"); } }
    }

    public static class LogWriter
    {
        public static string Write(LogType logType, string message)
        {
            return Write(logType, new string[] { message });
        }

        public static string Write(LogType logType, string[] message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            if (Regex.IsMatch(logType, "[<>:\\\"/\\\\\\|\\?\\*\\t\\r\\n\\s]"))
                throw new FormatException("Invalid characters in file name");
            ConstantPaths.LogDir.Create();
            string logFile = ConstantPaths.LogDir.FullName + @"\" + logType + "-" + timestamp + ".txt";
            File.WriteAllLines(logFile, message);
            return logFile;
        }
    }
}
