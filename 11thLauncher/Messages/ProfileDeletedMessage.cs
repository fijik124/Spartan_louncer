using _11thLauncher.Models;

namespace _11thLauncher.Messages
{
    public class ProfileDeletedMessage
    {
        public UserProfile Profile;

        public ProfileDeletedMessage(UserProfile profile)
        {
            Profile = profile;
        }
    }
}
