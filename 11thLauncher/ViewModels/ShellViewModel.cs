using System.Threading;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using _11thLauncher.Messages;
using _11thLauncher.Model;
using _11thLauncher.Model.Addon;
using _11thLauncher.Model.Server;
using _11thLauncher.Model.Settings;
using _11thLauncher.ViewModels.Controls;

namespace _11thLauncher.ViewModels
{
    public class ShellViewModel : PropertyChangedBase, IHandle<ServerVersionMessage>, IHandle<ShowDialogMessage>, IHandle<ExceptionMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly SettingsManager _settingsManager;
        private readonly AddonManager _addonManager;
        private readonly ServerManager _serverManager;

        public StatusbarViewModel Statusbar { get; set; }
        public AddonsViewModel Addons { get; set; }
        public ProfileSelectorViewModel ProfileSelector { get; set; }
        public ParametersViewModel Parameters { get; set; }

        #region Properties

        private WindowState _windowState;
        public WindowState WindowState
        {
            get { return _windowState; }
            set
            {
                _windowState = value;
                NotifyOfPropertyChange(() => WindowState);

                if (!_settingsManager.MinimizeNotification) return;

                if (value == WindowState.Minimized)
                {
                    ShowInTaskbar = false;
                    ShowTrayIcon = Visibility.Visible;
                }
                else
                {
                    ShowInTaskbar = true;
                    ShowTrayIcon = Visibility.Hidden;
                }
            }
        }

        private Visibility _showTrayIcon = Visibility.Hidden;
        public Visibility ShowTrayIcon
        {
            get { return _showTrayIcon; }
            set
            {
                _showTrayIcon = value;
                NotifyOfPropertyChange(() => ShowTrayIcon);
            }
        }

        private bool _showInTaskbar = true;
        public bool ShowInTaskbar
        {
            get { return _showInTaskbar; }
            set
            {
                _showInTaskbar = value;
                NotifyOfPropertyChange(() => ShowInTaskbar);
            }
        }

        private string _gameVersion;
        public string GameVersion
        {
            get { return _gameVersion; }

            set
            {
                _gameVersion = value;
                NotifyOfPropertyChange(() => GameVersion);
            }
        }

        private Visibility _showVersionMismatch = Visibility.Hidden;
        public Visibility ShowVersionMismatch
        {
            get { return _showVersionMismatch; }
            set
            {
                _showVersionMismatch = value;
                NotifyOfPropertyChange(() => ShowVersionMismatch);
            }
        }

        private string _versionMismatchTooltip;
        public string VersionMismatchTooltip
        {
            get { return _versionMismatchTooltip; }
            set
            {
                _versionMismatchTooltip = value;
                NotifyOfPropertyChange(() => VersionMismatchTooltip);
            }
        }

        #endregion

        public ShellViewModel(IEventAggregator eventAggregator, IDialogCoordinator dialogCoordinator, SettingsManager settingsManager, 
            AddonManager addonManager, ServerManager serverManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _dialogCoordinator = dialogCoordinator;

            _settingsManager = settingsManager;
            _addonManager = addonManager;
            _serverManager = serverManager;

            Statusbar = new StatusbarViewModel(eventAggregator);
            Addons = new AddonsViewModel(eventAggregator);
            ProfileSelector = new ProfileSelectorViewModel(eventAggregator, addonManager);
            Parameters = new ParametersViewModel(eventAggregator);

            Init();
        }

        public void Init()
        {
            //Read config
            var userProfiles = _settingsManager.Initialize();

            //Set application theme
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Constants.Accents[_settingsManager.Accent]), ThemeManager.GetAppTheme("BaseLight"));

            //Read addons
            var addons = _addonManager.ReadAddons(_settingsManager.Arma3Path);
            _eventAggregator.PublishOnCurrentThread(new AddonsMessage(AddonsAction.Added, addons));

            //Add profiles and load default
            _eventAggregator.PublishOnCurrentThread(new ProfilesMessage(ProfilesAction.Added, userProfiles));

            //TODO memory allocators

            CompareServerVersion(); //last thing
        }

        public void CompareServerVersion()
        {
            var gameVersion = _settingsManager.GetGameVersion();

            GameVersion = gameVersion;

            new Thread(_serverManager.GetServerVersion).Start();
        }

        #region Message handling

        public void Handle(ServerVersionMessage serverVersionMessage)
        {
            var serverVersion = serverVersionMessage.ServerVersion;

            if (string.IsNullOrEmpty(serverVersion)) return;

            if (!GameVersion.Equals(serverVersion))
            {
                ShowVersionMismatch = Visibility.Visible;
                VersionMismatchTooltip = string.Format(Properties.Resources.S_VERSION_MISMATCH, serverVersion);
            }
        }

        public void Handle(ShowDialogMessage message)
        {
            _dialogCoordinator.ShowMessageAsync(this, message.Title, message.Content);
        }

        public void Handle(ExceptionMessage exceptionMessage)
        {
            //TODO show exception message
        }

        #endregion

        #region UI Actions

        public void TrayIcon_Click()
        {
            WindowState = WindowState.Normal;
        }

        #endregion
    }
}
