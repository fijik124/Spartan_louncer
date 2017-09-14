using Caliburn.Micro;
using _11thLauncher.Models;

namespace _11thLauncher.Services.Contracts
{
    public interface IProfileService
    {
        void Write(UserProfile profile, BindableCollection<Addon> addons, BindableCollection<LaunchParameter> parameters, LaunchSettings launchSettings);

        void Read(UserProfile profile, out BindableCollection<Addon> addons, out BindableCollection<LaunchParameter> parameters, out LaunchSettings launchSetting);

        void PortLegacyProfiles(BindableCollection<UserProfile> profiles);

        void DeleteProfile(UserProfile profile);
    }
}
