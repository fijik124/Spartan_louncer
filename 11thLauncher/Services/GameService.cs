﻿using System;
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
    public class GameService: AbstractService, IGameService
    {
        private readonly IProcessAccessor _processAccessor;
        private readonly IClipboardAccessor _clipboardAccessor;

        private readonly ISettingsService _settingsService;
        private readonly IAddonService _addonService;
        private readonly IParameterService _parameterService;
        private readonly ISecurityService _securityService;

        private bool? _runningAsAdmin;

        public LaunchSettings LaunchSettings { get; }

        public GameService(IProcessAccessor processAccessor, IClipboardAccessor clipboardAccessor, ILogger logger, ISettingsService settingsService, 
            IAddonService addonService, IParameterService parameterService, ISecurityService securityService) : base(logger)
        {
            _processAccessor = processAccessor;
            _clipboardAccessor = clipboardAccessor;

            _settingsService = settingsService;
            _addonService = addonService;
            _parameterService = parameterService;
            _securityService = securityService;

            LaunchSettings = new LaunchSettings();
        }

        public LaunchGameResult StartGame()
        {
            var result = LaunchGameResult.GameLaunched;
            var gameParams = string.Join(" ", GetParameterArguments(), GetAddonArguments(), GetConnectionArguments()).Trim();
            var elevation = RunningAsAdmin();
            var steamRunning = SteamRunning();

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

            if (!elevation)
                result = LaunchGameResult.NoElevation;
            if (!steamRunning)
                result  = result | LaunchGameResult.NoSteam;

            if (!result.HasFlag(LaunchGameResult.NoElevation) && !result.HasFlag(LaunchGameResult.NoSteam))
            {
                _processAccessor.Start(process);
                Logger.LogInfo("GameService", $"Starting ArmA 3 in {LaunchSettings.Platform}");
            }
            else
            {
                Logger.LogInfo("GameService", $"Unable to launch game: {result}");
            }

            return result;
        }

        public void CopyLaunchShortcut()
        {
            var shortcut = string.Join(" ", 
                GetGameExecutablePath(), 
                GetParameterArguments(), 
                GetAddonArguments(), 
                GetConnectionArguments()).Trim();

            _clipboardAccessor.SetText(shortcut);
            Logger.LogInfo("GameService", "Launch shortcut copied to clipboard");
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
                Logger.LogException("GameService", "Error checking user elevation", e);
            }
            
            Logger.LogDebug("GameService", $"Checked if the program is running with elevation: {_runningAsAdmin}");

            return _runningAsAdmin.Value;
        }

        public bool SteamRunning()
        {
            bool result;

            try
            {
                result = _processAccessor.GetProcessesByName(ApplicationConfig.SteamProcess).Length != 0;
            }
            catch (Exception e)
            {
                result = false;
                Logger.LogException("GameService", "Error checking if steam is running", e);
            }

            Logger.LogDebug("GameService", $"Checked if steam is running: {result}");

            return result;
        }

        private string GetAddonArguments()
        {
            string addonParams = string.Join(";", _addonService.Addons.Where(a => a.IsEnabled).Select(a => a.Name));

            if (addonParams.Length > 0)
            {
                addonParams = "-mod=" + addonParams;
            }

            Logger.LogDebug("GameService", $"Addon arguments: {addonParams}");
            return addonParams;
        }

        private string GetParameterArguments()
        {
            var gameParams = string.Join(" ", _parameterService.Parameters
                .Where(p => p.IsEnabled && (p.Platform == ParameterPlatform.Any || (int)p.Platform == (int)LaunchSettings.Platform))
                .Select(p => p.LaunchString));

            Logger.LogDebug("GameService", $"Parameter arguments: {gameParams}");
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

                Logger.LogDebug("GameService", $"Connection arguments: {serverParams}");

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
