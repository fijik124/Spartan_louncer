using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Xml;
using Caliburn.Micro;
using MahApps.Metro;
using Microsoft.Win32;
using Newtonsoft.Json;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.Services
{
    public class SettingsService : ISettingsService
    {
        public SettingsService()
        {
            UserProfiles = new BindableCollection<UserProfile>();
            ApplicationSettings = new ApplicationSettings();
        }

        #region Properties

        public ApplicationSettings ApplicationSettings { get; set; }

        public Guid DefaultProfileId { get; set; }

        public BindableCollection<UserProfile> UserProfiles { get; set; }

        public BindableCollection<Server> Servers { get; set; }

        public string JavaVersion { get; set; }

        #endregion

        #region Methods

        public bool SettingsExist()
        {
            var settingsExist = false;

            if (Directory.Exists(Constants.ConfigPath))
            {
                settingsExist = File.Exists(Path.Combine(Constants.ConfigPath, Constants.ConfigFileName));
            }

            return settingsExist;
        }

        public string GetGameVersion()
        {
            string version = "";
            if (string.IsNullOrEmpty(ApplicationSettings.Arma3Path)) return version;

            FileVersionInfo info = FileVersionInfo.GetVersionInfo(Path.Combine(ApplicationSettings.Arma3Path, Constants.GameExecutable32));
            version = info.FileVersion + "." + info.FileBuildPart + info.FilePrivatePart;

            return version;
        }

        public void ReadPath()
        {
            string arma3RegPath;

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
                ApplicationSettings.Arma3Path = arma3RegPath;
            }
        }

        public bool Read()
        {
            var settingsLoaded = true;

            var configFile = new ConfigFile();
            if (SettingsExist())
            {
                try
                {
                    JsonConvert.PopulateObject(File.ReadAllText(Path.Combine(Constants.ConfigPath, Constants.ConfigFileName)), configFile);
                }
                catch (Exception)
                {
                    settingsLoaded = false;
                    configFile = new ConfigFile();
                }
            }

            configFile.Servers = new BindableCollection<Server>(configFile.Servers.Union(Constants.DefaultServers)); //Add default servers if they are not present

            DefaultProfileId = configFile.DefaultProfileId;

            foreach (var profile in configFile.Profiles)
            {
                UserProfiles.Add(new UserProfile(profile.Key, profile.Value, profile.Key == DefaultProfileId));
            }

            ApplicationSettings = configFile.ApplicationSettings;
            Servers = configFile.Servers;

            //Set language
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Constants.Languages.Contains(ApplicationSettings.Language) 
                ? ApplicationSettings.Language 
                : Constants.Languages.First());

            return settingsLoaded;
        }

        [Obsolete]
        private void ReadLegacy()
        {
            ConfigFile configFile = new ConfigFile();
            using (XmlReader reader = XmlReader.Create(Path.Combine(Constants.ConfigPath, Constants.LegacyConfigFileName)))
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
                            configFile.ApplicationSettings.JavaPath = value;
                            break;
                        case "ArmA3Path":
                            reader.Read();
                            value = reader.Value.Trim();
                            configFile.ApplicationSettings.Arma3Path = value;
                            break;
                        case "ArmA3SyncPath":
                            reader.Read();
                            value = reader.Value.Trim();
                            configFile.ApplicationSettings.Arma3SyncPath = value;
                            break;
                        case "Profiles":
                            var parameter = reader["default"];
                            reader.Read();
                            //configFile.DefaultProfileId = parameter; //TODO how to do this
                            break;
                        case "Profile":
                            reader.Read();
                            value = reader.Value.Trim();
                            UserProfiles.Add(new UserProfile(value));
                            break;
                        case "minimizeNotification":
                            reader.Read();
                            value = reader.Value.Trim();
                            configFile.ApplicationSettings.MinimizeNotification = bool.Parse(value);
                            break;
                        case "startMinimize":
                            reader.Read();
                            value = reader.Value.Trim();
                            configFile.ApplicationSettings.StartMinimize = bool.Parse(value);
                            break;
                        case "startClose":
                            reader.Read();
                            value = reader.Value.Trim();
                            configFile.ApplicationSettings.StartClose = bool.Parse(value);
                            break;
                        case "accent":
                            reader.Read();
                            value = reader.Value.Trim();
                            configFile.ApplicationSettings.AccentColor = (AccentColor) int.Parse(value);
                            break;
                        case "checkUpdates":
                            reader.Read();
                            value = reader.Value.Trim();
                            configFile.ApplicationSettings.CheckUpdates = bool.Parse(value);
                            break;
                        case "checkServers":
                            reader.Read();
                            value = reader.Value.Trim();
                            configFile.ApplicationSettings.CheckServers = bool.Parse(value);
                            break;
                        case "checkRepository":
                            reader.Read();
                            value = reader.Value.Trim();
                            configFile.ApplicationSettings.CheckRepository = bool.Parse(value);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CheckLegacyConfig()
        {
            return false;//TODO
        }

        public void Write()
        {
            var profiles = UserProfiles.ToDictionary(profile => profile.Id, profile => profile.Name);
            var configFile = new ConfigFile
            {
                ApplicationSettings = ApplicationSettings,
                DefaultProfileId = DefaultProfileId,
                Profiles = profiles,
                Servers = Servers
            };

            //If no config directory exists, create it
            if (!Directory.Exists(Constants.ConfigPath))
            {
                Directory.CreateDirectory(Constants.ConfigPath);
            }

            File.WriteAllText(Path.Combine(Constants.ConfigPath, Constants.ConfigFileName), JsonConvert.SerializeObject(configFile, Newtonsoft.Json.Formatting.Indented));
        }

        /// <summary>
        /// Write the XML configuration of the application with the current values
        /// </summary>
        [Obsolete]
        public void Writse()
        {
            //if (!Directory.Exists(Constants.ConfigPath))
            //{
            //Directory.CreateDirectory(Constants.ConfigPath);
            //}

            //XmlWriterSettings settings = new XmlWriterSettings
            //{
            //Indent = true,
            //IndentChars = "\t"
            //};

            //using (XmlWriter writer = XmlWriter.Create(Constants.ConfigPath + "\\config.xml", settings))
            //{
            //writer.WriteStartDocument();
            //writer.WriteStartElement("Configuration");

            ////Path
            //writer.WriteElementString("JavaPath", JavaPath);
            //writer.WriteElementString("ArmA3Path", Arma3Path);
            //writer.WriteElementString("ArmA3SyncPath", Arma3SyncPath);
            //writer.WriteElementString("ArmA3SyncRepository", Arma3SyncRepository);

            ////Profiles
            //writer.WriteStartElement("Profiles");
            //writer.WriteAttributeString("default", Profiles.DefaultProfile);
            //if (Profiles.UserProfiles != null)
            //{
            //foreach (string profile in Profiles.UserProfiles)
            //{
            //writer.WriteStartElement("Profile");
            //writer.WriteString(profile);
            //writer.WriteEndElement();
            //}
            //}
            //writer.WriteEndElement();

            ////Configuration parameters
            //writer.WriteElementString("minimizeNotification", MinimizeNotification.ToString());
            //writer.WriteElementString("startMinimize", StartMinimize.ToString());
            //writer.WriteElementString("startClose", StartClose.ToString());
            //writer.WriteElementString("accent", Accent.ToString());
            //writer.WriteElementString("checkUpdates", CheckUpdates.ToString());
            //writer.WriteElementString("checkServers", CheckServers.ToString());
            //writer.WriteElementString("checkRepository", CheckRepository.ToString());
            //writer.WriteElementString("serversGroupBox", ServersGroupBox.ToString());
            //writer.WriteElementString("repositoryGroupBox", RepositoryGroupBox.ToString());

            //writer.WriteEndElement();
            //writer.WriteEndDocument();
            //}
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

        public void UpdateThemeAndAccent(ThemeStyle theme, AccentColor accent)
        {
            ThemeManager.ChangeAppStyle(Application.Current,
                ThemeManager.GetAccent(accent.ToString()),
                ThemeManager.GetAppTheme(theme.ToString()));
        }

        #endregion
    }
}
