using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using ConstantsDLL;
using FeaturesOverlayPresentation.Properties;

namespace FeaturesOverlayPresentation
{
    internal static class MiscMethods
    {
        public static bool regCheck()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(StringsAndConstants.FopRegKey);
            string k = rk.GetValue(StringsAndConstants.DidItRunAlready).ToString();
            if (k.Equals("1"))
                return true;
            else
                return false;
        }

        public static void regRecreate(bool empty)
        {
            if (!empty && !regCheck())
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(StringsAndConstants.FopRunOnceKey, true);
                if (!key.GetValueNames().Contains(StringsAndConstants.FOP))
                {
                    if (Environment.Is64BitOperatingSystem == true)
                        key.SetValue(StringsAndConstants.FOP, StringsAndConstants.FOPx86, RegistryValueKind.String);
                    else
                        key.SetValue(StringsAndConstants.FOP, StringsAndConstants.FOPx64, RegistryValueKind.String);
                }
            }
        }

        public static void regDelete()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(StringsAndConstants.FopRunOnceKey, true);
            if (key.GetValueNames().Contains(StringsAndConstants.FOP))
                key.DeleteValue(StringsAndConstants.FOP);
        }

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

        //Generates a MD5 hash from an input
        public static string HashMd5Generator(string input)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));
            return sBuilder.ToString();
        }

        public static string Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

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
