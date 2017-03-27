using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Caliburn.Micro;
using Microsoft.Win32;
using _11thLauncher.Configuration;
using _11thLauncher.Messages;
using _11thLauncher.Model.Profile;

namespace _11thLauncher.Model.Settings
{
    public class SettingsManager
    {
        private readonly IEventAggregator _eventAggregator;

        //Program settings
        public string Arma3Path = "";
        public bool MinimizeNotification;
        public int Accent;


        //Profile settings
        private string _defaultProfileName;
        public UserProfile DefaultProfile;
        public BindableCollection<UserProfile> UserProfiles;

        //Memory allocators (TODO separately?)
        public BindableCollection<Allocator> MemoryAllocators;
        public BindableCollection<Allocator> MemoryAllocatorsX64;

        //TODO process and move up
        public string JavaPath = "";
        public string Arma3SyncPath = "";
        public string Arma3SyncRepository = "";
        public bool StartClose;
        public bool StartMinimize;
        public bool CheckUpdates = true;
        public bool CheckServers = true;
        public bool CheckRepository;
        public bool ServersGroupBox = true;
        public bool RepositoryGroupBox = true;

        public BindableCollection<UserProfile> Initialize()
        {
            UserProfiles = new BindableCollection<UserProfile>();

            if (Directory.Exists(Constants.ConfigPath))
            {
                Read();
            }
            else
            {
                if (!ReadPath())
                {
                    _eventAggregator.PublishOnUIThread(new ShowDialogMessage
                    {
                        Title = Properties.Resources.S_MSG_PATH_TITLE,
                        Content = Properties.Resources.S_MSG_PATH_CONTENT
                    });
                }

                //Create default profile
                UserProfile defaultProfile = new UserProfile(Constants.DefaultProfileName, true);
                defaultProfile.Write();
            }

            var found = false;
            foreach (var profile in UserProfiles)
            {
                if (profile.Name != _defaultProfileName) continue;

                profile.IsDefault = true;
                DefaultProfile = profile;
                found = true;
                break;
            }
            if (!found)
            {
                UserProfiles.First().IsDefault = true;
            }

            return UserProfiles;
        }

        /// <summary>
        /// Get the version number of the game executable
        /// </summary>
        /// <returns>Game version</returns>
        public string GetGameVersion()
        {
            string version = "";

            if (!string.IsNullOrEmpty(Arma3Path))
            {
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(Arma3Path + "\\arma3.exe");
                version = info.FileVersion + "." + info.FileBuildPart + info.FilePrivatePart;
            }

            return version;
        }

        /// <summary>
        /// Try to read the game path from the windows registry
        /// </summary>
        /// <returns>bool value to indicate if the path was read correctly</returns>
        public bool ReadPath()
        {
            string arma3RegPath;
            bool valid = false;

            //First try to get the path using ArmA 3 registry entry
            if (Environment.Is64BitOperatingSystem)
            {
                arma3RegPath = (string)Registry.GetValue(Constants.Arma3RegPath64[0], Constants.Arma3RegPath64[1], Constants.Arma3RegPath64[2]);
            }
            else
            {
                arma3RegPath = (string)Registry.GetValue(Constants.Arma3RegPath32[0], Constants.Arma3RegPath32[1], Constants.Arma3RegPath32[2]);
            }
            if (!Directory.Exists(arma3RegPath))
            {
                arma3RegPath = null;
            }

            //If ArmA 3 registry entry is not found, use Steam entry
            if (string.IsNullOrEmpty(arma3RegPath))
            {
                string steamPath;
                if (Environment.Is64BitOperatingSystem)
                {
                    steamPath = (string)Registry.GetValue(Constants.SteamRegPath64[0], Constants.SteamRegPath64[1], Constants.SteamRegPath64[2]);
                    arma3RegPath = Path.Combine(steamPath, Constants.DefaultArma3SteamPath);
                }
                else
                {
                    steamPath = (string)Registry.GetValue(Constants.SteamRegPath32[0], Constants.SteamRegPath32[1], Constants.SteamRegPath32[2]);
                    arma3RegPath = Path.Combine(steamPath, Constants.DefaultArma3SteamPath);
                }
            }
            if (!Directory.Exists(arma3RegPath))
            {
                arma3RegPath = null;
            }

            //If the path is found and exists, set to config and return that a valid path was found
            if (!string.IsNullOrEmpty(arma3RegPath))
            {
                Arma3Path = arma3RegPath;
                valid = true;
            }

            return valid;
        }

        /// <summary>
        /// Write the XML configuration of the application with the current values
        /// </summary>
        public void Write()
        {
            if (!Directory.Exists(Constants.ConfigPath))
            {
                Directory.CreateDirectory(Constants.ConfigPath);
            }

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t"
            };

            using (XmlWriter writer = XmlWriter.Create(Constants.ConfigPath + "\\config.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Configuration");

                //Path
                writer.WriteElementString("JavaPath", JavaPath);
                writer.WriteElementString("ArmA3Path", Arma3Path);
                writer.WriteElementString("ArmA3SyncPath", Arma3SyncPath);
                writer.WriteElementString("ArmA3SyncRepository", Arma3SyncRepository);

                //Profiles
                writer.WriteStartElement("Profiles");
                writer.WriteAttributeString("default", Profiles.DefaultProfile);
                if (Profiles.UserProfiles != null)
                {
                    foreach (string profile in Profiles.UserProfiles)
                    {
                        writer.WriteStartElement("Profile");
                        writer.WriteString(profile);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();

                //Configuration parameters
                writer.WriteElementString("minimizeNotification", MinimizeNotification.ToString());
                writer.WriteElementString("startMinimize", StartMinimize.ToString());
                writer.WriteElementString("startClose", StartClose.ToString());
                writer.WriteElementString("accent", Accent.ToString());
                writer.WriteElementString("checkUpdates", CheckUpdates.ToString());
                writer.WriteElementString("checkServers", CheckServers.ToString());
                writer.WriteElementString("checkRepository", CheckRepository.ToString());
                writer.WriteElementString("serversGroupBox", ServersGroupBox.ToString());
                writer.WriteElementString("repositoryGroupBox", RepositoryGroupBox.ToString());

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        /// <summary>
        /// Read the XML configuration file and set current values
        /// </summary>
        private void Read()
        {
            using (XmlReader reader = XmlReader.Create(Constants.ConfigPath + "\\config.xml"))
            {
                while (reader.Read())
                {
                    if (!reader.IsStartElement()) continue;

                    string value;
                    switch (reader.Name)
                    {
                        case "JavaPath":
                            reader.Read();
                            value = reader.Value.Trim();
                            JavaPath = value;
                            break;
                        case "ArmA3Path":
                            reader.Read();
                            value = reader.Value.Trim();
                            Arma3Path = value;
                            break;
                        case "ArmA3SyncPath":
                            reader.Read();
                            value = reader.Value.Trim();
                            Arma3SyncPath = value;
                            break;
                        case "ArmA3SyncRepository":
                            reader.Read();
                            value = reader.Value.Trim();
                            Arma3SyncRepository = value;
                            break;
                        case "Profiles":
                            var parameter = reader["default"];
                            reader.Read();
                            _defaultProfileName = parameter;
                            break;
                        case "Profile":
                            reader.Read();
                            value = reader.Value.Trim();
                            UserProfiles.Add(new UserProfile(value));
                            break;
                        case "minimizeNotification":
                            reader.Read();
                            value = reader.Value.Trim();
                            MinimizeNotification = bool.Parse(value);
                            break;
                        case "startMinimize":
                            reader.Read();
                            value = reader.Value.Trim();
                            StartMinimize = bool.Parse(value);
                            break;
                        case "startClose":
                            reader.Read();
                            value = reader.Value.Trim();
                            StartClose = bool.Parse(value);
                            break;
                        case "accent":
                            reader.Read();
                            value = reader.Value.Trim();
                            Accent = int.Parse(value);
                            break;
                        case "checkUpdates":
                            reader.Read();
                            value = reader.Value.Trim();
                            CheckUpdates = bool.Parse(value);
                            break;
                        case "checkServers":
                            reader.Read();
                            value = reader.Value.Trim();
                            CheckServers = bool.Parse(value);
                            break;
                        case "checkRepository":
                            reader.Read();
                            value = reader.Value.Trim();
                            CheckRepository = bool.Parse(value);
                            break;
                        case "serversGroupBox":
                            reader.Read();
                            value = reader.Value.Trim();
                            ServersGroupBox = bool.Parse(value);
                            break;
                        case "repositoryGroupBox":
                            reader.Read();
                            value = reader.Value.Trim();
                            RepositoryGroupBox = bool.Parse(value);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Delete the configuration folder completely, resetting the application status
        /// </summary>
        public void Delete()
        {
            if (Directory.Exists(Constants.ConfigPath))
            {
                Directory.Delete(Constants.ConfigPath, true);
            }
        }

        /// <summary>
        /// Read the memory allocators available in the ArmA 3 Dll folder
        /// </summary>
        public void ReadAllocators()
        {
            MemoryAllocators.Add(new Allocator("system", "system (Windows)", AllocatorType.X32));
            MemoryAllocatorsX64.Add(new Allocator("system", "system (Windows)", AllocatorType.X64));

            if (Arma3Path == "") return;

            string[] files = Directory.GetFiles(Arma3Path + "\\Dll", "*.dll");
            foreach (string file in files)
            {
                if (file.EndsWith("_x64.dll")) continue;
                var name = Path.GetFileNameWithoutExtension(file);
                MemoryAllocators.Add(new Allocator(name, name + " (x32)", AllocatorType.X32));
            }

            string[] filesX64 = Directory.GetFiles(Arma3Path + "\\Dll", "*_x64.dll");
            foreach (string file in filesX64)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                MemoryAllocatorsX64.Add(new Allocator(name, name + " (x64)", AllocatorType.X64));
            }
        }
    }
}
