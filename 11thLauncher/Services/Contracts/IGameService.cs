using _11thLauncher.Models;

namespace _11thLauncher.Services.Contracts
{
    public interface IGameService
    {
        LaunchSettings LaunchSettings { get; }

        void StartGame();

        void CopyLaunchShortcut();

        bool RunningAsAdmin();

        bool SteamRunning();
    }
}
