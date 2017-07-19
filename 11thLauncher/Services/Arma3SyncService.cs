using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Caliburn.Micro;
using Microsoft.Win32;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.Services
{
    public class Arma3SyncService : IAddonSyncService
    {
        public BindableCollection<Repository> ReadRepositories(string arma3SyncPath)
        {
            BindableCollection<Repository> repositories = new BindableCollection<Repository>();
            if (!Directory.Exists(arma3SyncPath)) return repositories;

            string[] files = Directory.GetFiles(Path.Combine(arma3SyncPath, Constants.Arma3SyncConfigFolder));
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                if (fileName != null) repositories.Add(new Repository
                {
                    Name = fileName.Substring(0, fileName.IndexOf('.')),
                    Path = file
                });
            }

            return repositories;
        }

        public void CheckRepository(string arma3SyncPath, string javaPath, Repository repository)
        {
            repository.Status = RepositoryStatus.Checking;

            //Extract A3SDS
            File.WriteAllBytes(Constants.A3SdsPath, Properties.Resources.A3SDS);

            DeserializeLocalRepository(arma3SyncPath, javaPath, repository);
            DeserializeRemoteRepository(javaPath, repository);

            //Delete A3SDS
            File.Delete(Constants.A3SdsPath);

            if (repository.LocalRevision != null)
            {
                if (repository.RemoteRevision != null)
                {
                    if (repository.LocalRevision.Equals(repository.RemoteRevision))
                    {
                        repository.Status = RepositoryStatus.Updated;
                    }
                    else
                    {
                        repository.Status = RepositoryStatus.Outdated;
                    }
                }
            }
        }

        private static void DeserializeLocalRepository(string arma3SyncPath, string javaPath, Repository repository)
        {
            string repositoryPath = Path.Combine(arma3SyncPath, Constants.Arma3SyncConfigFolder, repository.Name + Constants.Arma3SyncRepositoryExtension);

            Process p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = !string.IsNullOrEmpty(javaPath) ? javaPath : Constants.JavaPathCommand,
                    Arguments = " -jar " + Constants.A3SdsPath + " -deserializeRepository \"" + repositoryPath + "\""
                }
            };
            p.Start();
            string localRepository = p.StandardOutput.ReadToEnd();
            string[] localRepositoryInfo = localRepository.TrimEnd('\r', '\n').Split(',');
            p.WaitForExit();

            if (localRepositoryInfo.Length != 6) return;

            repository.LocalRevision = localRepositoryInfo[1];
            repository.Login = localRepositoryInfo[2];
            repository.Password = localRepositoryInfo[3];
            repository.Address = localRepositoryInfo[5];
        }

        private static void DeserializeRemoteRepository(string javaPath, Repository repository)
        {
            try
            {
                string tempPath = Path.GetTempPath() + repository.Name + "repoInfo";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format(Constants.Arma3SyncRemoteServerInfo, repository.Address));
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(repository.Login, repository.Password);

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                using (Stream s = File.Create(tempPath))
                {
                    responseStream?.CopyTo(s);
                }

                Process p = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        FileName = !string.IsNullOrEmpty(javaPath) ? javaPath : Constants.JavaPathCommand,
                        Arguments = " -jar " + Constants.A3SdsPath + " -deserializeServerInfo \"" + tempPath + "\""
                    }
                };
                p.Start();
                string remoteRepository = p.StandardOutput.ReadToEnd();
                string[] remoteRepositoryInfo = remoteRepository.TrimEnd('\r', '\n').Split(',');
                p.WaitForExit();

                //Delete temp file
                File.Delete(tempPath);

                if (remoteRepositoryInfo != null && remoteRepositoryInfo.Length == 2)
                {
                    repository.RemoteRevision = remoteRepositoryInfo[0];
                    repository.RemoteBuildDate = JavaDateToDatetime(remoteRepositoryInfo[1]);
                }
            }
            catch (WebException) { }
        }

        public string GetJavaInSystem()
        {
            var javaVersion = "";
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
                javaVersion = p.StandardError.ReadLine();
                p.StandardError.ReadToEnd();
                p.WaitForExit();
            }
            catch (Exception) { }

            return javaVersion;
        }

        public string GetAddonSyncPath()
        {
            string arma3SyncPath = "";

            if (Environment.Is64BitOperatingSystem)
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(Constants.Arma3SyncBaseRegistryPath64))
                {
                    arma3SyncPath = SearchRegistryInstallations(key);
                }
            }
            else
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(Constants.Arma3SyncBaseRegistryPath32))
                {
                    arma3SyncPath = SearchRegistryInstallations(key);
                }
            }

            if (string.IsNullOrWhiteSpace(arma3SyncPath) || !Directory.Exists(arma3SyncPath))
            {
                arma3SyncPath = "";
            }

            return arma3SyncPath;
        }

        public void StartAddonSync(string arma3SyncPath)
        {
            var p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = arma3SyncPath,
                    FileName = Path.Combine(arma3SyncPath, Constants.Arma3SyncExecutable)
                }
            };
            p.Start();
        }

        private static string SearchRegistryInstallations(RegistryKey key)
        {
            string arma3SyncPath = "";
            if (key == null) return arma3SyncPath;

            foreach (string subKeyName in key.GetSubKeyNames())
            {
                using (RegistryKey subkey = key.OpenSubKey(subKeyName))
                {
                    if (subkey == null) continue;
                    var displayName = (string)subkey.GetValue(Constants.Arma3SyncRegDisplayNameEntry, "");
                    if (!displayName.StartsWith(Constants.Arma3SyncRegDisplayNameValue)) continue;
                    arma3SyncPath = (string)subkey.GetValue(Constants.Arma3SyncRegLocationEntry, "");
                    break;
                }
            }

            return arma3SyncPath;
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
