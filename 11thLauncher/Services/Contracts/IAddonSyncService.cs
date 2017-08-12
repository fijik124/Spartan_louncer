using Caliburn.Micro;
using _11thLauncher.Models;

namespace _11thLauncher.Services.Contracts
{
    public interface IAddonSyncService
    {
        /// <summary>
        /// Get a list of repositories configured from the given Arma3Sync folder
        /// </summary>
        /// <param name="arma3SyncPath">Installation path of Arma3Sync</param>
        /// <returns>List of repositories</returns>
        BindableCollection<Repository> ReadRepositories(string arma3SyncPath);

        /// <summary>
        /// Checks the status of the given repository against the server
        /// </summary>
        /// <param name="addonSyncPath">Installation path of addonsync</param>
        /// <param name="javaPath">Java installation path. Empty if java is used from PATH</param>
        /// <param name="repository">Repository to check</param>
        void CheckRepository(string addonSyncPath, string javaPath, Repository repository);

        /// <summary>
        /// Check if Java is present on PATH and the installed version
        /// </summary>
        /// <returns>Version of Java on path</returns>
        string GetJavaInSystem();

        /// <summary>
        /// Returns the addon sync program path
        /// </summary>
        /// <returns>Path of the program in the system</returns>
        string GetAddonSyncPath();

        bool AddonSyncPathIsValid(string path);

        /// <summary>
        /// Starts the addon sync program
        /// </summary>
        void StartAddonSync(string arma3SyncPath);
    }
}
