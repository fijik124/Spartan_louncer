using Caliburn.Micro;
using _11thLauncher.Models;

namespace _11thLauncher.Messages
{
    public class ProfileAddedMessage
    {
        public BindableCollection<UserProfile> Profiles;

        public ProfileAddedMessage(BindableCollection<UserProfile> profiles)
        {
            Profiles = profiles;
        }
    }
}
