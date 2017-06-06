using Caliburn.Micro;
using _11thLauncher.Model.Addons;

namespace _11thLauncher.Services
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
    }
}
