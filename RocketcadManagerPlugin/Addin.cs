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
using SolidWorks.Interop.swconst;

namespace RocketcadManagerPlugin
{
    public class Addin : SwAddin
    {
        private SldWorks swApp;
        private int sessionCookie;
        private Config config;
        private ClientPipe addinLinkClient = new ClientPipe(ConstantPaths.DefaultPipeName);

        public bool ConnectToSW(object ThisSW, int Cookie)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Config.Open(out config);
            
            // Connect to SW
            try
            {
                swApp = (SldWorks)ThisSW;
                swApp.SetAddinCallbackInfo(0, this, Cookie);
                sessionCookie = Cookie;
            }
            catch (Exception e)
            {
                // Can't show an error message here because sw is not connected
                LogWriter.Write(LogType.AddinError, e.StackTrace);
            }

            // Register file open event handlers
            try
            {
                swApp.FileOpenNotify2 += SwApp_FileOpenNotify2;
                swApp.FileNewNotify2 += SwApp_FileNewNotify2;
                swApp.FileCloseNotify += SwApp_FileCloseNotify;
            }
            catch (Exception e)
            {
                LogErrorWithMessage(LogType.AddinError, e);
            }

            addinLinkClient.Connected += AddinLinkClient_Connected;
            addinLinkClient.Disconnected += AddinLinkClient_Disconnected;
            addinLinkClient.MessageRecieved += AddinLinkClient_MessageRecieved;

            swApp.SendMsgToUser2("starting",
                (int)swMessageBoxIcon_e.swMbInformation, (int)swMessageBoxBtn_e.swMbOk);
            addinLinkClient.Start();

            return true;
        }

        private void AddinLinkClient_MessageRecieved(string message)
        {
            swApp.SendMsgToUser2(message,
                (int)swMessageBoxIcon_e.swMbInformation, (int)swMessageBoxBtn_e.swMbOk);
        }

        private void AddinLinkClient_Disconnected()
        {
            swApp.SendMsgToUser2("AddinLinkClient_Disconnected",
                (int)swMessageBoxIcon_e.swMbInformation, (int)swMessageBoxBtn_e.swMbOk);
        }

        private void AddinLinkClient_Connected()
        {
            swApp.SendMsgToUser2("AddinLinkClient_Connected",
                (int)swMessageBoxIcon_e.swMbInformation, (int)swMessageBoxBtn_e.swMbOk);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Solidworks might be catching errors before they are logged
            // All possible errors should be caught locally
            Exception ex = (Exception)e.ExceptionObject;
            LogWriter.Write(LogType.AddinCrash, ex.StackTrace);
        }

        private void LogErrorWithMessage(LogType logType, Exception e)
        {
            string logLocation = LogWriter.Write(logType, e.StackTrace);
            swApp.SendMsgToUser2(string.Format("Error: {0}\nLog Saved to:\n{1}", e.Message, logLocation), 
                (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);
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
            // TODO: Remove part save events when files are closed (make this actually work)
#if DEBUG
            swApp.SendMsgToUser2("Removing",
                (int)swMessageBoxIcon_e.swMbInformation, (int)swMessageBoxBtn_e.swMbOk);
#endif
            ModelDoc2 swModel = swApp.ActiveDoc;
            if (swModel == null)
            {
                swApp.SendMsgToUser2("Error! swModel is null",
                (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);
                return;
            }

            swApp.SendMsgToUser2("Removing save event for : " + swModel.GetTitle(),
                (int)swMessageBoxIcon_e.swMbInformation, (int)swMessageBoxBtn_e.swMbOk);

            try
            {
                int modelType = swModel.GetType(); // 1:part 2:assembly 3:drawing
                if (modelType == 1)
                {
                    PartDoc part = (PartDoc)swModel;
                    part.FileSaveNotify -= Part_FileSaveNotify;
                    part.FileSaveAsNotify2 -= Part_FileSaveAsNotify2;
#if DEBUG
                    swApp.SendMsgToUser2("Removed part: " + swModel.GetTitle(),
                (int)swMessageBoxIcon_e.swMbInformation, (int)swMessageBoxBtn_e.swMbOk);
#endif
                }
                else if (modelType == 2)
                {
                    AssemblyDoc assembly = (AssemblyDoc)swModel;
                    assembly.FileSaveNotify -= Assembly_FileSaveNotify;
                    assembly.FileSaveAsNotify2 -= Assembly_FileSaveAsNotify2;
#if DEBUG
                    swApp.SendMsgToUser2("Removed assembly: " + swModel.GetTitle(),
                (int)swMessageBoxIcon_e.swMbInformation, (int)swMessageBoxBtn_e.swMbOk);
#endif
                }
            }
            catch (Exception e)
            {
                LogErrorWithMessage(LogType.AddinError, e);
            }
        }

        private void AddPartSaveEvent()
        {
            ModelDoc2 swModel = swApp.ActiveDoc;
            if (swModel == null)
            {
                swApp.SendMsgToUser2("Error! swModel is null",
                (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);
                return;
            }

            try
            {
                int modelType = swModel.GetType(); // 1:part 2:assembly 3:drawing
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
            catch (Exception e)
            {
                LogErrorWithMessage(LogType.AddinError, e);
            }
            
#if DEBUG
            swApp.SendMsgToUser2("Added part save event",
                (int)swMessageBoxIcon_e.swMbInformation, (int)swMessageBoxBtn_e.swMbOk);
#endif
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
#if DEBUG
            swApp.SendMsgToUser2("File save started",
                (int)swMessageBoxIcon_e.swMbInformation, (int)swMessageBoxBtn_e.swMbOk);
#endif
            if (!FilePathGood(filename))
            {
                swApp.SendMsgToUser2(string.Format("Error! Invalid file path. {0}", filename),
                (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);
                return;
            }
            ModelDoc2 swModel = swApp.ActiveDoc;
            if (swModel == null)
            {
                swApp.SendMsgToUser2("Error! swModel is null",
                (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);
                return;
            }
            if (swModel.GetPathName() != filename)
            {
                swApp.SendMsgToUser2(
                    string.Format("Error! Path names do not match. {0} {1}", 
                    swModel.GetPathName(), filename), (int)swMessageBoxIcon_e.swMbStop, 
                    (int)swMessageBoxBtn_e.swMbOk);
                return;
            }
            int modelType = swModel.GetType();
            if (modelType != 1 && modelType != 2)
                return; // Probably a drawing

            Image thumbnail = null;
            PartInfo partInfo = null;
            AssemblyInfo assemblyInfo = null;
            FileInfo file = new FileInfo(filename);

            // Load existing cad info file if it exists
            try
            {
                if (modelType == 1)
                    CadInfoLoader.OpenJsonImage(file, out partInfo, out thumbnail);
                else if (modelType == 2)
                    CadInfoLoader.OpenJsonImage(file, out assemblyInfo, out thumbnail);
            }
            catch (Exception e)
            {
                LogErrorWithMessage(LogType.AddinSaveError, e);
            }

            // Create a new PartInfo or AssemblyInfo if one was not loaded and update the info
            try
            {
                // TODO: Possibly store "Description" and other property names in a constants file
                CustomPropertyManager propertyManager = swModel.Extension.CustomPropertyManager[""];
                propertyManager.Get6("Description", false, out string val, out string resolvedVal, 
                    out bool wasResolved, out bool linkToProperty);

                if (resolvedVal == null || !wasResolved)
                    resolvedVal = "";

                if (modelType == 1)
                {
                    if (partInfo == null)
                        partInfo = new PartInfo();
                    GetPartInfo(swModel, ref partInfo);
                    partInfo.Description = resolvedVal;
                }
                else if (modelType == 2)
                {
                    if (assemblyInfo == null)
                        assemblyInfo = new AssemblyInfo();
                    GetAssemblyInfo(swModel, ref assemblyInfo);
                    assemblyInfo.Description = resolvedVal;
                }
            }
            catch (Exception e)
            {
                LogErrorWithMessage(LogType.AddinSaveError, e);
            }

            // Get a new thumbnail image
            thumbnail = GetThumbnailImage(filename);

            // Save the cad info file info with updated parts
            try
            {
                if (modelType == 1)
                    CadInfoLoader.SaveJsonImage(file, partInfo, thumbnail);
                else if (modelType == 2)
                    CadInfoLoader.SaveJsonImage(file, assemblyInfo, thumbnail);
            }
            catch (Exception e)
            {
                LogErrorWithMessage(LogType.AddinSaveError, e);
            }
#if DEBUG
            swApp.SendMsgToUser2("Saved!",
                (int)swMessageBoxIcon_e.swMbInformation, (int)swMessageBoxBtn_e.swMbOk);
#endif
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

            // Add referenced arts and subassemblies with absolute paths
            Dictionary<string, int> parts = new Dictionary<string, int>();
            Dictionary<string, int> subAssemblies = new Dictionary<string, int>();
            string parentPath = swModel.GetPathName();
            foreach (object subcomponentObj in subcomponentsObj)
            {
                Component2 subcomponent = (Component2)subcomponentObj;
                ModelDoc2 subModel = subcomponent.GetModelDoc2();
                int subModelType = subModel.GetType();
                string subModelPath = subModel.GetPathName();
                int count;

                if (subModelType == 1) // Part
                {
                    if (parts.TryGetValue(subModelPath, out count))
                        parts[subModelPath] = count + 1;
                    else
                        parts.Add(subModelPath, 1);
                }
                else if (subModelType == 2) // Assembly
                {
                    if (subAssemblies.TryGetValue(subModelPath, out count))
                        subAssemblies[subModelPath] = count + 1;
                    else
                        subAssemblies.Add(subModelPath, 1);
                }
            }

            // Make paths relative to this assembly and replace invalid paths
            assemblyInfo.Parts.Clear();
            int invalidPartCount = 0;
            foreach (string partPath in parts.Keys)
            {
                string relativePath;
                if (FilePathGood(partPath))
                    relativePath = GetRelativePath(parentPath, partPath);
                else
                    // TODO: Store INVALID_PART_REF_{0} and INVALID_ASSEMBLY_REF_{0} as constants in the library
                    relativePath = string.Format("INVALID_PART_REF_{0}", invalidPartCount++);
                assemblyInfo.Parts.Add(relativePath, parts[partPath]);
            }
            assemblyInfo.SubAssemblies.Clear();
            int invalidAssemblyCount = 0;
            foreach (string subAssemblyPath in subAssemblies.Keys)
            {
                string relativePath;
                if (FilePathGood(subAssemblyPath))
                    relativePath = GetRelativePath(parentPath, subAssemblyPath);
                else
                    relativePath = string.Format("INVALID_ASSEMBLY_REF_{0}", invalidAssemblyCount++);
                assemblyInfo.SubAssemblies.Add(relativePath, subAssemblies[subAssemblyPath]);
            }

            return true;
        }

        private string GetRelativePath(string parentPath, string childPath)
        {
            Uri parentPathUri = new Uri(parentPath);
            Uri childPathURI = new Uri(childPath);
            Uri diff = parentPathUri.MakeRelativeUri(childPathURI);
            // TODO: Fix this un-escaping. It's really ghetto
            return diff.OriginalString.Replace("%20", " ");
        }

        private bool FilePathGood(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;
            }
            catch (Exception)
            {
                // Not a valid path
                return false;
            }
            // Make sure file is contained in a designated CAD directory
            Uri child = new Uri(filePath);
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
                LogErrorWithMessage(LogType.AddinSaveError, e);
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
            Key.SetValue("Description", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
#else
            Key.SetValue("Title", "RocketcadManager");
            Key.SetValue("Description", "BOOM!");
#endif
        }

        [ComUnregisterFunction]
        private static void UnregisterAssembly(Type t)
        {
            string path = string.Format(@"SOFTWARE\SolidWorks\AddIns\{0:b}", t.GUID);
            Registry.LocalMachine.DeleteSubKey(path);
        }
    }
}
