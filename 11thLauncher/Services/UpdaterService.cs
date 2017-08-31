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
    public class UpdaterService : IUpdaterService
    {
        private readonly IFileAccessor _fileAccessor;
        private readonly INetworkAccessor _networkAccessor;
        private readonly ILogger _logger;

        private readonly string _assemblyVersion;
        private string _lastEtag = ""; //Latest entity tag for caching

        public UpdaterService(IFileAccessor fileAccessor, INetworkAccessor networkAccessor, ILogger logger)
        {
            _fileAccessor = fileAccessor;
            _networkAccessor = networkAccessor;
            _logger = logger;

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            _assemblyVersion = string.Join(".", version.Major, version.Minor, version.Build);
        }

        /// <summary>
        /// Use the GitHub API to check if there is a new release
        /// </summary>
        /// <returns>Result of the update check</returns>
        public UpdateCheckResult CheckUpdates()
        {
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
                            _logger.LogException("UpdaterService", "Unexpected HTTP response received", new ArgumentOutOfRangeException());
                            break;
                    }

                    return UpdateCheckResult.ErrorCheckingUpdates;
                }
                catch (Exception)
                {
                    return UpdateCheckResult.ErrorCheckingUpdates;
                }
            }
        }

        /// <summary>
        /// Extract and execute external updater, then close the application
        /// </summary>
        public void ExecuteUpdater()
        {
            //Extract updater
            _fileAccessor.WriteAllBytes(ApplicationConfig.UpdaterPath, Properties.Resources._11thLauncher_Updater);

            var appPath = Assembly.GetExecutingAssembly().Location;
            var fullPath = "";
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

            Application.Current.Shutdown();
        }

        public void RemoveUpdater()
        {
            _fileAccessor.DeleteFile(ApplicationConfig.UpdaterPath);
        }
    }
}
