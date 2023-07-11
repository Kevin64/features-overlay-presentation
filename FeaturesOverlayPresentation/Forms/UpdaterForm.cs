using FeaturesOverlayPresentation.Misc;
using FeaturesOverlayPresentation.Updater;
using LogGeneratorDLL;
using System;
using System.Windows.Forms;

namespace FeaturesOverlayPresentation.Forms
{
    /// <summary> 
    /// Class for handling Updater window
    /// </summary>
    public partial class UpdaterForm : Form
    {
        private readonly string currentVersion, newVersion, changelog, url;
        private readonly LogGenerator log;

        /// <summary> 
        /// Updater form constructor
        /// </summary>
        /// <param name="log">Log file object</param>
        /// <param name="ui">GitHub release information</param>
        public UpdaterForm(LogGenerator log, UpdateInfo ui)
        {
            InitializeComponent();

            if (ui != null)
            {
                newVersion = ui.TagName;
                changelog = ui.Body;
                url = ui.HtmlUrl;
            }
            currentVersion = MiscMethods.Version;
            this.log = log;
        }

        /// <summary> 
        /// Compares versions and sets labels if there is an update
        /// </summary>
        /// <returns>True is there is a new version, false otherwise</returns>
        public bool IsThereANewVersion()
        {
            lblOldVersion.Text = currentVersion;
            lblNewVersion.Text = newVersion;
            switch (newVersion.CompareTo(currentVersion))
            {
                case 1:
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), ConstantsDLL.Properties.Strings.NEW_VERSION_AVAILABLE, newVersion, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    lblUpdateAnnoucement.Text = ConstantsDLL.Properties.Strings.NEW_VERSION_AVAILABLE;
                    changelogTextBox.Text = changelog;
                    lblFixedNewVersion.Visible = true;
                    lblNewVersion.Visible = true;
                    downloadButton.Visible = true;
                    return true;
                default:
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), ConstantsDLL.Properties.Strings.NO_NEW_VERSION_AVAILABLE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    lblUpdateAnnoucement.Text = ConstantsDLL.Properties.Strings.NO_NEW_VERSION_AVAILABLE;
                    changelogTextBox.Text = changelog;
                    lblFixedNewVersion.Visible = false;
                    lblNewVersion.Visible = false;
                    downloadButton.Visible = false;
                    return false;
            }
        }

        /// <summary> 
        /// Opens the GitHub url in the browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_OPENING_GITHUB, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            _ = System.Diagnostics.Process.Start(url);
        }

        /// <summary> 
        /// Closes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary> 
        /// Handles the closing of the current form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdaterCheckerForm_Closing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_CLOSING_UPDATER_FORM, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
        }

        /// <summary> 
        /// Loads the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateCheckerForm_Load(object sender, EventArgs e)
        {
            FormClosing += UpdaterCheckerForm_Closing;
        }
    }
}
