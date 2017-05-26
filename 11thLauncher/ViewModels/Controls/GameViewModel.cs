using System;
using System.Windows.Controls;
using Caliburn.Micro;
using _11thLauncher.Game;
using _11thLauncher.Messages;
using _11thLauncher.Model;
using _11thLauncher.Model.Addons;
using _11thLauncher.Model.Game;
using _11thLauncher.Model.Parameter;
using _11thLauncher.Model.Settings;
using _11thLauncher.Parameter;
using _11thLauncher.Properties;

namespace _11thLauncher.ViewModels.Controls
{
    public class GameViewModel : PropertyChangedBase, IHandle<LoadProfileMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly LaunchManager _launchManager;
        private readonly AddonManager _addonManager;
        private readonly ParameterManager _parameterManager;
        private readonly SettingsManager _settingsManager;
        private LaunchOption _launchOption;
        private Platform _platform;
        private Priority _priority;
        private string _server;
        private string _port;

        public GameViewModel(IEventAggregator eventAggregator, LaunchManager launchManager, 
            AddonManager addonManager, ParameterManager parameterManager, SettingsManager settingsManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _launchManager = launchManager;
            _addonManager = addonManager;
            _parameterManager = parameterManager;
            _settingsManager = settingsManager;
        }

        #region Message handling

        public void Handle(LoadProfileMessage message)
        {
            //TODO copy gameconfig
        }

        #endregion

        public LaunchOption LaunchOption
        {
            get { return _launchOption; }
            set
            {
                _launchOption = value;
                NotifyOfPropertyChange();
            }
        }

        public Platform Platform
        {
            get { return _platform; }
            set
            {
                _platform = value;
                NotifyOfPropertyChange();
            }
        }

        public Priority Priority
        {
            get { return _priority; }
            set
            {
                _priority = value;
                NotifyOfPropertyChange();
            }
        }

        public string Server
        {
            get { return _server; }
            set
            {
                _server = value;
                NotifyOfPropertyChange();
            }
        }

        public string Port
        {
            get { return _port; }
            set
            {
                _port = value;
                NotifyOfPropertyChange();
            }
        }

        #region UI Actions

        public void ButtonLaunch(PasswordBox passwordBox)
        {
            var error = _launchManager.StartGame(_settingsManager.ApplicationSettings.Arma3Path, _addonManager.Addons, _parameterManager.Parameters,
                LaunchOption, Platform, Priority, 
                Server, Port, passwordBox.Password);

            switch (error)
            {
                case LaunchError.NoSteam:
                    _eventAggregator.PublishOnUIThread(new ShowDialogMessage
                    {
                        Title = Resources.S_MSG_NO_STEAM_TITLE,
                        Content = Resources.S_MSG_NO_STEAM_CONTENT
                    });
                    break;

                case LaunchError.NoGamePath:
                    _eventAggregator.PublishOnUIThread(new ShowDialogMessage
                    {
                        Title = Resources.S_MSG_PATH_TITLE,
                        Content = Resources.S_MSG_PATH_CONTENT
                    });
                    break;

                case LaunchError.NoElevation:
                    _eventAggregator.PublishOnUIThread(new ShowDialogMessage
                    {
                        Title = Resources.S_MSG_ELEVATION_TITLE,
                        Content = Resources.S_MSG_ELEVATION_CONTENT
                    });
                    break;

                case LaunchError.None:
                    break;

                default:
                    _eventAggregator.PublishOnUIThread(new ExceptionMessage(new ArgumentOutOfRangeException(nameof(error)), GetType().Name));
                    break;
            }
        }

        #endregion
    }
}
