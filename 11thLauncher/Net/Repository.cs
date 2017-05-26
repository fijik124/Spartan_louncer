using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using _11thLauncher.Configuration;
using _11thLauncher;

namespace _11thLauncher.Net
{
    public static class Repository
    {
        public static string JavaVersion = "";

        private static string _localRevision;

        private static string _remoteLogin;
        private static string _remotePassword;
        private static string _remoteUrl;
        private static string _remoteRevision;
        private static DateTime _remoteBuildDate;

        /// <summary>
        /// Get a list of repositories from the Arma3Sync ftp folder
        /// </summary>
        /// <returns>List of current repositories, empty list if the Arma3Sync path is not configured</returns>
        public static List<string> ListRepositories()
        {
            List<string> repositories = new List<string>();

            if (Settings.Arma3SyncPath != "")
            {
                try
                {
                    string[] files = Directory.GetFiles(Settings.Arma3SyncPath + "\\resources\\ftp\\");
                    foreach (string file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        if (fileName != null) repositories.Add(fileName.Substring(0, fileName.IndexOf('.')));
                    }
                }
                catch (Exception) {}
            }

            return repositories;
        }

        /// <summary>
        /// Check the selected local repository
        /// </summary>
        public static void CheckRepository()
        {
            MainWindow.UpdateForm("UpdateStatusBar", new object[] { "Comprobando repositorio" });

            //Extract A3SDS
            File.WriteAllBytes(Constants.A3SdsPath, Properties.Resources.A3SDS);

            deserializeLocalRepository();
            deserializeRemoteRepository();

            //Delete A3SDS
            File.Delete(Constants.A3SdsPath);

            string revision = _localRevision;
            bool updated = false;

            if (_localRevision != null && MainWindow.Form != null)
            {
                if (_remoteRevision != null)
                {
                    if (_localRevision.Equals(_remoteRevision))
                    {
                        updated = true;
                    }
                    else
                    {
                        revision = _remoteRevision;
                    }
                }
            }

            MainWindow.UpdateForm("UpdateRepositoryStatus", new object[] { revision, _remoteBuildDate, updated });
        }

        private static void deserializeLocalRepository()
        {
            string repositoryPath = "\"" + Settings.Arma3SyncPath + "\\resources\\ftp\\" + Settings.Arma3SyncRepository + ".a3s.repository" + "\"";

            Process p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = GetJavaPath(),
                    Arguments = " -jar " + Constants.A3SdsPath + " -deserializeRepository " + repositoryPath
                }
            };
            p.Start();
            string localRepository = p.StandardOutput.ReadToEnd();
            string[] localRepositoryInfo = localRepository.TrimEnd('\r', '\n').Split(',');
            p.WaitForExit();

            if (localRepositoryInfo.Length == 6)
            {
                _localRevision = localRepositoryInfo[1];
                _remoteLogin = localRepositoryInfo[2];
                _remotePassword = localRepositoryInfo[3];
                _remoteUrl = localRepositoryInfo[5];
            }
        }

        private static void deserializeRemoteRepository()
        {
            try
            {
                string tempPath = Path.GetTempPath() + "repoInfo";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + _remoteUrl + "/.a3s/serverinfo");
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(_remoteLogin, _remotePassword);

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                using (Stream s = File.Create(tempPath))
                {
                    responseStream.CopyTo(s);
                }

                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = @GetJavaPath();
                p.StartInfo.Arguments = " -jar " + Constants.A3SdsPath + " -deserializeServerInfo \"" + tempPath + "\"";
                p.Start();
                string remoteRepository = p.StandardOutput.ReadToEnd();
                string[] remoteRepositoryInfo = remoteRepository.TrimEnd('\r', '\n').Split(',');
                p.WaitForExit();

                //Delete temp file
                File.Delete(tempPath);

                if (remoteRepositoryInfo != null && remoteRepositoryInfo.Length == 2)
                {
                    _remoteRevision = remoteRepositoryInfo[0];
                    _remoteBuildDate = JavaDateToDatetime(remoteRepositoryInfo[1]);
                }
            }
            catch (WebException) {}
        }

        /// <summary>
        /// Check if Java is present in the system PATH variable
        /// </summary>
        public static void CheckJava()
        {
            try
            {
                var p = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        FileName = "java",
                        Arguments = "-version"
                    }
                };
                p.Start();
                JavaVersion = p.StandardError.ReadLine();
                p.StandardError.ReadToEnd();
                p.WaitForExit();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Start ArmA3Sync in the configured path
        /// </summary>
        public static void StartArmA3Sync()
        {
            var p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = Settings.Arma3SyncPath,
                    FileName = Settings.Arma3SyncPath + "\\ArmA3Sync.exe"
                }
            };
            p.Start();
        }

        /// <summary>
        /// Get the Java execution path
        /// </summary>
        /// <returns>Java execution path</returns>
        private static string GetJavaPath()
        {
            var path = Settings.JavaPath != "" ? Settings.JavaPath : "java";

            return path;
        }

        /// <summary>
        /// Convert a Java long date string to C# DateTime
        /// </summary>
        /// <param name="date">Java long date string</param>
        /// <returns>Converted DateTime</returns>
        private static DateTime JavaDateToDatetime(string date)
        {
            var dateLong = long.Parse(date);
            var ss = TimeSpan.FromMilliseconds(dateLong);
            var jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var ddd = jan1St1970.Add(ss);
            var final = ddd.ToUniversalTime();

            return final;
        }
    }
}
