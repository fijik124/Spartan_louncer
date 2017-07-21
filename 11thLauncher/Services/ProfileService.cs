using Caliburn.Micro;
using _11thLauncher.Config;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.Model.Profile
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

            profileFile.Write();
        }

        public void ReadProfile(UserProfile profile, out BindableCollection<Addon> addons,
            out BindableCollection<LaunchParameter> parameters, out GameConfig gameConfig)
        {
            ProfileFile profileFile = new ProfileFile { Profile = profile };
            profileFile.Read();
            addons = profileFile.Addons;
            parameters = profileFile.Parameters;
            gameConfig = profileFile.GameConfig;
        }
    }
}
