using System;
using System.IO;
using System.Windows;
using ConstantsDLL;

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
            File.Delete(StringsAndConstants.loginPath);
            Application.Current.Shutdown();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            File.Delete(StringsAndConstants.loginPath);
            Application.Current.Shutdown();
        }
    }
}
