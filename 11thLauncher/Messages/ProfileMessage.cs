using _11thLauncher.Models;

namespace _11thLauncher.Messages
{
    /// <summary>
    /// Type of action that triggered this <see cref="ProfileMessage"/>.
    /// </summary>
    public enum ProfileAction { Loaded, Updated }

    /// <summary>
    /// A message regarding a change in a profile. To be handled by an event aggregator.
    /// </summary>
    public class ProfileMessage
    {
        /// <summary>
        /// Profile changed by the action.
        /// </summary>
        public UserProfile Profile;

        /// <summary>
        /// Action that triggered the message.
        /// </summary>
        public ProfileAction Action;

        /// <summary>
        /// Creates a new instance of the <see cref="ProfileMessage"/> class, with the specified action and associated profile.
        /// </summary>
        public ProfileMessage(ProfileAction action, UserProfile profile = null)
        {
            Profile = profile;
            Action = action;
        }
    }
}
