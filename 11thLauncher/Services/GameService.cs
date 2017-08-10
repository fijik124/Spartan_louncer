using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.Services
{
    public class GameService: IGameService
    {
        private readonly ISettingsService _settingsService;
        private readonly IAddonService _addonService;
        private readonly IParameterService _parameterService;

        public LaunchSettings LaunchSettings { get; set; }

        public GameService(ISettingsService settingsService, IAddonService addonService, IParameterService parameterService)
        {
            _settingsService = settingsService;
            _addonService = addonService;
            _parameterService = parameterService;

            LaunchSettings = new LaunchSettings();
        }

        public void StartGame(LaunchOption launchOption, LaunchPlatform platform, string server, string port, string password)
        {
            var gameParams = string.Join(" ", 
                GetParameterArguments(platform),
                GetAddonArguments(), 
                GetConnectionArguments(launchOption, server, port, password)).Trim();

            Process process = new Process
            {
                StartInfo =
                {
                    FileName = "" + "\\" + GetGameExecutablePath(platform),
                    Verb = "runas"
                }
            };
            
            if (gameParams.Length > 0)
            {
                process.StartInfo.Arguments = gameParams;
            }
            
            process.Start();
        }

        public void CopyLaunchShortcut(LaunchOption launchOption, LaunchPlatform platform, string server, string port, string password)
        {
            var shortcut = string.Join(" ", 
                GetGameExecutablePath(platform), 
                GetParameterArguments(platform), 
                GetAddonArguments(), 
                GetConnectionArguments(launchOption, server, port, password)).Trim();
            Clipboard.SetText(shortcut);
        }

        private string GetAddonArguments()
        {
            string addonParams = string.Join(";", _addonService.GetAddons().Where(a => a.IsEnabled).Select(a => a.Name));

            if (addonParams.Length > 0)
            {
                addonParams = "-mod=" + addonParams;
            }

            return addonParams;
        }

        private string GetParameterArguments(LaunchPlatform platform)
        {
            return string.Join(" ", _parameterService.Parameters
                .Where(p => p.IsEnabled && (p.Platform == ParameterPlatform.Any || (int)p.Platform == (int)platform))
                .Select(p => p.LaunchString));
        }

        private string GetConnectionArguments(LaunchOption launchOption, string server, string port, string password)
        {
            var serverParams = "";

            if (launchOption == LaunchOption.JoinServer)
            {
                if (server.Length > 0)
                    serverParams += " -connect=" + server;
                if (port.Length > 0)
                    serverParams += " -port=" + port;
                if (password.Length > 0)
                    serverParams += " -password=" + password;
            }

            return serverParams;
        }

        private string GetGameExecutablePath(LaunchPlatform platform)
        {
            return Path.Combine(_settingsService.ApplicationSettings.Arma3Path, platform == LaunchPlatform.X86 ? Constants.GameExecutable32 : Constants.GameExecutable64);
        }
    }
}
