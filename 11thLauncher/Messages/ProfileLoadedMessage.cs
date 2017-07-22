using Caliburn.Micro;
using _11thLauncher.Models;

namespace _11thLauncher.Messages
{
    /// <summary>
    /// A message regarding a change in a profile. To be handled by an event aggregator.
    /// </summary>
    public class ProfileLoadedMessage
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
        /// Launch config for the profile.
        /// </summary>
        public LaunchSettings LaunchSettings;

        /// <summary>
        /// Creates a new instance of the <see cref="ProfileLoadedMessage"/> class, with the specified settings.
        /// </summary>
        public ProfileLoadedMessage(UserProfile profile, BindableCollection<Addon> addons, 
            BindableCollection<LaunchParameter>  parameters, LaunchSettings launchSettings)
        {
            Profile = profile;
            Addons = addons;
            Parameters = parameters;
            LaunchSettings = launchSettings;
        }
    }
}
