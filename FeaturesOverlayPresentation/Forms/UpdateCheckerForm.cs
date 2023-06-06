using ConstantsDLL;
using FeaturesOverlayPresentation.Misc;
using LogGeneratorDLL;
using Octokit;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FeaturesOverlayPresentation.Forms
{
    ///<summary>Class for handling Updater window</summary>
    public partial class UpdateCheckerForm : Form
    {
        private readonly string currentVersion, newVersion, changelog, url;
        private readonly LogGenerator log;

        ///<summary>Updater form constructor</summary>
        ///<param name="parametersList">List containing data from [Parameters]</param>
        ///<param name="themeBool">Theme mode</param>
        ///<param name="releases">GitHub release information</param>
        public UpdateCheckerForm(LogGenerator log, Release releases)
        {
            InitializeComponent();

            currentVersion = MiscMethods.Version;
            newVersion = releases.TagName;
            changelog = releases.Body;
            url = releases.HtmlUrl;
            this.log = log;
        }

        ///<summary>Compares versions and sets labels if there is an update</summary>
        ///<returns>True is there is a new version, false otherwise</returns>
        public bool IsThereANewVersion()
        {
            lblOldVersion.Text = currentVersion;
            lblNewVersion.Text = newVersion;
            switch (newVersion.CompareTo(currentVersion))
            {
                case 1:
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_CHECKING_FOR_UPDATES, ConstantsDLL.Properties.Strings.NEW_VERSION_AVAILABLE, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    lblUpdateAnnoucement.Text = ConstantsDLL.Properties.Strings.NEW_VERSION_AVAILABLE;
                    changelogTextBox.Text = changelog;
                    lblFixedNewVersion.Visible = true;
                    lblNewVersion.Visible = true;
                    downloadButton.Visible = true;
                    return true;
                default:
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_CHECKING_FOR_UPDATES, ConstantsDLL.Properties.Strings.NO_VERSION_AVAILABLE, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    lblUpdateAnnoucement.Text = ConstantsDLL.Properties.Strings.NO_VERSION_AVAILABLE;
                    changelogTextBox.Text = changelog;
                    lblFixedNewVersion.Visible = false;
                    lblNewVersion.Visible = false;
                    downloadButton.Visible = false;
                    return false;
            }
        }

        ///<summary>Opens the GitHub url in the browser</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void downloadButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_OPENING_GITHUB, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            _ = System.Diagnostics.Process.Start(url);
        }

        ///<summary>Closes the window</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_CLOSING_UPDATER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            Close();
        }
    }
}
