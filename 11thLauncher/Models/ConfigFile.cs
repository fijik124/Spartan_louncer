using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Newtonsoft.Json;

namespace _11thLauncher.Models
{
    public class ConfigFile
    {
        public ApplicationSettings ApplicationSettings;
        public Guid DefaultProfileId;
        public Dictionary<Guid, string> Profiles;
        [JsonIgnore]
        public BindableCollection<Server> Servers; //Custom servers disabled for now, so don't save

        public ConfigFile()
        {
            ApplicationSettings = new ApplicationSettings();
            DefaultProfileId = Guid.Empty;
            Profiles = new Dictionary<Guid, string>();
            Servers = new BindableCollection<Server>();
        }
    }
}