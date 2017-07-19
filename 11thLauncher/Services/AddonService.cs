using System;
using System.Diagnostics;
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
            if (string.IsNullOrEmpty(arma3Path)) return new BindableCollection<Addon>();
            if (_addons.Count != 0) return _addons; //Already read

            string[] directories = Directory.GetDirectories(arma3Path, Constants.AddonSubfolderName, SearchOption.AllDirectories);
            foreach (string directory in directories)
            {
                if (Constants.VanillaAddons.Contains(Directory.GetParent(directory).Name.ToLower())) continue;
                if (Directory.GetFiles(directory, Constants.AddonFilePattern).Length == 0) continue;
                int pathindex = directory.IndexOf(arma3Path, StringComparison.Ordinal) + arma3Path.Length + 1;
                string addonName = directory.Substring(pathindex, (directory.Length - pathindex) - (Constants.AddonSubfolderName.Length + 1));

                var addon = new Addon(Directory.GetParent(directory).FullName, addonName);
                ReadMetaData(addon);
                _addons.Add(addon);  
            }

            return _addons;
        }

        public void BrowseAddonFolder(Addon addon)
        {
            if (Directory.Exists(addon.Path))
            {
                Process.Start(addon.Path);
            }
        }

        public void BrowseAddonWebsite(Addon addon)
        {
            if (!string.IsNullOrEmpty(addon.MetaData?.Action))
            {
                Process.Start(addon.MetaData.Action);
            }
        }

        private static void ReadMetaData(Addon addon)
        {
            var metaFile = Path.Combine(addon.Path, Constants.AddonMetaDataFile);
            if (!File.Exists(metaFile)) return;

            addon.MetaData = new AddonMetaData();

            try
            {
                var properties = File.ReadAllLines(metaFile);
                foreach (var line in properties)
                {
                    var separator = line.IndexOf('=');
                    if (separator <= 0)
                        continue;
                    string param = line.Remove(separator).TrimEnd();
                    string value = line.Substring(separator + 1).TrimStart();
                    if (value.Length > 1)
                    {
                        value = value.Split('\"', '\"')[1];
                    }

                    switch (param)
                    {
                        case "name":
                            addon.MetaData.Name = value;
                            break;
                        case "tooltip":
                            addon.MetaData.Tooltip = value;
                            break;
                        case "action":
                            addon.MetaData.Action = value;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
                addon.MetaData = null;
            }
        }
    }
}
