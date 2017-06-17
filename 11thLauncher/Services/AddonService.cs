using System;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.Services
{
    public class AddonService : IAddonService
    {
        private readonly BindableCollection<Addon> _addons;

        public AddonService()
        {
            _addons = new BindableCollection<Addon>();
        }

        public BindableCollection<Addon> GetAddons()
        {
            return _addons;
        }

        public BindableCollection<Addon> ReadAddons(string arma3Path)
        {
            if (arma3Path == "") return null;

            string[] directories = Directory.GetDirectories(arma3Path, Constants.AddonSubfolderName, SearchOption.AllDirectories);
            foreach (string directory in directories)
            {
                if (Constants.VanillaAddons.Contains(Directory.GetParent(directory).Name.ToLower())) continue;
                if (Directory.GetFiles(directory, Constants.AddonFilePattern).Length == 0) continue;
                int pathindex = directory.IndexOf(arma3Path, StringComparison.Ordinal) + arma3Path.Length + 1;
                string addonName = directory.Substring(pathindex, (directory.Length - pathindex) - (Constants.AddonSubfolderName.Length + 1));

                _addons.Add(new Addon(addonName));
            }

            return _addons;
        }
    }
}
