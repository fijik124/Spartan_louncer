using System;
using System.Linq;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using _11thLauncher.Messages;
using _11thLauncher.Model;
using _11thLauncher.Model.Profile;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;
using _11thLauncher.ViewModels.Controls;

namespace _11thLauncher.ViewModels
{
    public class ShellViewModel : PropertyChangedBase, IHandle<ShowDialogMessage>, IHandle<ExceptionMessage>, IHandle<ThemeChangedMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IWindowManager _windowManager;
        private readonly ParameterManager _parameterManager;
        private readonly ProfileManager _profileManager;

        private readonly ISettingsService _settingsService;
        private readonly IAddonService _addonService;
        private readonly IServerQueryService _serverQueryService;
        private readonly IUpdaterService _updaterService;
        private readonly IGameLauncherService _gameLauncherService;

        private WindowState _windowState;
        private Visibility _showTrayIcon = Visibility.Hidden;
        private bool _showInTaskbar = true;
        private string _logoImage = Constants.LogoLight;
        private string _gameVersion;
        private Visibility _showVersionMismatch = Visibility.Hidden;
        private string _versionMismatchTooltip;

        public ShellViewModel(IEventAggregator eventAggregator, IDialogCoordinator dialogCoordinator, IWindowManager windowManager,
            ISettingsService settingsService, IAddonService addonService, IServerQueryService serverQueryService, IUpdaterService updaterService ,
            ParameterManager parameterManager, IGameLauncherService gameLauncherService, ProfileManager profileManager)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _dialogCoordinator = DialogCoordinator.Instance;
            _windowManager = windowManager;

            _settingsService = settingsService;
            _addonService = addonService;
            _serverQueryService = serverQueryService;
            _updaterService = updaterService;
            _parameterManager = parameterManager;
            _gameLauncherService = gameLauncherService;
            _profileManager = profileManager;

            Statusbar = IoC.Get<StatusbarViewModel>();
            ProfileSelector = IoC.Get<ProfileSelectorViewModel>();
            Addons = IoC.Get<AddonsViewModel>();
            Game = IoC.Get<GameViewModel>();
            ServerStatus = IoC.Get<ServerStatusViewModel>();
            Parameters = IoC.Get<ParametersViewModel>();
            ServerQuery = IoC.Get<ServerQueryViewModel>();
            ProfileManager = IoC.Get<ProfileManagerViewModel>();

            //Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("es-ES"); TODO this
            Init();
        }

        #region Properties

        public StatusbarViewModel Statusbar { get; set; }

        public ProfileSelectorViewModel ProfileSelector { get; set; }

        public AddonsViewModel Addons { get; set; }

        public GameViewModel Game { get; set; }

        public ServerStatusViewModel ServerStatus { get; set; }

        public ParametersViewModel Parameters { get; set; }

        public ServerQueryViewModel ServerQuery { get; set; }

        public ProfileManagerViewModel ProfileManager { get; set; }

        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                _windowState = value;
                NotifyOfPropertyChange(() => WindowState);

                if (!_settingsService.ApplicationSettings.MinimizeNotification) return;

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
            get => _showTrayIcon;
            set
            {
                _showTrayIcon = value;
                NotifyOfPropertyChange(() => ShowTrayIcon);
            }
        }

        public bool ShowInTaskbar
        {
            get => _showInTaskbar;
            set
            {
                _showInTaskbar = value;
                NotifyOfPropertyChange(() => ShowInTaskbar);
            }
        }

        public string GameVersion
        {
            get => _gameVersion;

            set
            {
                _gameVersion = value;
                NotifyOfPropertyChange(() => GameVersion);
            }
        }

        public Visibility ShowVersionMismatch
        {
            get => _showVersionMismatch;
            set
            {
                _showVersionMismatch = value;
                NotifyOfPropertyChange(() => ShowVersionMismatch);
            }
        }

        public string VersionMismatchTooltip
        {
            get => _versionMismatchTooltip;
            set
            {
                _versionMismatchTooltip = value;
                NotifyOfPropertyChange(() => VersionMismatchTooltip);
            }
        }

        public string LogoImage
        {
            get => _logoImage;
            set
            {
                _logoImage = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        private void Init()
        {
            //If just updated, remove updater and show notification
            if (Program.Updated)
            {
                _updaterService.RemoveUpdater();
                _eventAggregator.PublishOnUIThreadAsync(new ShowDialogMessage
                {
                    Title = Resources.Strings.S_MSG_UPDATE_SUCCESS_TITLE,
                    Content = Resources.Strings.S_MSG_UPDATE_SUCCESS_CONTENT
                });
            }

            //Notify if update failed
            if (Program.UpdateFailed)
            {
                _updaterService.RemoveUpdater();
                _eventAggregator.PublishOnUIThreadAsync(new ShowDialogMessage
                {
                    Title = Resources.Strings.S_MSG_UPDATE_FAIL_TITLE,
                    Content = Resources.Strings.S_MSG_UPDATE_FAIL_CONTENT
                });
            }

            //TODO LEGACY CONVERT 
            if (_settingsService.SettingsExist())
            {
                _settingsService.Read(true); //Read existing settings
            }
            else
            {
                _settingsService.Read(false); //Load default settings in memory

                _settingsService.ReadPath();
                if (string.IsNullOrWhiteSpace(_settingsService.ApplicationSettings.Arma3Path))
                {
                    _eventAggregator.PublishOnUIThreadAsync(new ShowDialogMessage
                    {
                        Title = Resources.Strings.S_MSG_PATH_TITLE,
                        Content = Resources.Strings.S_MSG_PATH_CONTENT
                    });
                }

                //Create default profile
                UserProfile defaultProfile = new UserProfile(Resources.Strings.S_DEFAULT_PROFILE_NAME, true);
                _settingsService.UserProfiles.Add(defaultProfile);
                _settingsService.DefaultProfileId = defaultProfile.Id;

                //Save default settings
                _settingsService.Write();

                //Save default profile
                //_profileManager.WriteProfile(defaultProfile, _addonService.GetAddons(), _parameterManager.Parameters, _launchManager.GameConfig); TODO
            }
            _eventAggregator.PublishOnCurrentThread(new SettingsLoadedMessage());

            //Read addons //TODO -> no path detected?
            var addons = _addonService.ReadAddons(_settingsService.ApplicationSettings.Arma3Path);
            _eventAggregator.PublishOnCurrentThread(new AddonsLoaded(addons));

            //Add profiles and load default
            _eventAggregator.PublishOnCurrentThread(new ProfilesMessage(ProfilesAction.Added, _settingsService.UserProfiles));

            //Read memory allocators TODO -> possible problems with parameters read before?
            _parameterManager.ReadAllocators(_settingsService.ApplicationSettings.Arma3Path);

            //Check local game version against remote server
            CompareServerVersion();

            //TODO - check updates
            //TODO - check repository if configured
        }

        private void CompareServerVersion()
        {
            var gameVersion = _settingsService.GetGameVersion();
            GameVersion = gameVersion;
            new Thread(() =>
            {
                var serverVersion = _serverQueryService.GetServerVersion(_settingsService.Servers.First()); //TODO no servers?
                if (string.IsNullOrEmpty(serverVersion) || string.IsNullOrEmpty(GameVersion)) return;

                var gameVersionInfo = GameVersion.Split('.');
                var serverVersionInfo = serverVersion.Split('.');
                if (gameVersionInfo.Length != 3 || serverVersionInfo.Length != 3) return;

                int gameBuild = Convert.ToInt32(gameVersionInfo[2]);
                int serverBuild = Convert.ToInt32(serverVersionInfo[2]);
                if (gameBuild >= serverBuild) return;

                ShowVersionMismatch = Visibility.Visible;
                VersionMismatchTooltip = string.Format(Resources.Strings.S_VERSION_MISMATCH, GameVersion, serverVersion);
            }).Start();
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            _dialogCoordinator.ShowMessageAsync(this, Resources.Strings.S_MSG_GENERIC_DOMAIN_EXCEPTION,
                unhandledExceptionEventArgs.ExceptionObject.ToString());
        }

        #region Message handling

        public void Handle(ShowDialogMessage message)
        {
            _dialogCoordinator.ShowMessageAsync(this, message.Title, message.Content);
        }

        public void Handle(ExceptionMessage exceptionMessage)
        {
            _dialogCoordinator.ShowMessageAsync(this, Resources.Strings.S_MSG_GENERIC_EXCEPTION,
                exceptionMessage.Exception.ToString());
        }

        public void Handle(ThemeChangedMessage message)
        {
            LogoImage = message.ThemeStyle == ThemeStyle.BaseLight ? Constants.LogoLight : Constants.LogoDark;
        }

        #endregion

        #region UI Actions

        public void ButtonSettings()
        {
            _windowManager.ShowDialog(new SettingsViewModel(_eventAggregator));
        }

        public void ButtonAbout()
        {
            _windowManager.ShowDialog(new AboutViewModel());
        }

        public void TrayIcon_Click()
        {
            WindowState = WindowState.Normal;
        }

        public void OnClose(ConsoleCancelEventArgs e)
        {
            Environment.Exit(Environment.ExitCode); //Close background thread on shell closed
        }

        #endregion
    }
}