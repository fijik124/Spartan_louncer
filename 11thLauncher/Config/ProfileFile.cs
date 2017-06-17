using System.IO;
using Newtonsoft.Json;
using Caliburn.Micro;
using _11thLauncher.Models;

namespace _11thLauncher.Config
{
    public class ProfileFile
    {
        public UserProfile Profile;
        public BindableCollection<Addon> Addons;
        public BindableCollection<LaunchParameter> Parameters;
        public GameConfig GameConfig;

        public void Read()
        {
            JsonConvert.PopulateObject(File.ReadAllText(Path.Combine(Constants.ProfilesPath, Profile.Id + ".json")), this);
        }

        public void Write()
        {
            if (!Directory.Exists(Constants.ProfilesPath))
            {
                Directory.CreateDirectory(Constants.ProfilesPath);
            }

            File.WriteAllText(Path.Combine(Constants.ProfilesPath, Profile.Id + ".json"), JsonConvert.SerializeObject(this));
        }
    }
}
