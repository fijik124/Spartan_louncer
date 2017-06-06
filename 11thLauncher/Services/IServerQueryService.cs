using _11thLauncher.Model.Server;

namespace _11thLauncher.Services
{
    public interface IServerQueryService
    {
        /// <summary>
        /// Query the given server to get it's current status.
        /// </summary>
        /// <param name="server">Server to check</param>
        void CheckServerStatus(Server server);

        /// <summary>
        /// Query the given server to get it's game version.
        /// </summary>
        /// <param name="server"></param>
        /// <returns>Server game version</returns>
        string GetServerVersion(Server server);
    }
}