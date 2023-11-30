using FeaturesOverlayPresentation.Forms;
using LogGeneratorDLL;
using Octokit;
using System;
using System.Net.Http;
using System.Windows.Forms;

namespace FeaturesOverlayPresentation.Updater
{
    /// <summary> 
    /// Template class for 'Updater'
    /// </summary>
    public class UpdateInfo
    {
        public string ETag { get; set; }
        public string TagName { get; set; }
        public string Body { get; set; }
        public string HtmlUrl { get; set; }
    }
    /// <summary> 
    /// Class for handling update checking tasks and UI
    /// </summary>
    internal static class UpdateChecker
    {
        private static HttpClient httpHeader;
        private static HttpRequestMessage request;
        private static HttpResponseMessage response;
        private static Release releases;
        private static UpdateInfo ui;

        /// <summary> 
        /// Checks for updates on GitHub repo
        /// </summary>
        /// <param name="client">Octokit GitHub object</param>
        /// <param name="log">Log file object</param>
        internal static async void Check(GitHubClient client, LogGenerator log)
        {
            try
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), ConstantsDLL.Properties.LogStrings.LOG_CONNECTING_GITHUB, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));

                httpHeader = new HttpClient();
                request = new HttpRequestMessage(HttpMethod.Head, ConstantsDLL.Properties.GenericResources.GITHUB_FOP_API_URL);
                request.Headers.Add("User-Agent", "Other");
                ui = Misc.MiscMethods.RegCheckUpdateData();
                if (ui != null)
                {
                    request.Headers.Add("If-None-Match", "\"" + ui.ETag + "\"");
                }
                response = await httpHeader.SendAsync(request);
                if (!((int)response.StatusCode).Equals(304))
                {
                    releases = await client.Repository.Release.GetLatest(ConstantsDLL.Properties.GenericResources.GITHUB_OWNER_FOP, ConstantsDLL.Properties.GenericResources.GITHUB_REPO_FOP);
                    ui = new UpdateInfo
                    {
                        ETag = response.Headers.ETag.ToString().Substring(3, response.Headers.ETag.ToString().Length - 4),
                        TagName = releases.TagName,
                        Body = releases.Body,
                        HtmlUrl = releases.HtmlUrl
                    };
                    Misc.MiscMethods.RegCreateUpdateData(ui);
                }
                else
                {
                    ui = new UpdateInfo
                    {
                        ETag = Misc.MiscMethods.RegCheckUpdateData().ETag,
                        TagName = Misc.MiscMethods.RegCheckUpdateData().TagName,
                        Body = Misc.MiscMethods.RegCheckUpdateData().Body,
                        HtmlUrl = Misc.MiscMethods.RegCheckUpdateData().HtmlUrl
                    };
                }


                UpdaterForm uForm = new UpdaterForm(log, ui);
                bool isNotUpdated = uForm.IsThereANewVersion();
                _ = uForm.ShowDialog();
            }
            catch (Exception e) when (e is ApiException || e is HttpRequestException)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), ConstantsDLL.Properties.LogStrings.LOG_GITHUB_UNREACHABLE, e.Message, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), ConstantsDLL.Properties.LogStrings.LOG_UPDATE_CHECK_IMPOSSIBLE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_GUI));
                _ = MessageBox.Show(ConstantsDLL.Properties.LogStrings.LOG_UPDATE_CHECK_IMPOSSIBLE, ConstantsDLL.Properties.UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
