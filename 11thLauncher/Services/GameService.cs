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
        private readonly ISecurityService _securityService;

        public LaunchSettings LaunchSettings { get; set; }

        public GameService(ISettingsService settingsService, IAddonService addonService, IParameterService parameterService, ISecurityService securityService)
        {
            _settingsService = settingsService;
            _addonService = addonService;
            _parameterService = parameterService;
            _securityService = securityService;

            LaunchSettings = new LaunchSettings();
        }

        public void StartGame()
        {
            var gameParams = string.Join(" ", GetParameterArguments(), GetAddonArguments(), GetConnectionArguments()).Trim();

            Process process = new Process
            {
                StartInfo =
                {
                    FileName = "" + "\\" + GetGameExecutablePath(),
                    Verb = "runas"
                }
            };
            
            if (gameParams.Length > 0)
            {
                process.StartInfo.Arguments = gameParams;
            }
            
            process.Start();
        }

        public void CopyLaunchShortcut()
        {
            var shortcut = string.Join(" ", 
                GetGameExecutablePath(), 
                GetParameterArguments(), 
                GetAddonArguments(), 
                GetConnectionArguments()).Trim();
            Clipboard.SetText(shortcut);
        }

        private string GetAddonArguments()
        {
            string addonParams = string.Join(";", _addonService.Addons.Where(a => a.IsEnabled).Select(a => a.Name));

            if (addonParams.Length > 0)
            {
                addonParams = "-mod=" + addonParams;
            }

            return addonParams;
        }

        private string GetParameterArguments()
        {
            return string.Join(" ", _parameterService.Parameters
                .Where(p => p.IsEnabled && (p.Platform == ParameterPlatform.Any || (int)p.Platform == (int)LaunchSettings.Platform))
                .Select(p => p.LaunchString));
        }

        private string GetConnectionArguments()
        {
            var serverParams = "";
            if (LaunchSettings.LaunchOption != LaunchOption.JoinServer) return serverParams;

            if (LaunchSettings.Server.Length > 0)
            {
                serverParams += "-connect=" + LaunchSettings.Server;

                if (LaunchSettings.Port.Length > 0)
                    serverParams += " -port=" + LaunchSettings.Port;
                if (LaunchSettings.Password.Length > 0)
                    serverParams += " -password=" + _securityService.DecryptPassword(LaunchSettings.Password);
            }

            return serverParams;
        }

        private string GetGameExecutablePath()
        {
            return Path.Combine(_settingsService.ApplicationSettings.Arma3Path, LaunchSettings.Platform == LaunchPlatform.X86 ? Constants.GameExecutable32 : Constants.GameExecutable64);
        }
    }
}
