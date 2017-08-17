using _11thLauncher.Models;

namespace _11thLauncher.Services.Contracts
{
    public interface IUpdaterService
    {
        /// <summary>
        /// Delete the program updater if it exists in the system
        /// </summary>
        void RemoveUpdater();

        UpdateCheckResult CheckUpdates();
    }
}
