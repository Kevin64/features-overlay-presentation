using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;

namespace FeaturesOverlayPresentation
{
    internal static class Utils
    {
        public static bool regCheck()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"Software\FOP");
            string k = rk.GetValue("DidItRunAlready").ToString();
            if (k.Equals("1"))
                return true;
            else
                return false;
        }

        public static void regRecreate(bool empty)
        {
            if (!empty && !regCheck())
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\RunOnce", true);
                if (!key.GetValueNames().Contains("FOP"))
                {
                    if (Environment.Is64BitOperatingSystem == true)
                        key.SetValue("FOP", Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%") + "\\FOP" + "\\Rever tutorial de uso do computador.lnk", RegistryValueKind.String);
                    else
                        key.SetValue("FOP", Environment.ExpandEnvironmentVariables("%ProgramFiles%") + "\\FOP" + "\\Rever tutorial de uso do computador.lnk", RegistryValueKind.String);
                }
            }
        }

        public static void regDelete()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\RunOnce", true);
            if (key.GetValueNames().Contains("FOP"))
                key.DeleteValue("FOP");
        }

        public static string OSCheck()
        {
            string current = Directory.GetCurrentDirectory();
            if (Environment.OSVersion.Version.ToString().Contains("6.1"))
                return current + "\\img-windows7\\";
            else
                return current + "\\img-windows10\\";
        }

        public static string Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public static bool PingHost(string servidor_web)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            if (servidor_web == "")
                return false;
            try
            {
                PingReply reply = pinger.Send(servidor_web);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
            }
            return pingable;
        }

        public static void resolutionError()
        {
            if (SystemParameters.PrimaryScreenWidth < 1280 || SystemParameters.PrimaryScreenHeight < 720)
            {
                MessageBox.Show("Resolução insuficiente. Este programa suporta apenas resoluções iguais ou superiores a 1280x720.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }
    }
}
