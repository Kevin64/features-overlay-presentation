using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using ConstantsDLL;
using FeaturesOverlayPresentation.Properties;

namespace FeaturesOverlayPresentation
{
    internal static class MiscMethods
    {
        //Checks via registry if the program was already executed
        public static bool regCheck()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(StringsAndConstants.FopRegKey);
            string k = rk.GetValue(StringsAndConstants.DidItRunAlready).ToString();
            if (k.Equals("1"))
                return true;
            else
                return false;
        }

        //Creates RunOnce and FOP registry keys
        public static void regCreate()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(StringsAndConstants.FopRunOnceKey, true);
            if (Environment.Is64BitOperatingSystem)
                key.SetValue(StringsAndConstants.FOP, StringsAndConstants.FOPx86, RegistryValueKind.String);
            else
                key.SetValue(StringsAndConstants.FOP, StringsAndConstants.FOPx64, RegistryValueKind.String);
            RegistryKey key2 = Registry.CurrentUser.CreateSubKey(StringsAndConstants.FopRegKey);
            key2.SetValue(StringsAndConstants.DidItRunAlready, 0, RegistryValueKind.DWord);
        }

        //ReCreates RunOnce registry key
        public static void regRecreate(bool empty)
        {
            if (!empty && !regCheck())
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(StringsAndConstants.FopRunOnceKey, true);
                if (!key.GetValueNames().Contains(StringsAndConstants.FOP))
                {
                    if (Environment.Is64BitOperatingSystem)
                        key.SetValue(StringsAndConstants.FOP, StringsAndConstants.FOPx86, RegistryValueKind.String);
                    else
                        key.SetValue(StringsAndConstants.FOP, StringsAndConstants.FOPx64, RegistryValueKind.String);
                }
            }
        }

        //Deletes RunOnce registry key
        public static void regDelete()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(StringsAndConstants.FopRunOnceKey, true);
            if (key.GetValueNames().Contains(StringsAndConstants.FOP))
                key.DeleteValue(StringsAndConstants.FOP);
        }

        //Checks OS version
        public static string OSCheck()
        {
            string current = Directory.GetCurrentDirectory();
            Version osFullVer = Environment.OSVersion.Version;
            //int osBuild = Convert.ToInt32(osFullVer);
            if (osFullVer.Major == Convert.ToInt32(StringsAndConstants.win7ntMajor) && osFullVer.Minor == Convert.ToInt32(StringsAndConstants.win7ntMinor))
                return current + StringsAndConstants.win7imgDir;
            else if (osFullVer.Build >= Convert.ToInt32(StringsAndConstants.win10ntBuild) && osFullVer.Build < Convert.ToInt32(StringsAndConstants.win11ntBuild))
                return current + StringsAndConstants.win10imgDir;
            else
                return current + StringsAndConstants.win11imgDir;
        }

        //Checks assembly version
        public static string Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        //Checks the current screen resolution
        public static bool resolutionError(bool exit)
        {
            if (SystemParameters.PrimaryScreenWidth < StringsAndConstants.Width || SystemParameters.PrimaryScreenHeight < StringsAndConstants.Height)
            {
                MessageBox.Show(StringsAndConstants.resolutionWarning, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                if(exit)
                {
                    File.Delete(StringsAndConstants.loginPath);
                    Application.Current.Shutdown();
                }
                return false;
            }
            return true;
        }

        //Checks if logfile exists
        public static string checkIfLogExists(string path)
        {
            bool b;
            try
            {
#if DEBUG
                //Checks if log directory exists
                b = File.Exists(path + StringsAndConstants.LOG_FILENAME_OOBE + "-v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version + "-" + Resources.dev_status + StringsAndConstants.LOG_FILE_EXT);
#else
                //Checks if log file exists
                b = File.Exists(path + StringsAndConstants.LOG_FILENAME_OOBE + "-v" + Application.Current.MainWindow.GetType().Assembly.GetName().Version + StringsAndConstants.LOG_FILE_EXT);
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
