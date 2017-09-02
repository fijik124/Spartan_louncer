using System;
using System.Timers;
using System.Windows;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;
using Timer = System.Timers.Timer;

namespace _11thLauncher.ViewModels.Controls
{
    public class GameViewModel : PropertyChangedBase, IHandle<ProfileLoadedMessage>, IHandle<FillServerInfoMessage>, IHandle<ApplicationClosingMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISettingsService _settingsService;
        private readonly IGameService _gameService;
        private readonly ISecurityService _securityService;

        private bool _loadingProfile;
        private Timer _timer;
        private string _uacIcon;
        private string _uacTooltip;
        private string _steamIcon;
        private string _steamTooltip;

        public GameViewModel(IEventAggregator eventAggregator, ISettingsService settingsService, IGameService gameService, ISecurityService securityService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _settingsService = settingsService;
            _gameService = gameService;
            _securityService = securityService;

            _timer = new Timer(30000);
            _timer.Elapsed += CheckSteam;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        #region Message handling

        public void Handle(ProfileLoadedMessage message)
        {
            _loadingProfile = true;

            _gameService.LaunchSettings.LaunchOption = message.LaunchSettings.LaunchOption;
            NotifyOfPropertyChange(() => LaunchOption);

            _gameService.LaunchSettings.Platform = message.LaunchSettings.Platform;
            NotifyOfPropertyChange(() => Platform);

            _gameService.LaunchSettings.Server = message.LaunchSettings.Server;
            NotifyOfPropertyChange(() => Server);

            _gameService.LaunchSettings.Port = message.LaunchSettings.Port;
            NotifyOfPropertyChange(() => Port);

            _gameService.LaunchSettings.Password = message.LaunchSettings.Password;
            NotifyOfPropertyChange(() => Password);

            CheckElevation();
            CheckSteam(null, null);
        }

        public void Handle(FillServerInfoMessage message)
        {
            if (message.Server == null) return;

            _gameService.LaunchSettings.Server = message.Server.Address;
            NotifyOfPropertyChange(() => Server);

            _gameService.LaunchSettings.Port = message.Server.Port.ToString();
            NotifyOfPropertyChange(() => Port);

            _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
        }

        public void Handle(ApplicationClosingMessage message)
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }

        #endregion

        public LaunchOption LaunchOption
        {
            get => _gameService.LaunchSettings.LaunchOption;
            set
            {
                _gameService.LaunchSettings.LaunchOption = value;
                NotifyOfPropertyChange();
                if (_loadingProfile) return;
                _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
            }
        }

        public LaunchPlatform Platform
        {
            get => _gameService.LaunchSettings.Platform;
            set
            {
                _gameService.LaunchSettings.Platform = value;
                NotifyOfPropertyChange();
                _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
            }
        }

        public string Server
        {
            get => _gameService.LaunchSettings.Server;
            set
            {
                _gameService.LaunchSettings.Server = value;
                NotifyOfPropertyChange();
                _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
            }
        }

        public string Port
        {
            get => _gameService.LaunchSettings.Port;
            set
            {
                _gameService.LaunchSettings.Port = value;
                NotifyOfPropertyChange();
                _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
            }
        }

        public string Password
        {
            get => _securityService.DecryptPassword(_gameService.LaunchSettings.Password);
            set
            {
                _gameService.LaunchSettings.Password = _securityService.EncryptPassword(value);
                NotifyOfPropertyChange();
                if (_loadingProfile) { _loadingProfile = false; return; } //Avoid profile write when loading profile
                _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
            }
        }

        public string UacIcon
        {
            get => _uacIcon;
            set
            {
                _uacIcon = value;
                NotifyOfPropertyChange();
            }
        }

        public string UacTooltip
        {
            get => _uacTooltip;
            set
            {
                _uacTooltip = value;
                NotifyOfPropertyChange();
            }
        }

        public string SteamIcon
        {
            get => _steamIcon;
            set
            {
                _steamIcon = value;
                NotifyOfPropertyChange();
            }
        }

        public string SteamTooltip
        {
            get => _steamTooltip;
            set
            {
                _steamTooltip = value;
                NotifyOfPropertyChange();
            }
        }

        #region UI Actions

        public void ButtonLaunch()
        {
            _gameService.StartGame();

            switch (_settingsService.ApplicationSettings.StartAction)
            {
                case StartAction.Minimize:
                    Application.Current.MainWindow.WindowState = WindowState.Minimized;
                    break;
                case StartAction.Close:
                    Application.Current.Shutdown();
                    break;
                case StartAction.Nothing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ButtonCopyToClipboard()
        {
            _gameService.CopyLaunchShortcut();
        }

        #endregion

        private void CheckElevation()
        {
            var runningAsAdmin = _gameService.RunningAsAdmin();

            UacIcon = runningAsAdmin
                ? ApplicationConfig.UacIconEnabled
                : ApplicationConfig.UacIconDisabled;

            UacTooltip = runningAsAdmin
                ? Resources.Strings.S_BTN_UAC_ENABLED_TIP
                : Resources.Strings.S_BTN_UAC_DISABLED_TIP;
        }

        private void CheckSteam(object source, ElapsedEventArgs e)
        {
            var steamRunning = _gameService.SteamRunning();

            SteamIcon = steamRunning
                ? ApplicationConfig.SteamIconEnabled
                : ApplicationConfig.SteamIconDisabled;

            SteamTooltip = steamRunning
                ? Resources.Strings.S_BTN_STEAM_ENABLED_TIP
                : Resources.Strings.S_BTN_STEAM_DISABLED_TIP;
        }
    }
}
