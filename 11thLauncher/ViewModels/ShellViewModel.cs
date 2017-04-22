using System;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using _11thLauncher.Messages;
using _11thLauncher.Model;
using _11thLauncher.Model.Addon;
using _11thLauncher.Model.Game;
using _11thLauncher.Model.Parameter;
using _11thLauncher.Model.Server;
using _11thLauncher.Model.Settings;
using _11thLauncher.Properties;
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
        private readonly ParameterManager _parameterManager;
        private readonly LaunchManager _launchManager;

        private WindowState _windowState;
        private Visibility _showTrayIcon = Visibility.Hidden;
        private bool _showInTaskbar = true;
        private string _gameVersion;
        private Visibility _showVersionMismatch = Visibility.Hidden;
        private string _versionMismatchTooltip;

        public ShellViewModel(IEventAggregator eventAggregator, IDialogCoordinator dialogCoordinator, SettingsManager settingsManager, 
            AddonManager addonManager, ServerManager serverManager, ParameterManager parameterManager, LaunchManager launchManager)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _dialogCoordinator = dialogCoordinator;

            _settingsManager = settingsManager;
            _addonManager = addonManager;
            _serverManager = serverManager;
            _parameterManager = parameterManager;
            _launchManager = launchManager;

            Statusbar = new StatusbarViewModel(_eventAggregator);
            Addons = new AddonsViewModel(_eventAggregator);
            ProfileSelector = new ProfileSelectorViewModel(_eventAggregator, _addonManager);
            Parameters = new ParametersViewModel(_eventAggregator, _parameterManager);
            Game = new GameViewModel(_eventAggregator, _launchManager, _addonManager, _parameterManager, _settingsManager);

            Init();
        }

        #region Properties

        public ProfileSelectorViewModel ProfileSelector { get; set; }

        public AddonsViewModel Addons { get; set; }

        public ParametersViewModel Parameters { get; set; }

        public GameViewModel Game { get; set; }

        public StatusbarViewModel Statusbar { get; set; }

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

        public Visibility ShowTrayIcon
        {
            get { return _showTrayIcon; }
            set
            {
                _showTrayIcon = value;
                NotifyOfPropertyChange(() => ShowTrayIcon);
            }
        }

        public bool ShowInTaskbar
        {
            get { return _showInTaskbar; }
            set
            {
                _showInTaskbar = value;
                NotifyOfPropertyChange(() => ShowInTaskbar);
            }
        }

        public string GameVersion
        {
            get { return _gameVersion; }

            set
            {
                _gameVersion = value;
                NotifyOfPropertyChange(() => GameVersion);
            }
        }

        public Visibility ShowVersionMismatch
        {
            get { return _showVersionMismatch; }
            set
            {
                _showVersionMismatch = value;
                NotifyOfPropertyChange(() => ShowVersionMismatch);
            }
        }

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

        public void Init()
        {
            //Read config
            var userProfiles = _settingsManager.Initialize();

            //Set application theme
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Constants.Accents[_settingsManager.Accent]), ThemeManager.GetAppTheme("BaseLight"));

            //Read addons //TODO -> no path detected?
            var addons = _addonManager.ReadAddons(_settingsManager.Arma3Path);
            _eventAggregator.PublishOnCurrentThread(new AddonsMessage(AddonsAction.Added, addons));

            //Add profiles and load default
            _eventAggregator.PublishOnCurrentThread(new ProfilesMessage(ProfilesAction.Added, userProfiles));

            //Read memory allocators
            _parameterManager.ReadAllocators(_settingsManager.Arma3Path);

            CompareServerVersion(); //last thing
        }

        public void CompareServerVersion()
        {
            var gameVersion = _settingsManager.GetGameVersion();

            GameVersion = gameVersion;

            new Thread(_serverManager.GetServerVersion).Start();
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            _dialogCoordinator.ShowMessageAsync(this, Resources.S_MSG_GENERIC_DOMAIN_EXCEPTION, unhandledExceptionEventArgs.ExceptionObject.ToString());
        }

        #region Message handling

        public void Handle(ServerVersionMessage serverVersionMessage)
        {
            var serverVersion = serverVersionMessage.ServerVersion;
            if (string.IsNullOrEmpty(serverVersion) || string.IsNullOrEmpty(GameVersion)) return;

            var gameVersionInfo = GameVersion.Split('.');
            var serverVersionInfo = serverVersion.Split('.');
            if (gameVersionInfo.Length != 3 || serverVersionInfo.Length != 3) return;

            int gameBuild = Convert.ToInt32(gameVersionInfo[2]);
            int serverBuild = Convert.ToInt32(serverVersionInfo[2]);
            if (gameBuild >= serverBuild) return;

            ShowVersionMismatch = Visibility.Visible;
            VersionMismatchTooltip = string.Format(Resources.S_VERSION_MISMATCH, GameVersion, serverVersion);
        }

        public void Handle(ShowDialogMessage message)
        {
            _dialogCoordinator.ShowMessageAsync(this, message.Title, message.Content);
        }

        public void Handle(ExceptionMessage exceptionMessage)
        {
            _dialogCoordinator.ShowMessageAsync(this, Resources.S_MSG_GENERIC_EXCEPTION, exceptionMessage.Exception.ToString());
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
