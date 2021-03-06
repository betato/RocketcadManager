﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketcadManagerLib
{
    public static class CadInfoLoader
    {
        private static readonly string JsonFileName = "status.json";
        private static readonly string ImageFileName = "thumbnail.jpg";

        private static FileInfo GetinfoFile(FileInfo cadFile)
        {
            return new FileInfo(cadFile.DirectoryName + @"\.partstatus\" + cadFile.Name + ".status");
        }

        private static void HideFolder(FileInfo infoFile)
        {
            DirectoryInfo di = infoFile.Directory;
            if ((di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                di.Attributes |= FileAttributes.Hidden;
        }

        public static bool InfoFileExists(FileInfo cadFile)
        {
            return GetinfoFile(cadFile).Exists;
        }

        public static bool OpenJson<T>(FileInfo cadFile, out T info) where T : CadInfo
        {
            info = null;
            Image image = null;
            return OpenJsonImage(cadFile, ref info, ref image, true, false);
        }

        public static bool OpenImage(FileInfo cadFile, out Image image)
        {
            image = null;
            CadInfo info = null;
            return OpenJsonImage(cadFile, ref info, ref image, false, true);
        }

        public static bool OpenJsonImage<T>(FileInfo cadFile, out T info, out Image image) where T : CadInfo
        {
            image = null;
            info = null;
            return OpenJsonImage(cadFile, ref info, ref image, true, true);
        }

        private static bool OpenJsonImage<T>(FileInfo cadFile, ref T info, ref Image image, bool getInfo, bool getImage) where T : CadInfo
        {
            FileInfo infoFile = GetinfoFile(cadFile);
            if (!infoFile.Exists)
                return false;

            using (FileStream zipToOpen = new FileStream(infoFile.FullName, FileMode.Open))
            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
            {
                ZipArchiveEntry entry = archive.GetEntry(JsonFileName);
                if (entry == null)
                    return false;

                if (getInfo)
                {
                    using (StreamReader streamReader = new StreamReader(entry.Open()))
                    using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                    {
                        JsonSerializer ser = new JsonSerializer();
                        info = ser.Deserialize<T>(jsonReader);
                        Console.WriteLine("Opened " + infoFile.Name);
                    }
                }

                if (getImage)
                {
                    entry = archive.GetEntry(ImageFileName);
                    if (entry == null)
                    {
                        Console.WriteLine("No thumbnail for " + infoFile.Name);
                        return false;
                    }
                    image = Image.FromStream(entry.Open());
                }

                return true;
            }
        }

        public static void SaveJson<T>(FileInfo cadFile, T info) where T : CadInfo
        {
            SaveJsonImage(cadFile, info, null);
        }

        public static void SaveImage(FileInfo cadFile, Image image)
        {
            SaveJsonImage<CadInfo>(cadFile, null, image);
        }

        public static void SaveJsonImage<T>(FileInfo cadFile, T info, Image image) where T : CadInfo
        {
            // Create directory if it doesn't already exist
            FileInfo infoFile = GetinfoFile(cadFile);
            infoFile.Directory.Create();

            using (FileStream zipFile = new FileStream(infoFile.FullName, FileMode.OpenOrCreate))
            using (ZipArchive archive = new ZipArchive(zipFile, ZipArchiveMode.Update))
            {
                ZipArchiveEntry entry;

                // Update json info
                if (info != null)
                {
                    entry = archive.GetEntry(JsonFileName);
                    // Remove previous entry
                    if (entry != null)
                        entry.Delete();

                    ZipArchiveEntry jsonEntry = archive.CreateEntry(JsonFileName);
                    using (StreamWriter writer = new StreamWriter(jsonEntry.Open()))
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(jsonWriter, info);
                        jsonWriter.Flush();
                    }
                }

                // Add thumbnail image
                if (image != null)
                {
                    // Remove previous image
                    entry = archive.GetEntry(ImageFileName);
                    if (entry != null)
                        entry.Delete();

                    using (MemoryStream imageStream = new MemoryStream())
                    {
                        image.Save(imageStream, ImageFormat.Jpeg);
                        imageStream.Position = 0;

                        ZipArchiveEntry imageEntry = archive.CreateEntry(ImageFileName);
                        imageStream.WriteTo(imageEntry.Open());
                    }
                }
            }
            HideFolder(infoFile);
        }
    }
}
