﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Caliburn.Micro;
using Microsoft.Win32;
using _11thLauncher.Accessors.Contracts;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.Services
{
    public class Arma3SyncService : IAddonSyncService
    {
        private readonly IFileAccessor _fileAccessor;
        private readonly IRegistryAccessor _registryAccessor;
        private readonly INetworkAccessor _networkAccessor;
        private readonly IProcessAccessor _processAccessor;

        public Arma3SyncService(IFileAccessor fileAccessor, IRegistryAccessor registryAccessor, INetworkAccessor networkAccessor, IProcessAccessor processAccessor)
        {
            _fileAccessor = fileAccessor;
            _registryAccessor = registryAccessor;
            _networkAccessor = networkAccessor;
            _processAccessor = processAccessor;
        }

        public BindableCollection<Repository> ReadRepositories(string arma3SyncPath)
        {
            BindableCollection<Repository> repositories = new BindableCollection<Repository>();
            if (!_fileAccessor.DirectoryExists(arma3SyncPath)) return repositories;

            string[] files = _fileAccessor.GetFiles(Path.Combine(arma3SyncPath, ApplicationConfig.Arma3SyncConfigFolder));
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
            _fileAccessor.WriteAllBytes(ApplicationConfig.A3SdsPath, Properties.Resources.A3SDS);

            DeserializeLocalRepository(arma3SyncPath, javaPath, repository);
            DeserializeRemoteRepository(javaPath, repository);

            //Delete A3SDS
            _fileAccessor.DeleteFile(ApplicationConfig.A3SdsPath);

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

        private void DeserializeLocalRepository(string arma3SyncPath, string javaPath, Repository repository)
        {
            string repositoryPath = Path.Combine(arma3SyncPath, ApplicationConfig.Arma3SyncConfigFolder, repository.Name + ApplicationConfig.Arma3SyncRepositoryExtension);

            Process p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = !string.IsNullOrEmpty(javaPath) ? javaPath : ApplicationConfig.JavaPathCommand,
                    Arguments = " -jar " + ApplicationConfig.A3SdsPath + " -deserializeRepository \"" + repositoryPath + "\""
                }
            };
            _processAccessor.Start(p);
            string localRepository = _processAccessor.GetStandardOutput(p).ReadToEnd();
            string[] localRepositoryInfo = localRepository.TrimEnd('\r', '\n').Split(',');
            _processAccessor.WaitForExit(p);

            if (localRepositoryInfo.Length != 6) return;

            repository.LocalRevision = localRepositoryInfo[1];
            repository.Login = localRepositoryInfo[2];
            repository.Password = localRepositoryInfo[3];
            repository.Address = localRepositoryInfo[5];
        }

        private void DeserializeRemoteRepository(string javaPath, Repository repository)
        {
            try
            {
                string tempPath = Path.GetTempPath() + repository.Name + "repoInfo";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format(ApplicationConfig.Arma3SyncRemoteServerInfo, repository.Address));
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(repository.Login, repository.Password);

                FtpWebResponse response = (FtpWebResponse)_networkAccessor.GetWebResponse(request);
                Stream responseStream = response.GetResponseStream();
                using (Stream s = _fileAccessor.CreateFile(tempPath))
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
                        FileName = !string.IsNullOrEmpty(javaPath) ? javaPath : ApplicationConfig.JavaPathCommand,
                        Arguments = " -jar " + ApplicationConfig.A3SdsPath + " -deserializeServerInfo \"" + tempPath + "\""
                    }
                };
                _processAccessor.Start(p);
                string remoteRepository = _processAccessor.GetStandardOutput(p).ReadToEnd();
                string[] remoteRepositoryInfo = remoteRepository.TrimEnd('\r', '\n').Split(',');
                _processAccessor.WaitForExit(p);

                //Delete temp file
                _fileAccessor.DeleteFile(tempPath);

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
                _processAccessor.Start(p);
                javaVersion = _processAccessor.GetStandardError(p).ReadLine();
                _processAccessor.GetStandardError(p).ReadToEnd();
                _processAccessor.WaitForExit(p);
            }
            catch (Exception) { }

            return javaVersion;
        }

        public string GetAddonSyncPath()
        {
            string arma3SyncPath = "";

            if (Environment.Is64BitOperatingSystem)
            {
                using (RegistryKey key = _registryAccessor.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(ApplicationConfig.Arma3SyncBaseRegistryPath64))
                {
                    arma3SyncPath = SearchRegistryInstallations(key);
                }
            }
            else
            {
                using (RegistryKey key = _registryAccessor.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(ApplicationConfig.Arma3SyncBaseRegistryPath32))
                {
                    arma3SyncPath = SearchRegistryInstallations(key);
                }
            }

            if (string.IsNullOrWhiteSpace(arma3SyncPath) || !_fileAccessor.DirectoryExists(arma3SyncPath))
            {
                arma3SyncPath = "";
            }

            return arma3SyncPath;
        }

        public bool AddonSyncPathIsValid(string path)
        {
            return !string.IsNullOrEmpty(path) && _fileAccessor.DirectoryExists(path) && _fileAccessor.FileExists(Path.Combine(path, ApplicationConfig.Arma3SyncExecutable));
        }

        public void StartAddonSync(string arma3SyncPath)
        {
            var p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = arma3SyncPath,
                    FileName = Path.Combine(arma3SyncPath, ApplicationConfig.Arma3SyncExecutable)
                }
            };
            _processAccessor.Start(p);
        }

        private string SearchRegistryInstallations(RegistryKey key)
        {
            string arma3SyncPath = "";
            if (key == null) return arma3SyncPath;

            foreach (string subKeyName in key.GetSubKeyNames())
            {
                using (RegistryKey subkey = _registryAccessor.OpenSubKey(key, subKeyName))
                {
                    if (subkey == null) continue;
                    var displayName = (string)_registryAccessor.GetKeyValue(subkey, ApplicationConfig.Arma3SyncRegDisplayNameEntry, "");
                    if (!displayName.StartsWith(ApplicationConfig.Arma3SyncRegDisplayNameValue)) continue;
                    arma3SyncPath = (string)_registryAccessor.GetKeyValue(subkey, ApplicationConfig.Arma3SyncRegLocationEntry, "");
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
