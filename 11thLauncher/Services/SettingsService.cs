﻿using System;
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
        private string _legacyDefaultProfileName;

        public SettingsService()
        {
            UserProfiles = new BindableCollection<UserProfile>();
            ApplicationSettings = new ApplicationSettings();
        }

        #region Properties

        public ApplicationSettings ApplicationSettings { get; set; }

        public Guid DefaultProfileId { get; set; }

        public UserProfile DefaultProfile => string.IsNullOrEmpty(_legacyDefaultProfileName) ? 
            UserProfiles.FirstOrDefault(p => p.Id.Equals(DefaultProfileId)) : 
            UserProfiles.FirstOrDefault(p => p.Name.Equals(_legacyDefaultProfileName));

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

        public LoadSettingsResult Read()
        {
            var loadResult = LoadSettingsResult.NoExistingSettings;
            var configFile = new ConfigFile();

            //Try to ready legacy config if present
            if (LegacyConfigExists())
            {
                try
                {
                    configFile = ReadLegacy();
                    loadResult = LoadSettingsResult.LoadedLegacySettings;
                    //TODO convert legacy profiles
                    //TODO after save new settings
                }
                catch (Exception)
                {
                    configFile = new ConfigFile();
                    loadResult = LoadSettingsResult.ErrorLoadingLegacySettings;
                }
            }
            else
            {
                //Read existing config if present
                if (SettingsExist())
                {
                    try
                    {
                        JsonConvert.PopulateObject(File.ReadAllText(Path.Combine(Constants.ConfigPath, Constants.ConfigFileName)), configFile);
                        loadResult = LoadSettingsResult.LoadedExistingSettings;
                    }
                    catch (Exception)
                    {
                        configFile = new ConfigFile();
                        loadResult = LoadSettingsResult.ErrorLoadingSettings;
                    }
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

            return loadResult;
        }

        private ConfigFile ReadLegacy()
        {
            ConfigFile configFile = new ConfigFile();

            var legacyConfigFile = Path.Combine(Constants.ConfigPath, Constants.LegacyConfigFileName);
            using (XmlReader reader = XmlReader.Create(legacyConfigFile))
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
                            _legacyDefaultProfileName = parameter;
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
                            configFile.ApplicationSettings.AccentColor = (AccentColor)int.Parse(value);
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

            //Delete the legacy config file after reading it
            File.Delete(legacyConfigFile);

            return configFile;
        }

        /// <summary>
        /// Checks if there is an existing launcher configuration of a legacy version.
        /// </summary>
        /// <returns>True if a legacy configuration is present</returns>
        private bool LegacyConfigExists()
        {
            return File.Exists(Path.Combine(Constants.ConfigPath, Constants.LegacyConfigFileName));
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
