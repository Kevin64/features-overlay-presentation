using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

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
            if (Environment.OSVersion.Version.ToString().Contains(StringsAndConstants.win7nt))
                return current + StringsAndConstants.win7imgDir;
            else
                return current + StringsAndConstants.win10imgDir;
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

        public static void resolutionError()
        {
            if (SystemParameters.PrimaryScreenWidth < 1280 || SystemParameters.PrimaryScreenHeight < 720)
            {
                MessageBox.Show(StringsAndConstants.resolutionWarning, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                File.Delete(StringsAndConstants.loginPath);
                Application.Current.Shutdown();
            }
        }
    }
}
