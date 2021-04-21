using System;
using System.Windows;

namespace FeaturesOverlayPresentation
{
    public partial class Error : Window
    {
        public Error()
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
