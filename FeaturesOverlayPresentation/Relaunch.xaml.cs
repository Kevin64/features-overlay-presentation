using Microsoft.Win32;
using System;
using System.Windows;

namespace FeaturesOverlayPresentation
{
    /// <summary>
    /// Lógica interna para Relaunch.xaml
    /// </summary>
    public partial class Relaunch : Window
    {
        public Relaunch()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\FOP");
            if (int.Parse(key.GetValue("DidItRunAlready").ToString()).Equals(0))
            {
                MainWindow m = new MainWindow();
                m.Show();
            }
            else
            {
                InitializeComponent();
            }            
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow m = new MainWindow();
            m.Show();
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void YesLaterButton_Click(object sender, RoutedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\RunOnce");
            key.SetValue("FOP", "C:\\Program Files (x86)\\FOP\\Rever tutorial de uso do computador.lnk");
            RegistryKey key2 = Registry.CurrentUser.CreateSubKey(@"Software\FOP");
            key2.SetValue("DidItRunAlready", 0, RegistryValueKind.DWord);
            Environment.Exit(0);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
