using System;
using System.Windows;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.ViewModels.Controls
{
    public class GameViewModel : PropertyChangedBase, IHandle<ProfileLoadedMessage>, IHandle<FillServerInfoMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISettingsService _settingsService;
        private readonly IGameService _gameService;
        private readonly ISecurityService _securityService;

        private bool _loadingProfile;

        public GameViewModel(IEventAggregator eventAggregator, ISettingsService settingsService, IGameService gameService, ISecurityService securityService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _settingsService = settingsService;
            _gameService = gameService;
            _securityService = securityService;
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

        #region UI Actions

        public void ButtonLaunch()
        {
            var result = _gameService.StartGame();

            if (result != LaunchGameResult.GameLaunched)
            {
                _eventAggregator.PublishOnUIThreadAsync(new ShowDialogMessage
                {
                    Title = Resources.Strings.S_MSG_LAUNCH_ERROR_TITLE,
                    Content = string.Concat(
                            result.HasFlag(LaunchGameResult.NoElevation) 
                            ? Resources.Strings.S_MSG_LAUNCH_ERROR_CONTENT_ELEVATION
                            : string.Empty, 
                            result.HasFlag(LaunchGameResult.NoSteam)
                            ? Resources.Strings.S_MSG_LAUNCH_ERROR_CONTENT_STEAM
                            : string.Empty
                        )
                });
                return;
            }

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
    }
}
