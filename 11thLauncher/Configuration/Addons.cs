using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _11thLauncher.Configuration
{
    class Addons
    {
        public static List<string> LocalAddons = new List<string>();

        //Blacklisted addon folders (not manually activable)
        private static readonly string[] _vanillaAddons = new string[] { "arma 3", "curator", "kart", "heli", "mark", "dlcbundle" };

        /// <summary>
        /// Read the addons from the configuration path
        /// </summary>
        public static void ReadAddons()
        {
            LocalAddons.Clear();

            if (Settings.Arma3Path != "")
            {
                string[] directories = Directory.GetDirectories(Settings.Arma3Path, "addons", SearchOption.AllDirectories);
                foreach (string directory in directories)
                {
                    if (!_vanillaAddons.Contains(Directory.GetParent(directory).Name.ToLower()))
                    {
                        int pathindex = directory.IndexOf(Settings.Arma3Path) + Settings.Arma3Path.Length + 1;
                        string addon = directory.Substring(pathindex, (directory.Length - pathindex) - ("Addons".Length + 1));

                        LocalAddons.Add(addon);
                    }
                }
            }
        }
    }
}
