using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FeaturesOverlayPresentation
{
    /// <summary>
    /// Interação lógica para Ending.xam
    /// </summary>
    public partial class Ending : Page
    {
        public Ending()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // for .NET Core you need to add UseShellExecute = true
            // see https://docs.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
