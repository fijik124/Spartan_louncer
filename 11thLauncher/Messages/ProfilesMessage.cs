using Caliburn.Micro;
using _11thLauncher.Models;

namespace _11thLauncher.Messages
{
    /// <summary>
    /// Type of action that triggered this <see cref="ProfilesMessage"/>.
    /// </summary>
    public enum ProfilesAction { Added, Deleted }

    /// <summary>
    /// A message regarding a change in the managed profiles. To be handled by an event aggregator.
    /// </summary>
    public class ProfilesMessage
    {
        /// <summary>
        /// Profiles changed by the action.
        /// </summary>
        public BindableCollection<UserProfile> Profiles;

        /// <summary>
        /// Action that triggered the message.
        /// </summary>
        public ProfilesAction Action;

        /// <summary>
        /// Creates a new instance of the <see cref="ProfilesMessage"/> class, with the specified action and associated profiles.
        /// </summary>
        public ProfilesMessage(ProfilesAction action, BindableCollection<UserProfile> profiles)
        {
            Profiles = profiles;
            Action = action;
        }
    }
}
