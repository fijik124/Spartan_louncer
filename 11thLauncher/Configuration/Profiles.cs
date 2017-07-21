using System.Collections.Generic;
using System.IO;

namespace _11thLauncher.Configuration
{
    public static class Profiles
    {
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

        }

        /// <summary>
        /// Read the profile with the given name from an XML file in the application config folder
        /// </summary>
        /// <param name="profile">Name of the profile to read</param>
        public static void ReadProfile(string profile)
        {

        }

        /// <summary>
        /// Delete the profile with the given name, both from the program and the application config folder
        /// </summary>
        /// <param name="profile">Name of the profile to delete</param>
        public static void DeleteProfile(string profile)
        {
            UserProfiles.Remove(profile);
            File.Delete(Constants.ProfilesPath + "\\" + profile + ".xml");
            //Settings.Write();
        }
    }
}
