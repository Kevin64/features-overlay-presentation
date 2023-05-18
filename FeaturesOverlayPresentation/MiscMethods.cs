using ConstantsDLL;
using FeaturesOverlayPresentation.Properties;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace FeaturesOverlayPresentation
{
    internal static class MiscMethods
    {
        //Checks via registry if the program was already executed
        public static bool RegCheck()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(ConstantsDLL.Properties.Resources.FOP_REG_KEY);
            string k = rk.GetValue(ConstantsDLL.Properties.Resources.DID_IT_RUN_ALREADY).ToString();
            return k.Equals("1");
        }

        //Creates RunOnce and FOP registry keys
        public static void RegCreate()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(ConstantsDLL.Properties.Resources.FOP_RUN_ONCE_KEY, true);
            if (Environment.Is64BitOperatingSystem)
            {
                key.SetValue(ConstantsDLL.Properties.Resources.FOP, ConstantsDLL.Properties.Resources.FOP_X86, RegistryValueKind.String);
            }
            else
            {
                key.SetValue(ConstantsDLL.Properties.Resources.FOP, ConstantsDLL.Properties.Resources.FOP_X64, RegistryValueKind.String);
            }

            RegistryKey key2 = Registry.CurrentUser.CreateSubKey(ConstantsDLL.Properties.Resources.FOP_REG_KEY);
            key2.SetValue(ConstantsDLL.Properties.Resources.DID_IT_RUN_ALREADY, 0, RegistryValueKind.DWord);
        }

        //ReCreates RunOnce registry key
        public static void RegRecreate(bool empty)
        {
            if (!empty && !RegCheck())
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(ConstantsDLL.Properties.Resources.FOP_RUN_ONCE_KEY, true);
                if (!key.GetValueNames().Contains(ConstantsDLL.Properties.Resources.FOP))
                {
                    if (Environment.Is64BitOperatingSystem)
                    {
                        key.SetValue(ConstantsDLL.Properties.Resources.FOP, ConstantsDLL.Properties.Resources.FOP_X86, RegistryValueKind.String);
                    }
                    else
                    {
                        key.SetValue(ConstantsDLL.Properties.Resources.FOP, ConstantsDLL.Properties.Resources.FOP_X64, RegistryValueKind.String);
                    }
                }
            }
        }

        //Deletes RunOnce registry key
        public static void RegDelete()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(ConstantsDLL.Properties.Resources.FOP_RUN_ONCE_KEY, true);
            if (key.GetValueNames().Contains(ConstantsDLL.Properties.Resources.FOP))
            {
                key.DeleteValue(ConstantsDLL.Properties.Resources.FOP);
            }
        }

        //Checks OS version
        public static string OSCheck()
        {
            string current = Directory.GetCurrentDirectory();
            Version osFullVer = Environment.OSVersion.Version;
            //int osBuild = Convert.ToInt32(osFullVer);
            if (osFullVer.Major == Convert.ToInt32(ConstantsDLL.Properties.Resources.WIN_7_NT_MAJOR) && osFullVer.Minor == Convert.ToInt32(ConstantsDLL.Properties.Resources.WIN_7_NT_MINOR))
            {
                return current + ConstantsDLL.Properties.Resources.RESOURCES_DIR + ConstantsDLL.Properties.Resources.IMG_DIR + ConstantsDLL.Properties.Resources.WIN_7_IMG_DIR;
            }
            else
            {
                return osFullVer.Build >= Convert.ToInt32(ConstantsDLL.Properties.Resources.WIN_10_NT_BUILD) && osFullVer.Build < Convert.ToInt32(ConstantsDLL.Properties.Resources.WIN_11_NT_BUILD)
                ? current + ConstantsDLL.Properties.Resources.RESOURCES_DIR + ConstantsDLL.Properties.Resources.IMG_DIR + ConstantsDLL.Properties.Resources.WIN_10_IMG_DIR
                : current + ConstantsDLL.Properties.Resources.RESOURCES_DIR + ConstantsDLL.Properties.Resources.IMG_DIR + ConstantsDLL.Properties.Resources.WIN_11_IMG_DIR;
            }
        }

        //Checks assembly version
        public static string Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        //Checks the current screen resolution
        public static bool ResolutionError(bool exit)
        {
            if (SystemParameters.PrimaryScreenWidth < Convert.ToInt32(ConstantsDLL.Properties.Resources.WIDTH) || SystemParameters.PrimaryScreenHeight < Convert.ToInt32(ConstantsDLL.Properties.Resources.HEIGHT))
            {
                _ = MessageBox.Show(string.Format(ConstantsDLL.Properties.Strings.RESOLUTION_WARNING, ConstantsDLL.Properties.Resources.WIDTH, ConstantsDLL.Properties.Resources.HEIGHT), ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                if (exit)
                {
                    File.Delete(StringsAndConstants.CREDENTIALS_FILE_PATH);
                    Application.Current.Shutdown();
                }
                return false;
            }
            return true;
        }

        //Checks if logfile exists
        public static string CheckIfLogExists(string path)
        {
            bool b;
            try
            {
#if DEBUG
                //Checks if log directory exists
                b = File.Exists(path + ConstantsDLL.Properties.Resources.LOG_FILENAME_FOP + "-v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version + "-" + Resources.dev_status + ConstantsDLL.Properties.Resources.LOG_FILE_EXT);
#else
                //Checks if log file exists
                b = File.Exists(path + ConstantsDLL.Properties.Resources.LOG_FILENAME_FOP + "-v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version + ConstantsDLL.Properties.Resources.LOG_FILE_EXT);
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
