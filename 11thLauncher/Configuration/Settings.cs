using System;
using System.IO;
using System.Xml;
using Microsoft.Win32;
using System.Collections.Generic;

namespace _11thLauncher.Configuration
{
    static class Settings
    {
        //Constants
        public static readonly string ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\11th Launcher";
        public static readonly List<string> Accents = new List<string>()
        {
            "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet",
            "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna"
        };

        //Program behaviour settings
        public static string JavaPath = "";
        public static string Arma3Path = "";
        public static string Arma3SyncPath = "";
        public static string Arma3SyncRepository = "";
        public static bool MinimizeNotification = false;
        public static bool StartClose = false;
        public static bool StartMinimize = false;
        public static int Accent = 0;
        public static bool CheckUpdates = true;
        public static bool CheckServers = true;
        public static bool CheckRepository = false;
        public static bool ServersGroupBox = true;
        public static bool RepositoryGroupBox = true;

        /// <summary>
        /// Check if the application settings file exists
        /// </summary>
        /// <returns>bool value to indicate if the file exists</returns>
        public static bool ConfigExists()
        {
            return Directory.Exists(ConfigPath);
        }

        /// <summary>
        /// Try to read the game path from the windows registry
        /// </summary>
        /// <returns>bool value to indicate if the path was read correctly</returns>
        public static bool ReadPath()
        {
            string arma3regPath;
            bool valid = false;

            //First try to get the path using ArmA 3 registry entry
            if (Environment.Is64BitOperatingSystem)
            {
                arma3regPath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Bohemia Interactive\\ArmA 3", "MAIN", null);
            }
            else
            {
                arma3regPath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Bohemia Interactive\\ArmA 3", "MAIN", null);
            }
            if (!Directory.Exists(arma3regPath))
            {
                arma3regPath = null;
            }

            //If ArmA 3 registry entry is not found, use Steam entry
            if (string.IsNullOrEmpty(arma3regPath))
            {
                string steamPath;
                if (Environment.Is64BitOperatingSystem)
                {
                    steamPath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Valve\\Steam", "InstallPath", "");
                    arma3regPath = Path.Combine(steamPath, "SteamApps\\common\\ArmA 3");
                }
                else
                {
                    steamPath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Valve\\Steam", "InstallPath", "");
                    arma3regPath = Path.Combine(steamPath, "SteamApps\\common\\ArmA 3");
                }
            }
            if (!Directory.Exists(arma3regPath))
            {
                arma3regPath = null;
            }

            //If the path is found and exists, set to config and return that a valid path was found
            if (!string.IsNullOrEmpty(arma3regPath))
            {
                Arma3Path = arma3regPath;
                valid = true;
            }

            return valid;
        }

        /// <summary>
        /// Write the XML configuration of the application with the current values
        /// </summary>
        public static void Write()
        {
            if (!Directory.Exists(ConfigPath))
            {
                Directory.CreateDirectory(ConfigPath);
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            using (XmlWriter writer = XmlWriter.Create(ConfigPath + "\\config.xml", settings))
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
        public static void Read()
        {
            Profiles.UserProfiles.Clear();
            using (XmlReader reader = XmlReader.Create(ConfigPath + "\\config.xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        string value;
                        string parameter;
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
                                parameter = reader["default"];
                                reader.Read();
                                Profiles.DefaultProfile = parameter;
                                break;
                            case "Profile":
                                reader.Read();
                                value = reader.Value.Trim();
                                Profiles.UserProfiles.Add(value);
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
        }

        /// <summary>
        /// Delete the configuration folder completely, resetting the application status
        /// </summary>
        public static void Delete()
        {
            if (Directory.Exists(ConfigPath))
            {
                Directory.Delete(ConfigPath, true);
            }
        }
    }
}
