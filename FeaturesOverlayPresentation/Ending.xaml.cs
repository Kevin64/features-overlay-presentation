using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FeaturesOverlayPresentation
{
    public partial class Ending : Page
    {
        public Ending()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
