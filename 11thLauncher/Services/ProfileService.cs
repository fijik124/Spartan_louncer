using System.IO;
using Caliburn.Micro;
using Newtonsoft.Json;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.Services
{
    public class ProfileService : IProfileService
    {
        public void WriteProfile(UserProfile profile, BindableCollection<Addon> addons, 
            BindableCollection<LaunchParameter> parameters, GameConfig gameConfig)
        {
            ProfileFile profileFile = new ProfileFile
            {
                Profile = profile,
                Addons = addons ?? new BindableCollection<Addon>(),
                Parameters = parameters ?? new BindableCollection<LaunchParameter>(),
                GameConfig = gameConfig ?? new GameConfig()
            };

            if (!Directory.Exists(Constants.ProfilesPath))
            {
                Directory.CreateDirectory(Constants.ProfilesPath);
            }

            File.WriteAllText(Path.Combine(Constants.ProfilesPath, profile.Id + ".json"), JsonConvert.SerializeObject(this));
        }

        public void ReadProfile(UserProfile profile, out BindableCollection<Addon> addons,
            out BindableCollection<LaunchParameter> parameters, out GameConfig gameConfig)
        {
            ProfileFile profileFile = new ProfileFile { Profile = profile };
            JsonConvert.PopulateObject(File.ReadAllText(Path.Combine(Constants.ProfilesPath, profile.Id + ".json")), profileFile); //TODO TRYCTACH
            addons = profileFile.Addons;
            parameters = profileFile.Parameters;
            gameConfig = profileFile.GameConfig;
        }
    }
}
