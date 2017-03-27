using Caliburn.Micro;
using _11thLauncher.Model.Addon;

namespace _11thLauncher.Messages
{
    public enum AddonsAction { Added }

    public class AddonsMessage
    {
        public BindableCollection<Addon> Addons;
        public AddonsAction Action;

        public AddonsMessage(AddonsAction action, BindableCollection<Addon> addons)
        {
            Addons = addons;
            Action = action;
        }
    }
}
