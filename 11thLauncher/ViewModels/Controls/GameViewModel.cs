using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.ViewModels.Controls
{
    public class GameViewModel : PropertyChangedBase, IHandle<ProfileLoadedMessage>, IHandle<FillServerInfoMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ILauncherService _launcherService;
        private readonly IAddonService _addonService;
        private readonly ParameterManager _parameterManager;
        private readonly ISecurityService _securityService;

        private bool _loadingProfile;

        public GameViewModel(IEventAggregator eventAggregator, ILauncherService launcherService, 
            IAddonService addonService, ParameterManager parameterManager, ISecurityService securityService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _launcherService = launcherService;
            _addonService = addonService;
            _parameterManager = parameterManager;
            _securityService = securityService;
        }

        #region Message handling

        public void Handle(ProfileLoadedMessage message)
        {
            _loadingProfile = true;

            _launcherService.LaunchSettings.LaunchOption = message.LaunchSettings.LaunchOption;
            NotifyOfPropertyChange(() => LaunchOption);

            _launcherService.LaunchSettings.Platform = message.LaunchSettings.Platform;
            NotifyOfPropertyChange(() => Platform);

            _launcherService.LaunchSettings.Server = message.LaunchSettings.Server;
            NotifyOfPropertyChange(() => Server);

            _launcherService.LaunchSettings.Port = message.LaunchSettings.Port;
            NotifyOfPropertyChange(() => Port);

            _launcherService.LaunchSettings.Password = message.LaunchSettings.Password;
            NotifyOfPropertyChange(() => Password);
        }

        public void Handle(FillServerInfoMessage message)
        {
            if (message.Server == null) return;
            Server = message.Server.Address;
            Port = message.Server.Port.ToString();
        }

        #endregion

        public LaunchOption LaunchOption
        {
            get => _launcherService.LaunchSettings.LaunchOption;
            set
            {
                _launcherService.LaunchSettings.LaunchOption = value;
                _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
                NotifyOfPropertyChange();
            }
        }

        public LaunchPlatform Platform
        {
            get => _launcherService.LaunchSettings.Platform;
            set
            {
                _launcherService.LaunchSettings.Platform = value;
                _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
                NotifyOfPropertyChange();
            }
        }

        public string Server
        {
            get => _launcherService.LaunchSettings.Server;
            set
            {
                _launcherService.LaunchSettings.Server = value;
                _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
                NotifyOfPropertyChange();
            }
        }

        public string Port
        {
            get => _launcherService.LaunchSettings.Port;
            set
            {
                _launcherService.LaunchSettings.Port = value;
                _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
                NotifyOfPropertyChange();
            }
        }

        public string Password
        {
            get => _securityService.DecryptPassword(_launcherService.LaunchSettings.Password);
            set
            {
                _launcherService.LaunchSettings.Password = _securityService.EncryptPassword(value);
                NotifyOfPropertyChange();
                if (_loadingProfile) { _loadingProfile = false; return; } //Avoid profile write when loading profile
                _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
            }
        }

        #region UI Actions

        public void ButtonLaunch()
        {
            _launcherService.StartGame(_addonService.GetAddons(), _parameterManager.Parameters,
                LaunchOption, Platform, Server, Port, Password);
        }

        #endregion
    }
}
