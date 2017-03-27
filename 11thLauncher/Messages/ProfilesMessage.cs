using Caliburn.Micro;
using _11thLauncher.Model.Profile;

namespace _11thLauncher.Messages
{
    public enum ProfilesAction { Added, Deleted }

    public class ProfilesMessage
    {
        public BindableCollection<UserProfile> Profiles;
        public ProfilesAction Action;

        public ProfilesMessage(ProfilesAction action, BindableCollection<UserProfile> profiles)
        {
            Profiles = profiles;
            Action = action;
        }
    }
}
