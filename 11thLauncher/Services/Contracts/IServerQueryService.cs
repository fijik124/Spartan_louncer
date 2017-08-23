using _11thLauncher.Models;

namespace _11thLauncher.Services.Contracts
{
    public interface IServerQueryService
    {
        /// <summary>
        /// Query the given server to get it's current status.
        /// </summary>
        /// <param name="server">Server to check</param>
        void GetServerStatus(Server server);
    }
}