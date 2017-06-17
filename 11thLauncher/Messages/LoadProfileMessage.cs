using Caliburn.Micro;
using _11thLauncher.Models;

namespace _11thLauncher.Messages
{
    /// <summary>
    /// A message regarding a change in a profile. To be handled by an event aggregator.
    /// </summary>
    public class LoadProfileMessage
    {
        /// <summary>
        /// Profile loaded.
        /// </summary>
        public UserProfile Profile;

        /// <summary>
        /// Addon settings for the profile.
        /// </summary>
        public BindableCollection<Addon> Addons;

        /// <summary>
        /// Parameter settings for the profile.
        /// </summary>
        public BindableCollection<LaunchParameter> Parameters;

        /// <summary>
        /// Game config for the profile.
        /// </summary>
        public GameConfig GameConfig;

        /// <summary>
        /// Creates a new instance of the <see cref="_11thLauncher.Messages.LoadProfileMessage"/> class, with the specified settings.
        /// </summary>
        public LoadProfileMessage(UserProfile profile, BindableCollection<Addon> addons, 
            BindableCollection<LaunchParameter>  parameters, GameConfig gameConfig)
        {
            Profile = profile;
            Addons = addons;
            Parameters = parameters;
            GameConfig = gameConfig;
        }
    }
}
