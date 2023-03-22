using ConstantsDLL;
using System;
using System.IO;
using System.Windows;

namespace FeaturesOverlayPresentation
{
    /// <summary>
    /// Interação lógica para ReinstallError.xam
    /// </summary>
    public partial class ReinstallError : Window
    {
        public ReinstallError()
        {
            InitializeComponent();
        }

        private void CloseAppButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(ConstantsDLL.Properties.Resources.loginPath);
            Application.Current.Shutdown();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            File.Delete(ConstantsDLL.Properties.Resources.loginPath);
            Application.Current.Shutdown();
        }
    }
}
