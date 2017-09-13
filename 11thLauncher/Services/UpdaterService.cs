using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using _11thLauncher.Accessors.Contracts;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;
using _11thLauncher.Util;

namespace _11thLauncher.Services
{
    public class UpdaterService : AbstractService, IUpdaterService
    {
        #region Fields

        private readonly IFileAccessor _fileAccessor;
        private readonly INetworkAccessor _networkAccessor;
        private readonly IProcessAccessor _processAccessor;
        private readonly IEventAggregator _eventAggregator;

        private readonly string _assemblyVersion;

        //Request caching
        private string _lastEtag = string.Empty; //Latest entity tag for caching
        private GithubRelease _release;
        private bool _updated;

        #endregion

        public UpdaterService(IFileAccessor fileAccessor, INetworkAccessor networkAccessor, IProcessAccessor processAccessor, ILogger logger, IEventAggregator eventAggregator) 
            : base(logger)
        {
            _fileAccessor = fileAccessor;
            _networkAccessor = networkAccessor;
            _processAccessor = processAccessor;
            _eventAggregator = eventAggregator;

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            _assemblyVersion = string.Join(".", version.Major, version.Minor, version.Build);
        }

        #region Methods

        /// <summary>
        /// Use the GitHub API to check if there is a new release
        /// </summary>
        /// <returns>Result of the update check</returns>
        public void CheckUpdates(bool manual)
        {
            Logger.LogDebug("UpdaterService", "Checking application updates");

            using (WebClient client = new WebClient())
            {
                client.UseDefaultCredentials = true;
                client.Headers.Add(HttpRequestHeader.Accept, ApplicationConfig.GithubApiCurrentVersion);
                client.Headers.Add(HttpRequestHeader.IfNoneMatch, _lastEtag);
                client.Headers.Add(HttpRequestHeader.UserAgent, ApplicationConfig.UpdaterServiceUserAgent);

                try
                {
                    string releaseStr = _networkAccessor.DownloadString(client, ApplicationConfig.GithubApiReleaseEndpoint);
                    _release = JsonConvert.DeserializeObject<GithubRelease>(releaseStr);
                    _updated = false;

                    if (!string.IsNullOrEmpty(_release.tag_name))
                    {
                        var latestVersion = _release.tag_name;

                        if (latestVersion != null)
                        {
                            _updated = latestVersion.Equals(string.Format(ApplicationConfig.GithubVersionTagFormat, _assemblyVersion));
                        }
                    }

                    _lastEtag = client.ResponseHeaders[HttpResponseHeader.ETag];

                    NotifyUpdates(_updated ? UpdateCheckResult.UpdateAvailable : UpdateCheckResult.NoUpdateAvailable, manual);
                }
                catch (WebException e)
                {
                    HttpWebResponse webException = e.Response as HttpWebResponse;
                    if (webException == null) throw;

                    switch (webException.StatusCode)
                    {
                        case HttpStatusCode.Forbidden:
                            int rateRemaining;
                            var rateHeader = int.TryParse(e.Response.Headers["X-RateLimit-Remaining"], out rateRemaining);
                            if (rateHeader && rateRemaining == 0)
                            {
                                Logger.LogInfo("UpdaterService", "The rate limit for the GitHub API has been reached");
                                NotifyUpdates(UpdateCheckResult.ErrorRateExceeded, manual);
                                return;
                            }
                            break;

                        case HttpStatusCode.NotModified:
                            NotifyUpdates(_updated ? UpdateCheckResult.UpdateAvailable : UpdateCheckResult.NoUpdateAvailable, manual);
                            return;

                        default:
                            Logger.LogException("UpdaterService", "Unexpected HTTP response received", new ArgumentOutOfRangeException(nameof(webException.StatusCode)));
                            break;
                    }

                    NotifyUpdates(UpdateCheckResult.ErrorCheckingUpdates, manual);
                }
                catch (Exception e)
                {
                    Logger.LogException("UpdaterService", "Exception checking updates", e);
                    NotifyUpdates(UpdateCheckResult.ErrorCheckingUpdates, manual);
                }
            }
        }

        public void RemoveUpdater()
        {
            Logger.LogDebug("UpdaterService", "Deleting updater file");
            _fileAccessor.DeleteFile(ApplicationConfig.UpdaterPath);
        }

        private void NotifyUpdates(UpdateCheckResult updateCheckResult, bool manual)
        {
            if (updateCheckResult.Equals(UpdateCheckResult.UpdateAvailable))
            {
                //Ask user if he wants to update
                _eventAggregator.PublishOnUIThreadAsync(new ShowQuestionDialogMessage
                {
                    Title = Resources.Strings.S_MSG_UPDATE_TITLE,
                    Content = Resources.Strings.S_MSG_UPDATE_CONTENT,
                    Callback = r =>
                    {
                        if (r.Equals(MessageDialogResult.Affirmative))
                        {
                            RunUpdater();
                        }
                    }
                });
            }
            else if (manual)
            {
                if (updateCheckResult.Equals(UpdateCheckResult.NoUpdateAvailable))
                {
                    //Notify no updates found
                    _eventAggregator.PublishOnUIThreadAsync(new ShowDialogMessage
                    {
                        Title = Resources.Strings.S_MSG_NO_UPDATES_TITLE,
                        Content = Resources.Strings.S_MSG_NO_UPDATES_CONTENT
                    });
                }
                else
                {
                    //Notify error while checking updates
                    _eventAggregator.PublishOnUIThreadAsync(new ShowDialogMessage
                    {
                        Title = Resources.Strings.S_MSG_UPDATE_ERROR_TITLE,
                        Content = Resources.Strings.S_MSG_UPDATE_ERROR_CONTENT
                    });
                }
            }
        }

        /// <summary>
        /// Extract and execute external updater, then close the application
        /// </summary>
        private void RunUpdater()
        {
            Logger.LogDebug("UpdaterService", "Preparing to run updater");

            try
            {
                //Extract updater
                _fileAccessor.WriteAllBytes(ApplicationConfig.UpdaterPath, Properties.Resources._11thLauncher_Updater);

                var appPath = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
                var downloadUrl = _release.assets.First().browser_download_url;
                var hashUrl = _release.assets.ElementAt(1).browser_download_url;

                //Execute updater
                var p = new Process
                {
                    StartInfo =
                    {
                        FileName = ApplicationConfig.UpdaterPath,
                        Arguments = string.Concat("\"", appPath, "\" ", downloadUrl, " ", hashUrl)
                    }
                };
                _processAccessor.Start(p);
            }
            catch (Exception e)
            {
                Logger.LogException("UpdaterService", "Error trying to launch updater", e);
                return;
            }

            Logger.LogDebug("UpdaterService", "Updater started, shutting down...");

            Application.Current.Shutdown();
        }

        #endregion
    }
}