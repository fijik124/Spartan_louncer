using Caliburn.Micro;
using _11thLauncher.Models;

namespace _11thLauncher.Services.Contracts
{
    public interface IAddonService
    {
        /// <summary>
        /// Addons on the launcher.
        /// </summary>
        BindableCollection<Addon> Addons { get; set; }

        /// <summary>
        /// Find addons on the specified path.
        /// </summary>
        /// <param name="path">Path used to look for addons</param>
        /// <returns>Addons found</returns>
        void ReadAddons(string path);

        /// <summary>
        /// Opens a browser window in the addon folder.
        /// </summary>
        /// <param name="addon">Addon with the path to open</param>
        void BrowseAddonFolder(Addon addon);

        /// <summary>
        /// Opens a web browser window with the addon website.
        /// </summary>
        /// <param name="addon">Addon for which the site should be opened</param>
        void BrowseAddonWebsite(Addon addon);
    }
}
