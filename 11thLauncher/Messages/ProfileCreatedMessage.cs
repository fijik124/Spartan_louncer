using _11thLauncher.Models;

namespace _11thLauncher.Messages
{
    public class ProfileCreatedMessage
    {
        public UserProfile Profile;

        public ProfileCreatedMessage(UserProfile profile)
        {
            Profile = profile;
        }
    }
}
