using System;
using System.IO;
using System.Linq;
using System.Xml;
using _11thLauncher.Models;

namespace _11thLauncher.Services
{
    public partial class SettingsService
    {
        /// <summary>
        /// Checks if there is an existing launcher configuration of a legacy version
        /// </summary>
        /// <returns>True if a legacy configuration is present</returns>
        private bool LegacyConfigExists()
        {
            bool fileExists = _fileAccessor.FileExists(Path.Combine(ApplicationConfig.ConfigPath, ApplicationConfig.LegacyConfigFileName));

            if (fileExists)
            {
                Logger.LogInfo("SettingsService", "Legacy config file detected");
            }

            return fileExists;
        }

        /// <summary>
        /// Reads the existing legacy launcher configuration and converts into current format
        /// </summary>
        /// <returns>ConfigFile object with the configuration</returns>
        private ConfigFile ReadLegacy()
        {
            Logger.LogInfo("SettingsService", "Starting conversion of legacy settings");

            ConfigFile configFile = new ConfigFile();
            string defaultProfileName = string.Empty;

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
                        Logger.LogException("SettingsService", "Error reading a legacy setting", e);
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
                Logger.LogInfo("SettingsService", "Legacy config file deleted");
            }
            catch (Exception e)
            {
                Logger.LogException("SettingsService", "Error deleting legacy config file", e);
            }

            Logger.LogInfo("SettingsService", "Finished conversion of legacy settings");
            return configFile;
        }
    }
}