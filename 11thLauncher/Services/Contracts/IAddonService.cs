using Caliburn.Micro;
using _11thLauncher.Models;

namespace _11thLauncher.Services.Contracts
{
    public interface IAddonService
    {
        /// <summary>
        /// Returns the addons currently stored in the service.
        /// </summary>
        /// <returns>Addons currently stored</returns>
        BindableCollection<Addon> GetAddons();

        /// <summary>
        /// Find addons on the specified path.
        /// </summary>
        /// <param name="path">Path used to look for addons</param>
        /// <returns>Addons found</returns>
        BindableCollection<Addon> ReadAddons(string path);

        /// <summary>
        /// Opens a browser window in the addon folder.
        /// </summary>
        /// <param name="addon">Addon with the path to open</param>
        void BrowseAddonFolder(Addon addon);
    }
}
