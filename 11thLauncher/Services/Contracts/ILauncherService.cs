using System.Collections.ObjectModel;
using _11thLauncher.Models;

namespace _11thLauncher.Services.Contracts
{
    public interface ILauncherService
    {
        LaunchSettings LaunchSettings { get; set; }

        void StartGame(Collection<Addon> addons, Collection<LaunchParameter> parameters,
            LaunchOption launchOption, LaunchPlatform platform, string server, string port, string password);
    }
}
