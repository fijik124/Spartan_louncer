using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _11thLauncher.Configuration
{
    static class Addons
    {
        public static List<string> LocalAddons = new List<string>();

        //Blacklisted addon folders (not manually activable)
        private static readonly string[] VanillaAddons = { "arma 3", "curator", "kart", "heli", "mark", "dlcbundle" };

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
                if (VanillaAddons.Contains(Directory.GetParent(directory).Name.ToLower())) continue;
                int pathindex = directory.IndexOf(Settings.Arma3Path, StringComparison.Ordinal) + Settings.Arma3Path.Length + 1;
                string addon = directory.Substring(pathindex, (directory.Length - pathindex) - ("Addons".Length + 1));

                LocalAddons.Add(addon);
            }
        }
    }
}
