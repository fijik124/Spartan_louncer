using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using _11thLauncher.Accessors.Contracts;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.Services
{
    public class AddonService : IAddonService
    {
        private readonly IFileAccessor _fileAccessor;

        public BindableCollection<Addon> Addons { get; set; }

        public AddonService(IFileAccessor fileAccessor)
        {
            _fileAccessor = fileAccessor;
            Addons = new BindableCollection<Addon>();
        }

        public BindableCollection<Addon> ReadAddons(string arma3Path)
        {
            if (string.IsNullOrEmpty(arma3Path)) return new BindableCollection<Addon>();
            if (Addons.Count != 0) return Addons; //Already read

            string[] directories = _fileAccessor.GetDirectories(arma3Path, Constants.AddonSubfolderName, SearchOption.AllDirectories);
            foreach (string directory in directories)
            {
                if (Constants.VanillaAddons.Contains(_fileAccessor.GetParent(directory).Name.ToLower())) continue;
                if (_fileAccessor.GetFiles(directory, Constants.AddonFilePattern).Length == 0) continue;
                int pathindex = directory.IndexOf(arma3Path, StringComparison.Ordinal) + arma3Path.Length + 1;
                string addonName = directory.Substring(pathindex, (directory.Length - pathindex) - (Constants.AddonSubfolderName.Length + 1));

                var addon = new Addon(_fileAccessor.GetParent(directory).FullName, addonName);
                ReadMetaData(addon);
                Addons.Add(addon);  
            }

            return Addons;
        }

        public void BrowseAddonFolder(Addon addon)
        {
            if (_fileAccessor.DirectoryExists(addon.Path))
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

        private void ReadMetaData(Addon addon)
        {
            var metaFile = Path.Combine(addon.Path, Constants.AddonMetaDataFile);
            if (!_fileAccessor.FileExists(metaFile)) return;

            addon.MetaData = new AddonMetaData();

            try
            {
                var properties = _fileAccessor.ReadAllLines(metaFile);
                foreach (var line in properties)
                {
                    try
                    {
                        var separator = line.IndexOf('=');
                        if (separator <= 0)
                            continue;
                        string param = line.Remove(separator).TrimEnd();
                        string value = line.Substring(separator + 1).TrimStart().TrimEnd(';');
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
                    catch (Exception)
                    {
                        // continue
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
