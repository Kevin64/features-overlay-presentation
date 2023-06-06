using FeaturesOverlayPresentation.Properties;
using FeaturesOverlayPresentation.Updater;
using LogGeneratorDLL;
using System.Reflection;
using System.Windows.Forms;

namespace FeaturesOverlayPresentation.Forms
{
    ///<summary>Class for About box</summary>
    internal partial class AboutBox : Form
    {
        private readonly LogGenerator log;

        ///<summary>About form constructor</summary>
        public AboutBox(LogGenerator log)
        {
            this.log = log;
            InitializeComponent();
            Text = string.Format("{0} {1}", labelFormTitle.Text, AssemblyTitle);
            labelProductName.Text = AssemblyProduct;
#if DEBUG
            labelVersion.Text = string.Format("Versão {0}-{1}", AssemblyVersion, Resources.dev_status);
#else
            labelVersion.Text = string.Format("Versão {0}", AssemblyVersion);
#endif
            labelCopyright.Text = AssemblyCopyright;
            labelCompanyName.Text = AssemblyCompany;
            textBoxDescription.Text = Strings.description;
            textBoxDescription.LinkClicked += TextBoxDescription_LinkClicked;
        }

        ///<summary>Handles link clicks inside the Description box</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void TextBoxDescription_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            _ = System.Diagnostics.Process.Start(e.LinkText);
        }

        #region Acessório de Atributos do Assembly

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != string.Empty)
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        ///<summary>Triggers an update check</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void CheckUpdateButton_Click(object sender, System.EventArgs e)
        {
            UpdateChecker.Check(log, false);
        }
    }
}
