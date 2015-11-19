using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace _11thLauncher.Configuration
{
    static class Profiles
    {
        private static readonly string ProfilesPath = Settings.ConfigPath + "\\profiles";

        //User profiles
        public static List<string> UserProfiles = new List<string>();
        public static string DefaultProfile = "";

        //Current profile data
        public static Dictionary<string, string> ProfileParameters = new Dictionary<string, string>();
        public static Dictionary<string, string> ProfileAddons = new Dictionary<string, string>();
        public static Dictionary<string, string> ProfileServerInfo = new Dictionary<string, string>();

        /// <summary>
        /// Return the given parameter value for the current profile or the default value if it doesn't exist
        /// </summary>
        /// <param name="parameter">Parameter name</param>
        /// <param name="defaultValue">Default value used if the parameter doesn't exist</param>
        /// <returns>Value of the parameter</returns>
        public static bool GetParameter(string parameter, bool defaultValue)
        {
            return ProfileParameters.ContainsKey(parameter) ? bool.Parse(ProfileParameters[parameter]) : defaultValue;
        }

        /// <summary>
        /// Return the given parameter value for the current profile or the default value if it doesn't exist
        /// </summary>
        /// <param name="parameter">Parameter name</param>
        /// <param name="defaultValue">Default value used if the parameter doesn't exist</param>
        /// <returns>Value of the parameter</returns>
        public static int GetParameter(string parameter, int defaultValue)
        {
            return ProfileParameters.ContainsKey(parameter) ? int.Parse(ProfileParameters[parameter]) : defaultValue;
        }

        /// <summary>
        /// Return the given parameter value for the current profile or the default value if it doesn't exist
        /// </summary>
        /// <param name="parameter">Parameter name</param>
        /// <param name="defaultValue">Default value used if the parameter doesn't exist</param>
        /// <returns>Value of the parameter</returns>
        public static string GetParameter(string parameter, string defaultValue)
        {
            return ProfileParameters.ContainsKey(parameter) ? ProfileParameters[parameter] : defaultValue;
        }

        /// <summary>
        /// Create and save a default profile
        /// </summary>
        public static void CreateDefault()
        {
            UserProfiles.Add("Predeterminado");
            DefaultProfile = "Predeterminado";

            ProfileServerInfo["server"] = "";
            ProfileServerInfo["port"] = "";
            ProfileServerInfo["pass"] = "";

            WriteProfile("Predeterminado");
        }

        /// <summary>
        /// Write profile with the given name to an XML file in the application config folder
        /// </summary>
        /// <param name="profile">Name of the profile to write</param>
        public static void WriteProfile(string profile)
        {
            if (!Directory.Exists(ProfilesPath))
            {
                Directory.CreateDirectory(ProfilesPath);
            }

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t"
            };

            using (XmlWriter writer = XmlWriter.Create(ProfilesPath + "\\" + profile + ".xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Profile");

                // Parameters
                writer.WriteStartElement("Parameters");
                if (ProfileParameters != null)
                {
                    foreach (KeyValuePair<string, string> entry in ProfileParameters)
                    {
                        writer.WriteStartElement("Parameter");
                        writer.WriteAttributeString("name", entry.Key);
                        writer.WriteString(entry.Value);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();

                // Addons
                writer.WriteStartElement("ArmA3Addons");
                if (ProfileAddons != null)
                {
                    foreach (KeyValuePair<string, string> addon in ProfileAddons)
                    {
                        writer.WriteStartElement("A3Addon");
                        writer.WriteAttributeString("name", addon.Key);
                        writer.WriteString(addon.Value);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();

                // ServerInfo
                writer.WriteStartElement("ArmA3Server");
                if (ProfileServerInfo != null)
                {
                    foreach (KeyValuePair<string, string> entry in ProfileServerInfo)
                    {
                        writer.WriteStartElement("A3ServerInfo");
                        writer.WriteAttributeString("name", entry.Key);
                        writer.WriteString(entry.Value);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        /// <summary>
        /// Read the profile with the given name from an XML file in the application config folder
        /// </summary>
        /// <param name="profile">Name of the profile to read</param>
        public static void ReadProfile(string profile)
        {
            ProfileAddons.Clear();

            //Read profile
            using (XmlReader reader = XmlReader.Create(ProfilesPath + "\\" + profile + ".xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        string parameter;
                        string value;
                        switch (reader.Name)
                        {
                            case "ArmA3Path":
                                reader.Read();
                                value = reader.Value.Trim();
                                Settings.Arma3Path = value;
                                break;
                            case "Parameter":
                                parameter = reader["name"];
                                reader.Read();
                                value = reader.Value.Trim();
                                if (parameter != null) ProfileParameters[parameter] = value;
                                break;
                            case "A3Addon":
                                parameter = reader["name"];
                                reader.Read();
                                value = reader.Value.Trim();
                                //If addon no longer exists, discard it 
                                if (Addons.LocalAddons.Contains(parameter))
                                {
                                    if (parameter != null) ProfileAddons[parameter] = value;
                                }
                                break;
                            case "A3ServerInfo":
                                parameter = reader["name"];
                                reader.Read();
                                value = reader.Value.Trim();
                                if (parameter != null) ProfileServerInfo[parameter] = value;
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Delete the profile with the given name, both from the program and the application config folder
        /// </summary>
        /// <param name="profile">Name of the profile to delete</param>
        public static void DeleteProfile(string profile)
        {
            UserProfiles.Remove(profile);
            File.Delete(ProfilesPath + "\\" + profile + ".xml");
            Settings.Write();
        }
    }
}
