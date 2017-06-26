using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Newtonsoft.Json;
using _11thLauncher.Models;

namespace _11thLauncher.Config
{
    public class ConfigFile
    {
        public ApplicationSettings ApplicationSettings;
        public Guid DefaultProfileId;
        public Dictionary<Guid, string> Profiles;
        public BindableCollection<Server> Servers;

        public ConfigFile()
        {
            ApplicationSettings = new ApplicationSettings();
            DefaultProfileId = Guid.Empty;
            Profiles = new Dictionary<Guid, string>();
            Servers = new BindableCollection<Server>();
        }

        public void LoadDefaultServers()
        {
            //Add default servers if they are not present
            BindableCollection<Server> defaultServers = new BindableCollection<Server>();
            JsonConvert.PopulateObject(Encoding.Default.GetString(Properties.Resources.servers), defaultServers);
            Servers = new BindableCollection<Server>(Servers.Union(defaultServers));
        }

        public void Read()
        {
            JsonConvert.PopulateObject(File.ReadAllText(Path.Combine(Constants.ConfigPath, Constants.ConfigFileName)), this);
        }

        public void Write()
        {
            //If no config directory exists, create it
            if (!Directory.Exists(Constants.ConfigPath))
            {
                Directory.CreateDirectory(Constants.ConfigPath);
            }

            File.WriteAllText(Path.Combine(Constants.ConfigPath, Constants.ConfigFileName), JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}