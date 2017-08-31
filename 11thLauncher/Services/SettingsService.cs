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
using Newtonsoft.Json;
using _11thLauncher.Accessors.Contracts;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;
using _11thLauncher.Util;

namespace _11thLauncher.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IFileAccessor _fileAccessor;
        private readonly IRegistryAccessor _registryAccessor;
        private readonly ILogger _logger;
        private string _gameVersion;

        public SettingsService(IFileAccessor fileAccessor, IRegistryAccessor registryAccessor, ILogger logger)
        {
            _fileAccessor = fileAccessor;
            _registryAccessor = registryAccessor;
            _logger = logger;

            UserProfiles = new BindableCollection<UserProfile>();
            ApplicationSettings = new ApplicationSettings();
        }

        #region Properties

        public ApplicationSettings ApplicationSettings { get; set; }

        public Guid DefaultProfileId { get; set; }

        public UserProfile DefaultProfile => UserProfiles.FirstOrDefault(p => p.Id.Equals(DefaultProfileId));

        public BindableCollection<UserProfile> UserProfiles { get; set; }

        public BindableCollection<Server> Servers { get; set; }

        public string JavaVersion { get; set; }

        #endregion

        #region Methods

        public bool SettingsExist()
        {
            var settingsExist = false;

            if (_fileAccessor.DirectoryExists(ApplicationConfig.ConfigPath))
            {
                settingsExist = _fileAccessor.FileExists(Path.Combine(ApplicationConfig.ConfigPath, ApplicationConfig.ConfigFileName));
            }

            _logger.LogDebug("SettingsService", "Checked if settings exist, result is: " + settingsExist);
            return settingsExist;
        }

        public string GetGameVersion()
        {
            if (!string.IsNullOrEmpty(_gameVersion)) return _gameVersion;

            string version = "";
            if (string.IsNullOrEmpty(ApplicationSettings.Arma3Path))
            {
                _logger.LogDebug("SettingsService", "Unable to detect game version, path is not set");
                return version;
            }

            FileVersionInfo info = FileVersionInfo.GetVersionInfo(Path.Combine(ApplicationSettings.Arma3Path, ApplicationConfig.GameExecutable32));
            version = info.FileVersion + "." + info.FileBuildPart + info.FilePrivatePart;
            _gameVersion = version;

            _logger.LogDebug("SettingsService", "Local game version detected to be: " + version);
            return version;
        }

        public void ReadPath()
        {
            string arma3RegPath = null;

            try
            {
                //First try to get the path using ArmA 3 registry entry
                if (Environment.Is64BitOperatingSystem)
                {
                    arma3RegPath = (string)_registryAccessor.GetValue(ApplicationConfig.Arma3RegPath64[0], ApplicationConfig.Arma3RegPath64[1], ApplicationConfig.Arma3RegPath64[2]);
                }
                else
                {
                    arma3RegPath = (string)_registryAccessor.GetValue(ApplicationConfig.Arma3RegPath32[0], ApplicationConfig.Arma3RegPath32[1], ApplicationConfig.Arma3RegPath32[2]);
                }
                if (!_fileAccessor.DirectoryExists(arma3RegPath))
                {
                    arma3RegPath = null;
                }

                //If ArmA 3 registry entry is not found, use Steam entry
                if (string.IsNullOrEmpty(arma3RegPath))
                {
                    string steamPath;
                    if (Environment.Is64BitOperatingSystem)
                    {
                        steamPath = (string)_registryAccessor.GetValue(ApplicationConfig.SteamRegPath64[0], ApplicationConfig.SteamRegPath64[1], ApplicationConfig.SteamRegPath64[2]);
                        arma3RegPath = Path.Combine(steamPath, ApplicationConfig.DefaultArma3SteamPath);
                    }
                    else
                    {
                        steamPath = (string)_registryAccessor.GetValue(ApplicationConfig.SteamRegPath32[0], ApplicationConfig.SteamRegPath32[1], ApplicationConfig.SteamRegPath32[2]);
                        arma3RegPath = Path.Combine(steamPath, ApplicationConfig.DefaultArma3SteamPath);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogException("SettingsService", "Exception trying to read game path from registry", e);
            }

            if (!_fileAccessor.DirectoryExists(arma3RegPath))
            {
                _logger.LogDebug("SettingsService", $"Unable to detect game path from registry, the directory '{arma3RegPath}' doesn't exist");
                arma3RegPath = null;
            }

            //If the path is found and exists, set to config and return that a valid path was found
            if (!string.IsNullOrEmpty(arma3RegPath))
            {
                _logger.LogInfo("SettingsService", $"Game path successfully read from registry: '{arma3RegPath}'");
                ApplicationSettings.Arma3Path = arma3RegPath;
            }
        }

        public LoadSettingsResult Read()
        {
            _logger.LogDebug("SettingsService", "Reading settings from disk");

            var loadResult = LoadSettingsResult.NoExistingSettings;
            var configFile = new ConfigFile();

            //Try to ready legacy config if present
            if (LegacyConfigExists())
            {
                try
                {
                    configFile = ReadLegacy();
                    loadResult = LoadSettingsResult.LoadedLegacySettings;

                    ApplicationSettings = configFile.ApplicationSettings;
                    DefaultProfileId = configFile.DefaultProfileId;
                    Write();
                }
                catch (Exception e)
                {
                    configFile = new ConfigFile();
                    loadResult = LoadSettingsResult.ErrorLoadingLegacySettings;
                    _logger.LogException("SettingsService", "Exception reading legacy settings", e);
                }
            }
            else
            {
                //Read existing config if present
                if (SettingsExist())
                {
                    try
                    {
                        JsonConvert.PopulateObject(_fileAccessor.ReadAllText(Path.Combine(ApplicationConfig.ConfigPath, ApplicationConfig.ConfigFileName)), configFile);
                        loadResult = LoadSettingsResult.LoadedExistingSettings;
                    }
                    catch (Exception)
                    {
                        configFile = new ConfigFile();
                        loadResult = LoadSettingsResult.ErrorLoadingSettings;
                    }
                }
            }

            configFile.Servers = new BindableCollection<Server>(configFile.Servers.Union(ApplicationConfig.DefaultServers)); //Add default servers if they are not present

            DefaultProfileId = configFile.DefaultProfileId;

            foreach (var profile in configFile.Profiles)
            {
                UserProfiles.Add(new UserProfile(profile.Key, profile.Value, profile.Key == DefaultProfileId));
            }

            ApplicationSettings = configFile.ApplicationSettings;
            Servers = configFile.Servers;

            //Set culture if no previous settings
            if (loadResult != LoadSettingsResult.LoadedExistingSettings)
            {
                var installedCulture = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
                var cultureMatch = ApplicationConfig.Languages.FirstOrDefault(l => l.StartsWith(installedCulture));
                if (cultureMatch != null)
                    ApplicationSettings.Language = cultureMatch;
            }
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(ApplicationConfig.Languages.Contains(ApplicationSettings.Language) 
                ? ApplicationSettings.Language 
                : ApplicationConfig.Languages.First());

            return loadResult;
        }

        private ConfigFile ReadLegacy()
        {
            _logger.LogInfo("SettingsService", "Starting conversion of legacy settings");

            ConfigFile configFile = new ConfigFile();
            string defaultProfileName = "";

            var legacyConfigFile = Path.Combine(ApplicationConfig.ConfigPath, ApplicationConfig.LegacyConfigFileName);
            var legacyConfigFileContent = _fileAccessor.ReadAllText(legacyConfigFile);

            using (StringReader stringReader = new StringReader(legacyConfigFileContent))
            using (XmlReader reader = XmlReader.Create(stringReader))
            {
                while (reader.Read())
                {
                    if (!reader.IsStartElement()) continue;
                    try
                    {
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
                                defaultProfileName = parameter;
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
                                if (bool.Parse(value))
                                    configFile.ApplicationSettings.StartAction = StartAction.Minimize;
                                break;
                            case "startClose":
                                reader.Read();
                                value = reader.Value.Trim();
                                if (bool.Parse(value))
                                    configFile.ApplicationSettings.StartAction = StartAction.Close;
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
                    catch (Exception e)
                    {
                        _logger.LogException("SettingsService", "Error reading a legacy setting", e);
                    }
                    
                }
            }

            var defaultProfile = UserProfiles.SingleOrDefault(p => p.Name.Equals(defaultProfileName));
            if (defaultProfile != null)
            {
                configFile.DefaultProfileId = defaultProfile.Id;
                defaultProfile.IsDefault = true;
            }

            //Delete the legacy config file after reading it
            try
            {
                _fileAccessor.DeleteFile(legacyConfigFile);
                _logger.LogInfo("SettingsService", "Legacy config file deleted");
            }
            catch (Exception e)
            {
                _logger.LogException("SettingsService", "Error deleting legacy config file", e);
            }

            _logger.LogInfo("SettingsService", "Finished conversion of legacy settings");
            return configFile;
        }

        /// <summary>
        /// Checks if there is an existing launcher configuration of a legacy version.
        /// </summary>
        /// <returns>True if a legacy configuration is present</returns>
        private bool LegacyConfigExists()
        {
            bool fileExists = _fileAccessor.FileExists(Path.Combine(ApplicationConfig.ConfigPath, ApplicationConfig.LegacyConfigFileName));

            if (fileExists)
            {
                _logger.LogInfo("SettingsService", "Legacy config file detected");
            }

            return fileExists;
        }

        public void Write()
        {
            _logger.LogDebug("SettingsService", "Writing settings to disk");

            var profiles = UserProfiles.ToDictionary(profile => profile.Id, profile => profile.Name);
            var configFile = new ConfigFile
            {
                ApplicationSettings = ApplicationSettings,
                DefaultProfileId = DefaultProfileId,
                Profiles = profiles,
                Servers = Servers
            };

            try
            {
                //If no config directory exists, create it
                if (!_fileAccessor.DirectoryExists(ApplicationConfig.ConfigPath))
                {
                    _fileAccessor.CreateDirectory(ApplicationConfig.ConfigPath);
                }

                _fileAccessor.WriteAllText(Path.Combine(ApplicationConfig.ConfigPath, ApplicationConfig.ConfigFileName), JsonConvert.SerializeObject(configFile, ApplicationConfig.JsonFormatting));
            }
            catch (Exception e)
            {
                _logger.LogException("SettingsService", "Error writing settings", e);
            }
        }

        /// <summary>
        /// Delete the configuration folder completely, resetting the application status
        /// </summary>
        public void Delete()
        {
            if (_fileAccessor.DirectoryExists(ApplicationConfig.ConfigPath))
            {
                _fileAccessor.DeleteDirectory(ApplicationConfig.ConfigPath, true);
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
