using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RocketcadManagerLib;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;
using System.Windows.Forms;

namespace RocketcadManagerPlugin
{
    public class Addin : SwAddin
    {
        private SldWorks swApp;
        private int sessionCookie;
        private Config config;

        public bool ConnectToSW(object ThisSW, int Cookie)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            ConfigLoader.Open(out config);

            swApp = (SldWorks)ThisSW;
            swApp.SetAddinCallbackInfo(0, this, Cookie);
            sessionCookie = Cookie;

            swApp.FileOpenNotify2 += SwApp_FileOpenNotify2;
            swApp.FileNewNotify2 += SwApp_FileNewNotify2;

            swApp.FileCloseNotify += SwApp_FileCloseNotify;

            return true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            LogWriter.Write("plugin-crash", new string[] { ex.StackTrace });
        }

        public bool DisconnectFromSW()
        {
            swApp = null;
            return true;
        }

        #region OPEN_EVENTS
        private int SwApp_FileOpenNotify2(string FileName)
        {
            AddPartSaveEvent();
            return 0;
        }

        private int SwApp_FileNewNotify2(object NewDoc, int DocType, string TemplateName)
        {
            AddPartSaveEvent();
            return 0;
        }

        private int SwApp_FileCloseNotify(string FileName, int reason)
        {
            RemovePartSaveEvent();
            return 0;
        }
        #endregion

        private void RemovePartSaveEvent()
        {
            swApp.SendMsgToUser("Removing");

            ModelDoc2 swModel = swApp.ActiveDoc;
            if (swModel == null)
                return;

            swApp.SendMsgToUser("Removing save event for : " + swModel.GetTitle());

            int modelType = swModel.GetType();
            // 1:part 2:assembly 3:drawing
            if (modelType == 1)
            {
                PartDoc part = (PartDoc)swModel;
                part.FileSaveNotify -= Part_FileSaveNotify;
                part.FileSaveAsNotify2 -= Part_FileSaveAsNotify2;
                swApp.SendMsgToUser("Removed part: " + swModel.GetTitle());
            }
            else if (modelType == 2)
            {
                AssemblyDoc assembly = (AssemblyDoc)swModel;
                assembly.FileSaveNotify -= Assembly_FileSaveNotify;
                assembly.FileSaveAsNotify2 -= Assembly_FileSaveAsNotify2;
                swApp.SendMsgToUser("Removed assembly: " + swModel.GetTitle());
            }
        }

        private void AddPartSaveEvent()
        {
            ModelDoc2 swModel = swApp.ActiveDoc;
            if (swModel == null)
                return;
            
            int modelType = swModel.GetType();
            // 1:part 2:assembly 3:drawing
            if (modelType == 1)
            {
                PartDoc part = (PartDoc)swModel;
                part.FileSaveNotify += Part_FileSaveNotify;
                part.FileSaveAsNotify2 += Part_FileSaveAsNotify2;
            }
            else if (modelType == 2)
            {
                AssemblyDoc assembly = (AssemblyDoc)swModel;
                assembly.FileSaveNotify += Assembly_FileSaveNotify;
                assembly.FileSaveAsNotify2 += Assembly_FileSaveAsNotify2;
            }
        }

        #region SAVE_EVENTS
        private int Part_FileSaveNotify(string FileName)
        {
            SavePartInfo(FileName);
            SaveThumbnailImage(FileName);
            return 0;
        }

        private int Part_FileSaveAsNotify2(string FileName)
        {
            SavePartInfo(FileName);
            SaveThumbnailImage(FileName);
            return 0;
        }

        private int Assembly_FileSaveNotify(string FileName)
        {
            SaveAssemblyInfo(FileName);
            SaveThumbnailImage(FileName);
            return 0;
        }

        private int Assembly_FileSaveAsNotify2(string FileName)
        {
            SaveAssemblyInfo(FileName);
            SaveThumbnailImage(FileName);
            return 0;
        }
        #endregion
        
        private void SavePartInfo(string filename)
        {
            FileInfo file = new FileInfo(filename);
            ModelDoc2 swModel = swApp.ActiveDoc;
            if (swModel == null)
                return;
            if (!FilePathGood(file))
                return;
            if (swModel.GetPathName() != filename)
                return;
            if (swModel.GetType() != 1)
                return;

            PartDoc part = (PartDoc)swModel;
            
            try
            {
                if (!CadInfoLoader.OpenJson(file, out PartInfo partInfo))
                    partInfo = new PartInfo();
                CadInfoLoader.SaveJson(file, partInfo);
            }
            catch (Exception e)
            {
                swApp.SendMsgToUser(e.Message);
                // TODO: Log error
            }
        }

        private void SaveAssemblyInfo(string filename)
        {
            FileInfo file = new FileInfo(filename);
            ModelDoc2 swModel = swApp.ActiveDoc;
            if (swModel == null)
                return;
            if (!FilePathGood(file))
                return;
            if (swModel.GetPathName() != filename)
                return;
            if (swModel.GetType() != 2)
                return;
            
            AssemblyDoc assembly = (AssemblyDoc)swModel;
            
            object[] subcomponentsObj = assembly.GetComponents(true);
            if (subcomponentsObj == null)
                return;

            AssemblyInfo assemblyInfo = new AssemblyInfo();
            try
            {
                // Open assemblyInfo file if it exists
                CadInfoLoader.OpenJson(file, out assemblyInfo);
            }
            catch (Exception e)
            {
                swApp.SendMsgToUser(e.Message);
                // TODO: Log error
            }

            foreach (object subcomponentObj in subcomponentsObj)
            {
                Component2 subcomponent = (Component2)subcomponentObj;
                ModelDoc2 subModel = subcomponent.GetModelDoc2();
                int subModelType = subModel.GetType();

                // TODO: Eliminate paths that leave the GrabCAD directory
                // TODO: Fix relative path math
                Uri path1 = new Uri(file.FullName);
                Uri path2 = new Uri(subModel.GetPathName());
                Uri diff = path1.MakeRelativeUri(path2);
                // TODO: Fix this un-escaping. It's really ghetto
                string relativePath = diff.OriginalString.Replace("%20", " ");

                int count;
                if (subModelType == 1) // Part
                {
                    if (assemblyInfo.Parts.TryGetValue(relativePath, out count))
                    {
                        assemblyInfo.Parts[relativePath] = count + 1;
                    }
                    else
                    {
                        assemblyInfo.Parts.Add(relativePath, 1);
                    }
                }
                else if (subModelType == 2) // Assembly
                {
                    if (assemblyInfo.SubAssemblies.TryGetValue(relativePath, out count))
                    {
                        assemblyInfo.SubAssemblies[relativePath] = count + 1;
                    }
                    else
                    {
                        assemblyInfo.SubAssemblies.Add(relativePath, 1);
                    }
                }
            }

            // Save assembly info with updated parts
            try
            {
                CadInfoLoader.SaveJson(file, assemblyInfo);
            }
            catch (Exception e)
            {
                swApp.SendMsgToUser(e.Message);
                // TODO: Log error
            }
        }

        private bool FilePathGood(FileInfo file)
        {
            if (!file.Exists)
                return false;
            // Make sure file is contained in a designated CAD directory
            Uri child = new Uri(file.FullName);
            foreach (string cadDirectory in config.CadDirectories)
            {
                Uri parent = new Uri(cadDirectory);
                if (parent.IsBaseOf(child))
                    return true;
            }
            return false;
        }

        private void SaveThumbnailImage(string filename)
        {
            try
            {
                object bitmapObj = swApp.GetPreviewBitmap(filename, "Default");

                Image thumb = PictureDispConverter.Convert(bitmapObj);
                CadInfoLoader.SaveImage(new FileInfo(filename), thumb);
            }
            catch (Exception e)
            {
                swApp.SendMsgToUser(e.Message);
                // TODO: Log error
            }
        }

        public class PictureDispConverter : AxHost
        {
            public PictureDispConverter() : base("56174C86-1546-4778-8EE6-B6AC606875E7") { }

            public static Image Convert(object objIDispImage)
            {
                Image objPicture = default(Image);
                objPicture = GetPictureFromIPicture(objIDispImage);
                return objPicture;
            }
        }

        [ComRegisterFunction]
        private static void RegisterAssembly(Type t)
        {
            string path = string.Format(@"SOFTWARE\SolidWorks\AddIns\{0:b}", t.GUID);
            RegistryKey Key = Registry.LocalMachine.CreateSubKey(path);
            Key.SetValue(null, 1);
            Key.SetValue("Title", "RocketcadManager");
            Key.SetValue("Description", "BOOM!");
        } 

        [ComUnregisterFunction]
        private static void UnregisterAssembly(Type t)
        {
            string path = string.Format(@"SOFTWARE\SolidWorks\AddIns\{0:b}", t.GUID);
            Registry.LocalMachine.DeleteSubKey(path);
        }
    }
}
