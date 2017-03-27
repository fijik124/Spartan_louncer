using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _11thLauncher.Configuration
{
    public class Addon
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string DisplayName => Name.Replace("_", "__");
    }

    public static class Addons
    {
        public static List<string> LocalAddons = new List<string>();

        /// <summary>
        /// Read the addons from the configuration path
        /// </summary>
        public static void ReadAddons()
        {
            LocalAddons.Clear();

            if (Settings.Arma3Path == "") return;

            string[] directories = Directory.GetDirectories(Settings.Arma3Path, "addons", SearchOption.AllDirectories);
            foreach (string directory in directories)
            {
                if (Model.Constants.VanillaAddons.Contains(Directory.GetParent(directory).Name.ToLower())) continue;
                int pathindex = directory.IndexOf(Settings.Arma3Path, StringComparison.Ordinal) + Settings.Arma3Path.Length + 1;
                string addon = directory.Substring(pathindex, (directory.Length - pathindex) - ("Addons".Length + 1));

                LocalAddons.Add(addon);
            }
        }
    }


}
