using _11thLauncher.Models;

namespace _11thLauncher.Services.Contracts
{
    public interface IGameService
    {
        LaunchSettings LaunchSettings { get; set; }

        void StartGame(LaunchOption launchOption, LaunchPlatform platform, string server, string port, string password);

        void CopyLaunchShortcut(LaunchOption launchOption, LaunchPlatform platform, string server, string port, string password);
    }
}
