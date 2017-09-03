using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Caliburn.Micro;
using Microsoft.Win32;
using _11thLauncher.Accessors.Contracts;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;
using _11thLauncher.Util;

namespace _11thLauncher.Services
{
    public class Arma3SyncService : AbstractService, IAddonSyncService
    {
        private readonly IFileAccessor _fileAccessor;
        private readonly IRegistryAccessor _registryAccessor;
        private readonly INetworkAccessor _networkAccessor;
        private readonly IProcessAccessor _processAccessor;

        public Arma3SyncService(IFileAccessor fileAccessor, IRegistryAccessor registryAccessor, INetworkAccessor networkAccessor, IProcessAccessor processAccessor, ILogger logger)
            : base(logger)
        {
            _fileAccessor = fileAccessor;
            _registryAccessor = registryAccessor;
            _networkAccessor = networkAccessor;
            _processAccessor = processAccessor;
        }

        public BindableCollection<Repository> ReadRepositories(string arma3SyncPath)
        {
            BindableCollection<Repository> repositories = new BindableCollection<Repository>();
            if (!_fileAccessor.DirectoryExists(arma3SyncPath))
            {
                Logger.LogDebug("Arma3SyncService", "No valid Arma3Sync path defined, skipping reading repositories");
                return repositories;
            };
            
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

            Logger.LogDebug("Arma3SyncService", $"Finished reading repositories, found {repositories.Count}");

            return repositories;
        }

        public void CheckRepository(string arma3SyncPath, string javaPath, Repository repository)
        {
            repository.Status = RepositoryStatus.Checking;

            //Extract A3SDS
            Logger.LogDebug("Arma3SyncService", "Extracting deserializer");
            _fileAccessor.WriteAllBytes(ApplicationConfig.A3SdsPath, Properties.Resources.A3SDS);

            Logger.LogDebug("Arma3SyncService", "Deserializing local repository");
            DeserializeLocalRepository(arma3SyncPath, javaPath, repository);

            Logger.LogDebug("Arma3SyncService", "Deserializing remote repository");
            DeserializeRemoteRepository(javaPath, repository);

            //Delete A3SDS
            Logger.LogDebug("Arma3SyncService", "Cleaning up deserializer");
            _fileAccessor.DeleteFile(ApplicationConfig.A3SdsPath);

            if (repository.LocalRevision != null && repository.RemoteRevision != null)
            {
                repository.Status = repository.LocalRevision.Equals(repository.RemoteRevision)
                    ? RepositoryStatus.Updated
                    : RepositoryStatus.Outdated;
            }
            else
            {
                repository.Status = RepositoryStatus.Unknown;
            }

            Logger.LogDebug("Arma3SyncService", $"Finished checking repository status, status is {repository.Status}");
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

            if (localRepositoryInfo.Length != 6)
            {
                Logger.LogDebug("Arma3SyncService", "Unable to read local repository info");
                return;
            };

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

                if (remoteRepositoryInfo.Length == 2)
                {
                    repository.RemoteRevision = remoteRepositoryInfo[0];
                }
            }
            catch (WebException e)
            {
                Logger.LogException("Arma3SyncService", "Error deserializing remote repository", e);
            }
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
                Logger.LogDebug("Arma3SyncService", $"Java version in system detected: {javaVersion}");
            }
            catch (Exception e)
            {
                Logger.LogException("Arma3SyncService", "Error detecting Java version", e);
            }

            return javaVersion;
        }

        public string GetAddonSyncPath()
        {
            string arma3SyncPath = "";

            try
            {
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
            }
            catch (Exception e)
            {
                Logger.LogException("Arma3SyncService", "Error searching the Windows registry for a valid installation", e);
            }

            if (string.IsNullOrWhiteSpace(arma3SyncPath) || !_fileAccessor.DirectoryExists(arma3SyncPath))
            {
                arma3SyncPath = "";
                Logger.LogInfo("Arma3SyncService", "No valid Arma3Sync installations found in Windows Registry");
            }
            else
            {
                Logger.LogInfo("Arma3SyncService", $"Arma3Sync installation found in Windows Registry: '{arma3SyncPath}'");
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
            Logger.LogInfo("Arma3SyncService", "Starting Arma3Sync");
        }

        private string SearchRegistryInstallations(RegistryKey key)
        {
            string arma3SyncPath = "";
            if (key == null) return arma3SyncPath;

            Logger.LogInfo("Arma3SyncService", "Searching the Windows Registry for Arma3Sync installations");

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
    }
}
