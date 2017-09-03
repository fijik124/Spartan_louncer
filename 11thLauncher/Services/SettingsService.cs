using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro;
using Newtonsoft.Json;
using _11thLauncher.Accessors.Contracts;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;
using _11thLauncher.Util;

namespace _11thLauncher.Services
{
    public partial class SettingsService : AbstractService, ISettingsService
    {
        #region Fields

        private readonly IFileAccessor _fileAccessor;
        private readonly IRegistryAccessor _registryAccessor;
        private string _gameVersion;

        #endregion


        public SettingsService(IFileAccessor fileAccessor, IRegistryAccessor registryAccessor, ILogger logger) : base(logger)
        {
            _fileAccessor = fileAccessor;
            _registryAccessor = registryAccessor;

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

            Logger.LogDebug("SettingsService", "Checked if settings exist, result is: " + settingsExist);
            return settingsExist;
        }

        public string GetGameVersion()
        {
            if (!string.IsNullOrEmpty(_gameVersion)) return _gameVersion;

            string version = string.Empty;
            if (string.IsNullOrEmpty(ApplicationSettings.Arma3Path))
            {
                Logger.LogDebug("SettingsService", "Unable to detect game version, path is not set");
                return version;
            }

            FileVersionInfo info = FileVersionInfo.GetVersionInfo(Path.Combine(ApplicationSettings.Arma3Path, ApplicationConfig.GameExecutable32));
            version = info.FileVersion + "." + info.FileBuildPart + info.FilePrivatePart;
            _gameVersion = version;

            Logger.LogDebug("SettingsService", "Local game version detected to be: " + version);
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
                Logger.LogException("SettingsService", "Exception trying to read game path from registry", e);
            }

            if (!_fileAccessor.DirectoryExists(arma3RegPath))
            {
                Logger.LogDebug("SettingsService", $"Unable to detect game path from registry, the directory '{arma3RegPath}' doesn't exist");
                arma3RegPath = null;
            }

            //If the path is found and exists, set to config and return that a valid path was found
            if (!string.IsNullOrEmpty(arma3RegPath))
            {
                Logger.LogInfo("SettingsService", $"Game path successfully read from registry: '{arma3RegPath}'");
                ApplicationSettings.Arma3Path = arma3RegPath;
            }
        }

        public LoadSettingsResult Read()
        {
            Logger.LogDebug("SettingsService", "Reading settings from disk");

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
                    Logger.LogException("SettingsService", "Exception reading legacy settings", e);
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
                    catch (Exception e)
                    {
                        configFile = new ConfigFile();
                        loadResult = LoadSettingsResult.ErrorLoadingSettings;
                        Logger.LogException("SettingsService", "Exception reading settings", e);
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

            Logger.LogDebug("SettingsService", $"Finished reading settings, result was: {loadResult}");

            return loadResult;
        }

        public void Write()
        {
            Logger.LogDebug("SettingsService", "Writing settings to disk");

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
                Logger.LogException("SettingsService", "Error writing settings", e);
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
                Logger.LogInfo("SettingsService", "Application settings deleted");
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