using System.Collections.Generic;
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
        List<Repository> ReadRepositories(string arma3SyncPath);
    }
}
