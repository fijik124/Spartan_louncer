using System.IO;
using Caliburn.Micro;
using Newtonsoft.Json;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.Services
{
    public class ProfileService : IProfileService
    {
        public void Write(UserProfile profile, BindableCollection<Addon> addons, 
            BindableCollection<LaunchParameter> parameters, LaunchSettings launchSettings)
        {
            ProfileFile profileFile = new ProfileFile
            {
                Profile = profile,
                Addons = addons ?? new BindableCollection<Addon>(),
                Parameters = parameters ?? new BindableCollection<LaunchParameter>(),
                LaunchSettings = launchSettings ?? new LaunchSettings()
            };

            if (!Directory.Exists(Constants.ProfilesPath))
            {
                Directory.CreateDirectory(Constants.ProfilesPath);
            }

            File.WriteAllText(Path.Combine(Constants.ProfilesPath, profile.Id + ".json"), JsonConvert.SerializeObject(profileFile, Formatting.Indented));
        }

        public void Read(UserProfile profile, out BindableCollection<Addon> addons,
            out BindableCollection<LaunchParameter> parameters, out LaunchSettings launchSettings)
        {
            ProfileFile profileFile = new ProfileFile { Profile = profile };
            JsonConvert.PopulateObject(File.ReadAllText(Path.Combine(Constants.ProfilesPath, profile.Id + ".json")), profileFile); //TODO TRYCTACH
            addons = profileFile.Addons;
            parameters = profileFile.Parameters;
            launchSettings = profileFile.LaunchSettings;
        }

        public void DeleteProfile(UserProfile profile)
        {
            var profileFile = Path.Combine(Constants.ProfilesPath, profile.Id + ".json");
            if (File.Exists(profileFile))
            {
                File.Delete(profileFile);
            }
        }
    }
}
