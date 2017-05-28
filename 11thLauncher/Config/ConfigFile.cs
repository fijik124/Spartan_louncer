using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Caliburn.Micro;
using Newtonsoft.Json;
using _11thLauncher.Model.Settings;

namespace _11thLauncher.Config
{
    public class ConfigFile
    {
        public ApplicationSettings ApplicationSettings;
        public Guid DefaultProfileId;
        public Dictionary<Guid, string> Profiles;
        public BindableCollection<Model.Server.Server> Servers;

        public ConfigFile()
        {
            ApplicationSettings = new ApplicationSettings();
            DefaultProfileId = Guid.Empty;
            Profiles = new Dictionary<Guid, string>();
            Servers = new BindableCollection<Model.Server.Server>();
        }

        public void ReadDefault()
        {
            JsonConvert.PopulateObject(Encoding.Default.GetString(Properties.Resources.servers), Servers);
        }

        public void Read()
        {
            JsonConvert.PopulateObject(File.ReadAllText(Path.Combine(Constants.ConfigPath, Constants.ConfigFileName)), this);
        }

        public void Write()
        {
            if (Directory.Exists(Constants.ConfigPath))
            {
                Directory.CreateDirectory(Constants.ConfigPath);
            }

            File.WriteAllText(Path.Combine(Constants.ConfigPath, Constants.ConfigFileName), JsonConvert.SerializeObject(this));
        }
    }
}