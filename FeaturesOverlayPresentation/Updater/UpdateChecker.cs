using FeaturesOverlayPresentation.Forms;
using LogGeneratorDLL;
using Octokit;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FeaturesOverlayPresentation.Updater
{
    ///<summary>Class for handling update checking tasks and UI</summary>
    internal static class UpdateChecker
    {
        ///<summary>Checks for updates on GitHub repo</summary>
        ///<param name="parametersList">List containing data from [Parameters]</param>
        ///<param name="themeBool">Theme mode</param>
        internal static async void Check(LogGenerator log, bool autoCheck)
        {
            try
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_CONNECTING_GITHUB, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                GitHubClient client = new GitHubClient(new ProductHeaderValue(ConstantsDLL.Properties.Resources.GITHUB_REPO_FOP));
                Release releases = await client.Repository.Release.GetLatest(ConstantsDLL.Properties.Resources.GITHUB_OWNER_FOP, ConstantsDLL.Properties.Resources.GITHUB_REPO_FOP);

                UpdateCheckerForm uForm = new UpdateCheckerForm(log, releases);
                bool isUpdated = uForm.IsThereANewVersion();
                if((!isUpdated && !autoCheck) || isUpdated)
                {
                    _ = uForm.ShowDialog();
                }
            }
            catch (ApiException e)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ConstantsDLL.Properties.Strings.LOG_GITHUB_UNREACHABLE, e.Message, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                _ = MessageBox.Show(ConstantsDLL.Properties.Strings.LOG_UPDATE_CHECK_IMPOSSIBLE, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), ConstantsDLL.Properties.Strings.LOG_NO_INTERNET_AVAILABLE, e.Message, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                _ = MessageBox.Show(ConstantsDLL.Properties.Strings.LOG_NO_INTERNET_AVAILABLE, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
