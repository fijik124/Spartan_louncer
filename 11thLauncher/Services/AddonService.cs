using System;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using _11thLauncher.Accessors.Contracts;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;
using _11thLauncher.Util;

namespace _11thLauncher.Services
{
    public class AddonService : AbstractService, IAddonService
    {
        private readonly IFileAccessor _fileAccessor;
        private readonly IProcessAccessor _processAccessor;

        public BindableCollection<Addon> Addons { get; set; }

        public AddonService(IFileAccessor fileAccessor, IProcessAccessor processAccessor, ILogger logger) : base(logger)
        {
            _fileAccessor = fileAccessor;
            _processAccessor = processAccessor;

            Addons = new BindableCollection<Addon>();
        }

        public void ReadAddons(string arma3Path)
        {
            if (string.IsNullOrEmpty(arma3Path) || Addons.Count != 0)
            {
                Logger.LogDebug("AddonService", "No game path defined or addons already read, skipping reading addons");
                return;
            };

            string[] directories = _fileAccessor.GetDirectories(arma3Path, ApplicationConfig.AddonSubfolderName, SearchOption.AllDirectories);
            foreach (string directory in directories)
            {
                if (ApplicationConfig.VanillaAddons.Contains(_fileAccessor.GetParent(directory).Name.ToLower())) continue;
                if (_fileAccessor.GetFiles(directory, ApplicationConfig.AddonFilePattern).Length == 0) continue;
                int pathindex = directory.IndexOf(arma3Path, StringComparison.Ordinal) + arma3Path.Length + 1;
                string addonName = directory.Substring(pathindex, (directory.Length - pathindex) - (ApplicationConfig.AddonSubfolderName.Length + 1));

                var addon = new Addon(_fileAccessor.GetParent(directory).FullName, addonName);
                ReadMetaData(addon);
                Addons.Add(addon);  
            }

            Logger.LogDebug("AddonService", $"Addons read from game path successfully, number found was: {Addons.Count}");
        }

        public void BrowseAddonFolder(Addon addon)
        {
            if (!_fileAccessor.DirectoryExists(addon.Path))
            {
                Logger.LogDebug("AddonService", $"Unable to open addon folder for {addon.Name}");
                return;
            }

            _processAccessor.StartProcess(addon.Path);
            Logger.LogDebug("AddonService", $"Opening addon folder for '{addon.Name}'");
        }

        public void BrowseAddonWebsite(Addon addon)
        {
            if (string.IsNullOrEmpty(addon.MetaData?.Action))
            {
                Logger.LogDebug("AddonService", $"Unable to open addon website for {addon.Name}");
                return;
            }

            _processAccessor.StartProcess(addon.MetaData.Action);
            Logger.LogDebug("AddonService", $"Opening addon website for {addon.Name}");
        }

        private void ReadMetaData(Addon addon)
        {
            var metaFile = Path.Combine(addon.Path, ApplicationConfig.AddonMetaDataFile);
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
            catch (Exception e)
            {
                addon.MetaData = null;
                Logger.LogException("AddonService", $"Error reading addon metadata for {addon.Name}", e);
            }
        }
    }
}
