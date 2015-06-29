using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using _11thLauncher.Configuration;

namespace _11thLauncher.Net
{
    static class Repository
    {
        public static string JavaVersion = "";

        private static readonly string _a3sdsPath = Path.Combine(Path.GetTempPath(), "A3SDS.jar");

        private static string _localRevision;

        private static string _remoteLogin;
        private static string _remotePassword;
        private static string _remoteURL;
        private static string _remotePort;
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
                        repositories.Add(fileName.Substring(0, fileName.IndexOf('.')));
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
            File.WriteAllBytes(_a3sdsPath, Properties.Resources.A3SDS);

            deserializeLocalRepository();
            deserializeRemoteRepository();

            //Delete A3SDS
            File.Delete(_a3sdsPath);

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
                        updated = false;
                    }
                }
            }

            MainWindow.UpdateForm("UpdateRepositoryStatus", new object[] { revision, _remoteBuildDate, updated });
        }

        private static void deserializeLocalRepository()
        {
            string repositoryPath = "\"" + Settings.Arma3SyncPath + "\\resources\\ftp\\" + Settings.Arma3SyncRepository + ".a3s.repository" + "\"";

            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = GetJavaPath();
            p.StartInfo.Arguments = " -jar " + _a3sdsPath + " -deserializeRepository " + repositoryPath;
            p.Start();
            string localRepository = p.StandardOutput.ReadToEnd();
            string[] localRepositoryInfo = localRepository.TrimEnd(new char[] { '\r', '\n' }).Split(',');
            p.WaitForExit();

            if (localRepositoryInfo != null && localRepositoryInfo.Length == 6)
            {
                _localRevision = localRepositoryInfo[1];
                _remoteLogin = localRepositoryInfo[2];
                _remotePassword = localRepositoryInfo[3];
                _remotePort = localRepositoryInfo[4];
                _remoteURL = localRepositoryInfo[5];
            }
        }

        private static void deserializeRemoteRepository()
        {
            try
            {
                string tempPath = Path.GetTempPath() + "repoInfo";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + _remoteURL + "/.a3s/serverinfo");
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
                p.StartInfo.Arguments = " -jar " + _a3sdsPath + " -deserializeServerInfo \"" + tempPath + "\"";
                p.Start();
                string remoteRepository = p.StandardOutput.ReadToEnd();
                string[] remoteRepositoryInfo = remoteRepository.TrimEnd(new char[] { '\r', '\n' }).Split(',');
                p.WaitForExit();

                //Delete temp file
                File.Delete(tempPath);

                if (remoteRepositoryInfo != null && remoteRepositoryInfo.Length == 2)
                {
                    _remoteRevision = remoteRepositoryInfo[0];
                    _remoteBuildDate = JavaDateToDatetime(remoteRepositoryInfo[1]);
                }
            }
            catch (WebException){}
        }

        /// <summary>
        /// Check if Java is present in the system PATH variable
        /// </summary>
        public static void CheckJava()
        {
            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "java";
                p.StartInfo.Arguments = "-version";
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
            Process p = new Process();
            p.StartInfo.WorkingDirectory = Settings.Arma3SyncPath;
            p.StartInfo.FileName = Settings.Arma3SyncPath + "\\ArmA3Sync.exe";
            p.Start();
        }

        /// <summary>
        /// Get the Java execution path
        /// </summary>
        /// <returns>Java execution path</returns>
        private static string GetJavaPath()
        {
            string path = "";

            if (Settings.JavaPath != "")
            {
                path = Settings.JavaPath;
            } else
            {
                path = "java";
            }

            return path;
        }

        /// <summary>
        /// Convert a Java long date string to C# DateTime
        /// </summary>
        /// <param name="date">Java long date string</param>
        /// <returns>Converted DateTime</returns>
        private static DateTime JavaDateToDatetime(string date)
        {
            long dateLong = long.Parse(date);
            TimeSpan ss = TimeSpan.FromMilliseconds(dateLong);
            DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime ddd = Jan1st1970.Add(ss);
            DateTime final = ddd.ToUniversalTime();

            return final;
        }
    }
}
