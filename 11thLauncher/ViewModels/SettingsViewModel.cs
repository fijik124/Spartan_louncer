using System.Windows;
using Caliburn.Micro;
using MahApps.Metro;
using _11thLauncher.Messages;
using _11thLauncher.Model;

namespace _11thLauncher.ViewModels
{
    public class SettingsViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _eventAggregator;

        private bool _checkUpdates = true;
        private bool _checkServers = true;
        private bool _checkRepository;
        private string _gamePath;
        private StartAction _startAction;

        private string _javaPath;
        private string _syncPath;
        //TODO repository

        private ThemeStyle _theme;
        private AccentColor _accent;
        private bool _minimizeToNotification;
        private bool _serversOpenAtStart;
        private bool _repositoryOpenAtStart;

        public SettingsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        public bool CheckUpdates
        {
            get => _checkUpdates;
            set
            {
                _checkUpdates = value;
                NotifyOfPropertyChange();
            }
        }

        public bool CheckServers
        {
            get => _checkServers;
            set
            {
                _checkServers = value;
                NotifyOfPropertyChange();
            }
        }

        public bool CheckRepository
        {
            get => _checkRepository;
            set
            {
                _checkRepository = value;
                NotifyOfPropertyChange();
            }
        }

        public string GamePath
        {
            get => _gamePath;
            set
            {
                _gamePath = value;
                NotifyOfPropertyChange();
            }
        }

        public StartAction StartAction
        {
            get => _startAction;
            set
            {
                _startAction = value;
                NotifyOfPropertyChange();
            }
        }

        public string JavaPath
        {
            get => _javaPath;
            set
            {
                _javaPath = value;
                NotifyOfPropertyChange();
            }
        }

        public string SyncPath
        {
            get => _syncPath;
            set
            {
                _syncPath = value;
                NotifyOfPropertyChange();
            }
        }

        public ThemeStyle SelectedTheme
        {
            get => _theme;
            set
            {
                _theme = value;
                NotifyOfPropertyChange();
            }
        }

        public AccentColor SelectedAccent
        {
            get => _accent;
            set
            {
                _accent = value;
                NotifyOfPropertyChange();
            }
        }

        public bool MinimizeToNotification
        {
            get => _minimizeToNotification;
            set
            {
                _minimizeToNotification = value;
                NotifyOfPropertyChange();
            }
        }

        public bool ServersOpenAtStart
        {
            get => _serversOpenAtStart;
            set
            {
                _serversOpenAtStart = value;
                NotifyOfPropertyChange();
                
            }
        }

        public bool RepositoryOpenAtStart
        {
            get => _repositoryOpenAtStart;
            set
            {
                _repositoryOpenAtStart = value;
                NotifyOfPropertyChange();
                
            }
        }

        #region UI Actions

        public void SelectGamePath()
        {

        }

        public void Theme_SelectionChanged()
        {
            //TODO move to settingsmanager?
            ThemeManager.ChangeAppStyle(Application.Current,
                ThemeManager.GetAccent(SelectedAccent.ToString()),
                ThemeManager.GetAppTheme(SelectedTheme.ToString()));

            _eventAggregator.PublishOnCurrentThread(new ThemeChangedMessage() { ThemeStyle = SelectedTheme, AccentColor = SelectedAccent });
        }

        public void Accent_SelectionChanged()
        {
            //TODO move to settingsmanager?
            ThemeManager.ChangeAppStyle(Application.Current,
                ThemeManager.GetAccent(SelectedAccent.ToString()),
                ThemeManager.GetAppTheme(SelectedTheme.ToString()));

            _eventAggregator.PublishOnCurrentThread(new ThemeChangedMessage(){ ThemeStyle = SelectedTheme, AccentColor = SelectedAccent });
        }

        #endregion
    }
}
