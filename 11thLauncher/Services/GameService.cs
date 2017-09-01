using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using _11thLauncher.Accessors.Contracts;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;
using _11thLauncher.Util;

namespace _11thLauncher.Services
{
    public class GameService: IGameService
    {
        private readonly IProcessAccessor _processAccessor;
        private readonly IClipboardAccessor _clipboardAccessor;
        private readonly ILogger _logger;

        private readonly ISettingsService _settingsService;
        private readonly IAddonService _addonService;
        private readonly IParameterService _parameterService;
        private readonly ISecurityService _securityService;

        private bool? _runningAsAdmin;

        public LaunchSettings LaunchSettings { get; }

        public GameService(IProcessAccessor processAccessor, IClipboardAccessor clipboardAccessor, ILogger logger, ISettingsService settingsService, 
            IAddonService addonService, IParameterService parameterService, ISecurityService securityService)
        {
            _processAccessor = processAccessor;
            _clipboardAccessor = clipboardAccessor;
            _logger = logger;

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
                    FileName = GetGameExecutablePath(),
                    Verb = RunningAsAdmin() ? "runas" : string.Empty
                }
            };
            
            if (gameParams.Length > 0)
            {
                process.StartInfo.Arguments = gameParams;
            }

            _processAccessor.Start(process);
            _logger.LogInfo("GameService", $"Starting ArmA 3 in {LaunchSettings.Platform}");
        }

        public void CopyLaunchShortcut()
        {
            var shortcut = string.Join(" ", 
                GetGameExecutablePath(), 
                GetParameterArguments(), 
                GetAddonArguments(), 
                GetConnectionArguments()).Trim();

            _clipboardAccessor.SetText(shortcut);
            _logger.LogInfo("GameService", "Launch shortcut copied to clipboard");
        }

        public bool RunningAsAdmin()
        {
            if (_runningAsAdmin != null) return _runningAsAdmin.Value;

            try
            {
                _runningAsAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception e)
            {
                _runningAsAdmin = false;
                _logger.LogException("GameService", "Error checking user elevation", e);
            }
            
            _logger.LogDebug("GameService", $"Checked if the program is running with elevation: {_runningAsAdmin}");

            return _runningAsAdmin.Value;
        }

        private string GetAddonArguments()
        {
            string addonParams = string.Join(";", _addonService.Addons.Where(a => a.IsEnabled).Select(a => a.Name));

            if (addonParams.Length > 0)
            {
                addonParams = "-mod=" + addonParams;
            }

            _logger.LogDebug("GameService", $"Addon arguments: {addonParams}");
            return addonParams;
        }

        private string GetParameterArguments()
        {
            var gameParams = string.Join(" ", _parameterService.Parameters
                .Where(p => p.IsEnabled && (p.Platform == ParameterPlatform.Any || (int)p.Platform == (int)LaunchSettings.Platform))
                .Select(p => p.LaunchString));

            _logger.LogDebug("GameService", $"Parameter arguments: {gameParams}");
            return gameParams;
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

                _logger.LogDebug("GameService", $"Connection arguments: {serverParams}");

                if (LaunchSettings.Password.Length > 0)
                    serverParams += " -password=" + _securityService.DecryptPassword(LaunchSettings.Password);
            }

            return serverParams;
        }

        private string GetGameExecutablePath()
        {
            return Path.Combine(_settingsService.ApplicationSettings.Arma3Path, LaunchSettings.Platform == LaunchPlatform.X86 ? 
                ApplicationConfig.GameExecutable32 :
                ApplicationConfig.GameExecutable64);
        }
    }
}
