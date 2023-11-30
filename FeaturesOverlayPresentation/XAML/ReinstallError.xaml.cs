using System;
using System.Windows;

namespace FeaturesOverlayPresentation.XAML
{
    /// <summary> 
    /// Class for ReinstallError.xaml
    /// </summary>
    public partial class ReinstallError : Window
    {
        /// <summary> 
        /// ReinstallError constructor
        /// </summary>
        public ReinstallError()
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Handles when user closes the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseAppButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Handles when program window closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
