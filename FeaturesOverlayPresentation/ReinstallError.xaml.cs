using System;
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

        private void closeAppButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
