using Caliburn.Micro;
using _11thLauncher.Model.Addon;

namespace _11thLauncher.Messages
{
    /// <summary>
    /// Type of action that triggered this <see cref="AddonsMessage"/>.
    /// </summary>
    public enum AddonsAction { Added }

    /// <summary>
    /// A message regarding a change in the management of addons. To be handled by an event aggregator.
    /// </summary>
    public class AddonsMessage
    {
        /// <summary>
        /// Collection of addons changed by the action.
        /// </summary>
        public BindableCollection<Addon> Addons;

        /// <summary>
        /// Action that triggered the message.
        /// </summary>
        public AddonsAction Action;

        /// <summary>
        /// Creates a new instance of the <see cref="AddonsMessage"/> class, with the specified action and associated addons.
        /// </summary>
        public AddonsMessage(AddonsAction action, BindableCollection<Addon> addons)
        {
            Addons = addons;
            Action = action;
        }
    }
}
