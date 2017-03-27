using System;
using System.IO;
using System.Linq;
using Caliburn.Micro;

namespace _11thLauncher.Model.Addon
{
    public class AddonManager
    {
        public BindableCollection<Addon> Addons;

        /// <summary>
        /// Read the addons from the game path
        /// </summary>
        public BindableCollection<Addon> ReadAddons(string arma3Path)
        {
            Addons = new BindableCollection<Addon>();

            if (arma3Path == "") return null;

            string[] directories = Directory.GetDirectories(arma3Path, Constants.AddonSubfolderName, SearchOption.AllDirectories);
            foreach (string directory in directories)
            {
                if (Constants.VanillaAddons.Contains(Directory.GetParent(directory).Name.ToLower())) continue;
                int pathindex = directory.IndexOf(arma3Path, StringComparison.Ordinal) + arma3Path.Length + 1;
                string addonName = directory.Substring(pathindex, (directory.Length - pathindex) - (Constants.AddonSubfolderName.Length + 1));

                Addons.Add(new Addon(addonName));
            }

            return Addons;
        }
    }
}
