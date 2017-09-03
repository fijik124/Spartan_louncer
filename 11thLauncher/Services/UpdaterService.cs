using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;
using Newtonsoft.Json;
using _11thLauncher.Accessors.Contracts;
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

        private readonly string _assemblyVersion;
        private string _lastEtag = string.Empty; //Latest entity tag for caching

        #endregion

        public UpdaterService(IFileAccessor fileAccessor, INetworkAccessor networkAccessor, ILogger logger) : base(logger)
        {
            _fileAccessor = fileAccessor;
            _networkAccessor = networkAccessor;

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            _assemblyVersion = string.Join(".", version.Major, version.Minor, version.Build);
        }

        #region Methods

        /// <summary>
        /// Use the GitHub API to check if there is a new release
        /// </summary>
        /// <returns>Result of the update check</returns>
        public UpdateCheckResult CheckUpdates()
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
                    GithubRelease release = JsonConvert.DeserializeObject<GithubRelease>(releaseStr);
                    var updated = false;

                    if (!string.IsNullOrEmpty(release.tag_name))
                    {
                        var latestVersion = release.tag_name;

                        if (latestVersion != null)
                        {
                            updated = latestVersion.Equals(string.Format(ApplicationConfig.GithubVersionTagFormat, _assemblyVersion));
                        }
                    }

                    _lastEtag = client.ResponseHeaders[HttpResponseHeader.ETag];

                    return updated ? UpdateCheckResult.UpdateAvailable : UpdateCheckResult.NoUpdateAvailable;
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
                                return UpdateCheckResult.ErrorRateExceeded;
                            }
                            break;

                        default:
                            Logger.LogException("UpdaterService", "Unexpected HTTP response received", new ArgumentOutOfRangeException(nameof(webException.StatusCode)));
                            break;
                    }

                    return UpdateCheckResult.ErrorCheckingUpdates;
                }
                catch (Exception e)
                {
                    Logger.LogException("UpdaterService", "Exception checking updates", e);
                    return UpdateCheckResult.ErrorCheckingUpdates;
                }
            }
        }

        /// <summary>
        /// Extract and execute external updater, then close the application
        /// </summary>
        public void ExecuteUpdater()
        {
            Logger.LogDebug("UpdaterService", "Preparing to run updater");

            //Extract updater
            _fileAccessor.WriteAllBytes(ApplicationConfig.UpdaterPath, Properties.Resources._11thLauncher_Updater);

            var appPath = Assembly.GetExecutingAssembly().Location;
            var fullPath = string.Empty;
            if (appPath != null)
            {
                fullPath = Path.GetFullPath(appPath);
            }

            //Execute updater
            //var p = new Process
            //{
            //StartInfo =
            //{
            //FileName = ApplicationConfig.UpdaterPath,
            //Arguments =
            //$"\"{fullPath}\"" + " " +
            //(ApplicationConfig.DownloadBaseUrl + "11thLauncher" + _latestVersion + ".zip")

            //}
            //};
            //p.Start();

            Logger.LogDebug("UpdaterService", "Updater started, shutting down...");

            Application.Current.Shutdown();
        }

        public void RemoveUpdater()
        {
            Logger.LogDebug("UpdaterService", "Deleting updater file");
            _fileAccessor.DeleteFile(ApplicationConfig.UpdaterPath);
        }

        #endregion
    }
}