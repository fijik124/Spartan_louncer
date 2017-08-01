using Caliburn.Micro;

namespace _11thLauncher.Models
{
    public class ProfileFile
    {
        public UserProfile Profile;
        public BindableCollection<Addon> Addons;
        public BindableCollection<LaunchParameter> Parameters;
        public LaunchSettings LaunchSettings;
    }
}
