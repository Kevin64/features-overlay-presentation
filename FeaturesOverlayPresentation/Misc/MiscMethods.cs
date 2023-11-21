using ConstantsDLL.Properties;
using FeaturesOverlayPresentation.Updater;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace FeaturesOverlayPresentation.Misc
{
    /// <summary> 
    /// Class for miscelaneous methods
    /// </summary>
    internal static class MiscMethods
    {
        /// <summary> 
        /// Creates registrys keys when a successful update check is made
        /// </summary>
        /// <param name="ui">An UpdateInfo object to write into the registry</param>
        internal static void RegCreateUpdateData(UpdateInfo ui)
        {
            RegistryKey rk = Registry.CurrentUser.CreateSubKey(ConstantsDLL.Properties.GenericResources.REGISTRY_FOP_REG_KEY, true);
            rk.SetValue(ConstantsDLL.Properties.GenericResources.GITHUB_ETAG, ui.ETag, RegistryValueKind.String);
            rk.SetValue(ConstantsDLL.Properties.GenericResources.GITHUB_TAG_NAME, ui.TagName, RegistryValueKind.String);
            rk.SetValue(ConstantsDLL.Properties.GenericResources.GITHUB_BODY, ui.Body, RegistryValueKind.String);
            rk.SetValue(ConstantsDLL.Properties.GenericResources.GITHUB_HTML_URL, ui.HtmlUrl, RegistryValueKind.String);
        }

        /// <summary>
        /// Checks the registry for existing update metadata
        /// </summary>
        /// <returns>An UpdateInfo object containing the ETag, TagName, Body and HtmlURL</returns>
        internal static UpdateInfo RegCheckUpdateData()
        {
            UpdateInfo ui = new UpdateInfo();
            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey(ConstantsDLL.Properties.GenericResources.REGISTRY_FOP_REG_KEY);
                ui.ETag = rk.GetValue(ConstantsDLL.Properties.GenericResources.GITHUB_ETAG).ToString();
                ui.TagName = rk.GetValue(ConstantsDLL.Properties.GenericResources.GITHUB_TAG_NAME).ToString();
                ui.Body = rk.GetValue(ConstantsDLL.Properties.GenericResources.GITHUB_BODY).ToString();
                ui.HtmlUrl = rk.GetValue(ConstantsDLL.Properties.GenericResources.GITHUB_HTML_URL).ToString();
                return ui;
            }
            catch
            {
                return null;
            }
        }

        /// <summary> 
        /// Checks via registry if the program was already executed
        /// </summary>
        /// <returns>'true' if already executed, 'false' if not</returns>
        public static bool RegCheck()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(ConstantsDLL.Properties.GenericResources.REGISTRY_FOP_REG_KEY);
            string k = rk.GetValue(ConstantsDLL.Properties.GenericResources.REGISTRY_DID_IT_RUN_ALREADY).ToString();
            return k.Equals("1");
        }

        /// <summary> 
        /// Creates RunOnce and FOP registry keys
        /// </summary>
        public static void RegCreate()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(ConstantsDLL.Properties.GenericResources.REGISTRY_FOP_RUN_ONCE_KEY, true);
            if (Environment.Is64BitOperatingSystem)
            {
                key.SetValue(ConstantsDLL.Properties.GenericResources.FOP_NAME, ConstantsDLL.Properties.GenericResources.INSTALL_PATH_FOP_X86, RegistryValueKind.String);
            }
            else
            {
                key.SetValue(ConstantsDLL.Properties.GenericResources.FOP_NAME, ConstantsDLL.Properties.GenericResources.INSTALL_PATH_FOP_X64, RegistryValueKind.String);
            }

            RegistryKey key2 = Registry.CurrentUser.CreateSubKey(ConstantsDLL.Properties.GenericResources.REGISTRY_FOP_REG_KEY);
            key2.SetValue(ConstantsDLL.Properties.GenericResources.REGISTRY_DID_IT_RUN_ALREADY, 0, RegistryValueKind.DWord);
        }

        /// <summary> 
        /// ReCreates RunOnce registry key
        /// </summary>
        /// <param name="empty"></param>
        public static void RegRecreate(bool empty)
        {
            if (!empty && !RegCheck())
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(ConstantsDLL.Properties.GenericResources.REGISTRY_FOP_RUN_ONCE_KEY, true);
                if (!key.GetValueNames().Contains(ConstantsDLL.Properties.GenericResources.FOP_NAME))
                {
                    if (Environment.Is64BitOperatingSystem)
                    {
                        key.SetValue(ConstantsDLL.Properties.GenericResources.FOP_NAME, ConstantsDLL.Properties.GenericResources.INSTALL_PATH_FOP_X86, RegistryValueKind.String);
                    }
                    else
                    {
                        key.SetValue(ConstantsDLL.Properties.GenericResources.FOP_NAME, ConstantsDLL.Properties.GenericResources.INSTALL_PATH_FOP_X64, RegistryValueKind.String);
                    }
                }
            }
        }

        /// <summary> 
        /// Deletes RunOnce registry key
        /// </summary>
        public static void RegDelete()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(ConstantsDLL.Properties.GenericResources.REGISTRY_FOP_RUN_ONCE_KEY, true);
            if (key.GetValueNames().Contains(ConstantsDLL.Properties.GenericResources.FOP_NAME))
            {
                key.DeleteValue(ConstantsDLL.Properties.GenericResources.FOP_NAME);
            }
        }

        /// <summary> 
        /// Checks OS version
        /// </summary>
        /// <return>The image paths according to OS version</return>
        public static string OSCheck()
        {
            string current = Directory.GetCurrentDirectory();
            Version osFullVer = Environment.OSVersion.Version;
            //int osBuild = Convert.ToInt32(osFullVer);
            return osFullVer.Major == Convert.ToInt32(ConstantsDLL.Properties.GenericResources.WIN_7_NT_MAJOR) && osFullVer.Minor == Convert.ToInt32(ConstantsDLL.Properties.GenericResources.WIN_7_NT_MINOR)
                ? current + ConstantsDLL.Properties.GenericResources.FOP_RESOURCES_DIR + ConstantsDLL.Properties.GenericResources.FOP_IMG_DIR + ConstantsDLL.Properties.GenericResources.WIN_7_FOP_IMG_DIR
                : osFullVer.Build >= Convert.ToInt32(ConstantsDLL.Properties.GenericResources.WIN_10_NT_BUILD) && osFullVer.Build < Convert.ToInt32(ConstantsDLL.Properties.GenericResources.WIN_11_NT_BUILD)
                ? current + ConstantsDLL.Properties.GenericResources.FOP_RESOURCES_DIR + ConstantsDLL.Properties.GenericResources.FOP_IMG_DIR + ConstantsDLL.Properties.GenericResources.WIN_10_FOP_IMG_DIR
                : current + ConstantsDLL.Properties.GenericResources.FOP_RESOURCES_DIR + ConstantsDLL.Properties.GenericResources.FOP_IMG_DIR + ConstantsDLL.Properties.GenericResources.WIN_11_FOP_IMG_DIR;
        }

        /// <summary> 
        /// Checks assembly version
        /// </summary>
        /// <returns>The assembly version</returns>
        public static string Version
        {
            get
            {
                return "v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        /// <summary> 
        /// Checks the current screen resolution
        /// </summary>
        /// <param name="exit"></param>
        /// <returns>'true' if resolution above minimum, 'false' otherwise</returns>
        public static bool ResolutionError(bool exit)
        {
            if (SystemParameters.PrimaryScreenWidth < Convert.ToInt32(ConstantsDLL.Properties.GenericResources.FOP_MIN_SCREEN_WIDTH) || SystemParameters.PrimaryScreenHeight < Convert.ToInt32(ConstantsDLL.Properties.GenericResources.FOP_MIN_SCREEN_HEIGHT))
            {
                _ = MessageBox.Show(string.Format(ConstantsDLL.Properties.UIStrings.RESOLUTION_WARNING, ConstantsDLL.Properties.GenericResources.FOP_MIN_SCREEN_WIDTH, ConstantsDLL.Properties.GenericResources.FOP_MIN_SCREEN_HEIGHT), ConstantsDLL.Properties.UIStrings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                if (exit)
                {
                    Application.Current.Shutdown();
                }
                return false;
            }
            return true;
        }

        /// <summary> 
        /// Checks if logfile exists
        /// </summary>
        /// <param name="path">Path of the log file</param>
        /// <returns>"true" if exists, "false" otherwise</returns>
        public static string CheckIfLogExists(string path)
        {
            bool b;
            try
            {
#if DEBUG
                //Checks if log directory exists
                b = File.Exists(path + ConstantsDLL.Properties.GenericResources.LOG_FILENAME_FOP + "-v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version + "-" + GenericResources.DEV_STATUS_BETA + ConstantsDLL.Properties.GenericResources.LOG_FILE_EXT);
#else
                //Checks if log file exists
                b = File.Exists(path + ConstantsDLL.Properties.GenericResources.LOG_FILENAME_FOP + "-v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version + ConstantsDLL.Properties.GenericResources.LOG_FILE_EXT);
#endif
                //If not, creates a new directory
                if (!b)
                {
                    Directory.CreateDirectory(path);
                    return "false";
                }
                return "true";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
