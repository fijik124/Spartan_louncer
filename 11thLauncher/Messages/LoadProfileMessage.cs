using _11thLauncher.Models;

namespace _11thLauncher.Messages
{
    public class LoadProfileMessage
    {
        public UserProfile Profile;

        public LoadProfileMessage(UserProfile profile)
        {
            Profile = profile;
        }
    }
}
