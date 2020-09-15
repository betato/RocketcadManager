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
            SaveCadInfo(FileName);
            return 0;
        }

        private int Part_FileSaveAsNotify2(string FileName)
        {
            SaveCadInfo(FileName);
            return 0;
        }

        private int Assembly_FileSaveNotify(string FileName)
        {
            SaveCadInfo(FileName);
            return 0;
        }

        private int Assembly_FileSaveAsNotify2(string FileName)
        {
            SaveCadInfo(FileName);
            return 0;
        }
        #endregion

        private void SaveCadInfo(string filename)
        {
            FileInfo file = new FileInfo(filename);
            if (!FilePathGood(file))
                return;
            ModelDoc2 swModel = swApp.ActiveDoc;
            if (swModel == null)
                return;
            if (swModel.GetPathName() != filename)
            {
                swApp.SendMsgToUser("Error! Path names do not match. " + 
                    swModel.GetPathName() + " " + filename);
            }
            int modelType = swModel.GetType();
            if (modelType != 1 && modelType != 2)
                return; // Probably a drawing

            Image thumbnail = null;
            PartInfo partInfo = null;
            AssemblyInfo assemblyInfo = null;
            
            try
            {
                // Open cad info file if it exists
                if (modelType == 1)
                    CadInfoLoader.OpenJsonImage(file, out partInfo, out thumbnail);
                else if (modelType == 2)
                    CadInfoLoader.OpenJsonImage(file, out assemblyInfo, out thumbnail);
            }
            catch (Exception e)
            {
                swApp.SendMsgToUser(e.Message);
                LogWriter.Write("plugin-cad-save-error", new string[] { e.StackTrace });
            }

            if (modelType == 1)
            {
                // Create a new PartInfo if one was not loaded
                if (partInfo == null)
                    partInfo = new PartInfo();
                GetPartInfo(swModel, ref partInfo);
            }
            else if (modelType == 2)
            {
                // Create a new AssemblyInfo if one was not loaded
                if (assemblyInfo == null)
                    assemblyInfo = new AssemblyInfo();
                GetAssemblyInfo(swModel, ref assemblyInfo);
            }
            thumbnail = GetThumbnailImage(filename);

#if DEBUG
            swApp.SendMsgToUser("Cad info saved");
#endif

            try
            {
                // Save cad file info with updated parts
                if (modelType == 1)
                    CadInfoLoader.SaveJsonImage(file, partInfo, thumbnail);
                else if (modelType == 2)
                    CadInfoLoader.SaveJsonImage(file, assemblyInfo, thumbnail);
            }
            catch (Exception e)
            {
                swApp.SendMsgToUser(e.Message);
                LogWriter.Write("plugin-cad-save-error", new string[] { e.StackTrace });
            }
        }

        private bool GetPartInfo(ModelDoc2 swModel, ref PartInfo partInfo)
        {
            PartDoc part = (PartDoc)swModel;
            // TODO: Modify partInfo here

            return true;
        }

        private bool GetAssemblyInfo(ModelDoc2 swModel, ref AssemblyInfo assemblyInfo)
        {            
            AssemblyDoc assembly = (AssemblyDoc)swModel;
            object[] subcomponentsObj = assembly.GetComponents(true);
            if (subcomponentsObj == null)
                return false;
            
            foreach (object subcomponentObj in subcomponentsObj)
            {
                Component2 subcomponent = (Component2)subcomponentObj;
                ModelDoc2 subModel = subcomponent.GetModelDoc2();
                int subModelType = subModel.GetType();

                // TODO: Eliminate paths that leave the GrabCAD directory
                // TODO: Replace invalid subModel paths (they are invalid for broken references)
                Uri path1 = new Uri(swModel.GetPathName());
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

            return true;
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

        private Image GetThumbnailImage(string filename)
        {
            Image thumb = null;
            try
            {
                object bitmapObj = swApp.GetPreviewBitmap(filename, "Default");
                thumb = PictureDispConverter.Convert(bitmapObj);
            }
            catch (Exception e)
            {
                swApp.SendMsgToUser(e.Message);
                LogWriter.Write("plugin-image-save-error", new string[] { e.StackTrace });
            }
            return thumb;
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
#if DEBUG
            Key.SetValue("Title", "RocketcadManager DEBUG BUILD");
#else
            Key.SetValue("Title", "RocketcadManager");
#endif
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
