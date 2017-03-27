using _11thLauncher.Model.Profile;

namespace _11thLauncher.Messages
{
    public enum ProfileAction { Loaded, Updated }

    public class ProfileMessage
    {
        public UserProfile Profile;
        public ProfileAction Action;

        public ProfileMessage(ProfileAction action, UserProfile profile = null)
        {
            Profile = profile;
            Action = action;
        }
    }
}
