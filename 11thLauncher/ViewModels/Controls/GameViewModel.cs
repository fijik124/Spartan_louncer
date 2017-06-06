using System.Windows.Controls;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Model;
using _11thLauncher.Model.Game;
using _11thLauncher.Model.Parameter;
using _11thLauncher.Model.Settings;
using _11thLauncher.Services;

namespace _11thLauncher.ViewModels.Controls
{
    public class GameViewModel : PropertyChangedBase, IHandle<LoadProfileMessage>, IHandle<FillServerInfoMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly LaunchManager _launchManager;
        private readonly IAddonService _addonService;
        private readonly ParameterManager _parameterManager;
        private readonly SettingsManager _settingsManager;
        private LaunchOption _launchOption;
        private LaunchPlatform _platform;
        private string _server;
        private string _port;

        public GameViewModel(IEventAggregator eventAggregator, LaunchManager launchManager, 
            IAddonService addonService, ParameterManager parameterManager, SettingsManager settingsManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _launchManager = launchManager;
            _addonService = addonService;
            _parameterManager = parameterManager;
            _settingsManager = settingsManager;
        }

        #region Message handling

        public void Handle(LoadProfileMessage message)
        {
            //TODO copy gameconfig
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
            get => _launchOption;
            set
            {
                _launchOption = value;
                NotifyOfPropertyChange();
            }
        }

        public LaunchPlatform Platform
        {
            get => _platform;
            set
            {
                _platform = value;
                NotifyOfPropertyChange();
            }
        }

        public string Server
        {
            get => _server;
            set
            {
                _server = value;
                NotifyOfPropertyChange();
            }
        }

        public string Port
        {
            get => _port;
            set
            {
                _port = value;
                NotifyOfPropertyChange();
            }
        }

        #region UI Actions

        public void ButtonLaunch(PasswordBox passwordBox)
        {
            _launchManager.StartGame(_addonService.GetAddons(), _parameterManager.Parameters,
                LaunchOption, Platform, Server, Port, passwordBox.Password);
        }

        #endregion
    }
}
