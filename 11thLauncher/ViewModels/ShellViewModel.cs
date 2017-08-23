using System;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using _11thLauncher.Accessors.Contracts;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;
using _11thLauncher.ViewModels.Controls;

namespace _11thLauncher.ViewModels
{
    public class ShellViewModel : PropertyChangedBase, IHandle<ShowDialogMessage>, IHandle<ExceptionMessage>, IHandle<ThemeChangedMessage>, IHandle<ServerQueryFinished>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IWindowManager _windowManager;
   
        private readonly ILogger _logger;
        private readonly ISettingsService _settingsService;
        private readonly IProfileService _profileService;
        private readonly IAddonService _addonService;
        private readonly IParameterService _parameterService;
        private readonly IAddonSyncService _addonSyncService;
        private readonly IUpdaterService _updaterService;
        private readonly IGameService _launcherService;

        private WindowState _windowState;
        private Visibility _showTrayIcon = Visibility.Hidden;
        private bool _showInTaskbar = true;
        private string _logoImage = Constants.LogoLight;
        private string _gameVersion;
        private Visibility _showVersionMismatch = Visibility.Hidden;
        private string _versionMismatchTooltip;

        public ShellViewModel(IEventAggregator eventAggregator, IDialogCoordinator dialogCoordinator, IWindowManager windowManager, ILogger logger,
            ISettingsService settingsService, IAddonService addonService, IAddonSyncService addonSyncService, IUpdaterService updaterService, 
            IParameterService parameterService, IGameService launcherService, IProfileService profileService)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _dialogCoordinator = DialogCoordinator.Instance;
            _windowManager = windowManager;

            _logger = logger;
            _settingsService = settingsService;
            _profileService = profileService;
            _addonService = addonService;
            _addonSyncService = addonSyncService;
            _updaterService = updaterService;
            _parameterService = parameterService;
            _launcherService = launcherService;

            StatusbarControl = IoC.Get<StatusbarViewModel>();
            ProfileSelectorControl = IoC.Get<ProfileSelectorViewModel>();
            AddonsControl = IoC.Get<AddonsViewModel>();
            GameControl = IoC.Get<GameViewModel>();
            ServerStatusControl = IoC.Get<ServerStatusViewModel>();
            RepositoryStatusControl = IoC.Get<RepositoryStatusViewModel>();
            ParametersControl = IoC.Get<ParametersViewModel>();
            ServerQueryControl = IoC.Get<ServerQueryViewModel>();
            ProfileManagerControl = IoC.Get<ProfileManagerViewModel>();

            Init();
        }

        #region Properties

        public StatusbarViewModel StatusbarControl { get; set; }

        public ProfileSelectorViewModel ProfileSelectorControl { get; set; }

        public AddonsViewModel AddonsControl { get; set; }

        public GameViewModel GameControl { get; set; }

        public ServerStatusViewModel ServerStatusControl { get; set; }

        public RepositoryStatusViewModel RepositoryStatusControl { get; set; }

        public ParametersViewModel ParametersControl { get; set; }

        public ServerQueryViewModel ServerQueryControl { get; set; }

        public ProfileManagerViewModel ProfileManagerControl { get; set; }

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
            _logger.LogDebug("ShellViewModel", "Starting Shell initialization");

            //If just updated, remove updater and show notification
            if (Program.Updated)
            {
                _updaterService.RemoveUpdater();
                _eventAggregator.PublishOnUIThreadAsync(new ShowDialogMessage
                {
                    Title = Resources.Strings.S_MSG_UPDATE_SUCCESS_TITLE,
                    Content = Resources.Strings.S_MSG_UPDATE_SUCCESS_CONTENT
                });
                _logger.LogInfo("ShellViewModel", "An update to the program was detected");
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
                _logger.LogInfo("ShellViewModel", "A failed update to the program was detected");
            }

            //TODO LEGACY CONVERT 
            var loadResult = _settingsService.Read();

            if (loadResult.Equals(LoadSettingsResult.LoadedLegacySettings))
            {
                //TODO trycatchhere?
                _profileService.PortLegacyProfiles(_settingsService.UserProfiles);
            }

            //If there were no existing settings or failed to load, start first time load process
            if (loadResult.Equals(LoadSettingsResult.NoExistingSettings) || loadResult.Equals(LoadSettingsResult.ErrorLoadingLegacySettings))
            {
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

                //Write default settings
                _settingsService.Write();

                //Write default profile
                _profileService.Write(defaultProfile, _addonService.Addons, _parameterService.Parameters, _launcherService.LaunchSettings);
            }

            _eventAggregator.PublishOnCurrentThread(new SettingsLoadedMessage());

            //Set application style
            var style = _settingsService.ApplicationSettings.ThemeStyle;
            var accent = _settingsService.ApplicationSettings.AccentColor;
            _eventAggregator.PublishOnCurrentThread(new ThemeChangedMessage(style, accent));

            //Initialize startup parameters and read memory allocators from game folder
            _parameterService.InitializeParameters(_settingsService.ApplicationSettings.Arma3Path);
            _eventAggregator.PublishOnCurrentThread(new ParametersInitializedMessage(_parameterService.Parameters));

            //Read addons
            _addonService.ReadAddons(_settingsService.ApplicationSettings.Arma3Path);
            _eventAggregator.PublishOnCurrentThread(new AddonsLoadedMessage());

            //Add profiles and load default
            _eventAggregator.PublishOnCurrentThread(new ProfileAddedMessage(_settingsService.UserProfiles));
            _eventAggregator.PublishOnCurrentThread(new LoadProfileMessage(_settingsService.DefaultProfile));

            //Read repository settings
            Task.Run(() =>
            {
                _settingsService.JavaVersion = _addonSyncService.GetJavaInSystem();

                if (!_addonSyncService.AddonSyncPathIsValid(_settingsService.ApplicationSettings.Arma3SyncPath))
                {
                    //If Arma3Sync path not valid try to get from registry
                    _settingsService.ApplicationSettings.Arma3SyncPath = _addonSyncService.GetAddonSyncPath();
                    _settingsService.Write();
                }

                var repositories = _addonSyncService.ReadRepositories(_settingsService.ApplicationSettings.Arma3SyncPath);
                _eventAggregator.PublishOnUIThreadAsync(new RepositoriesLoadedMessage(repositories));
            });


            if (_settingsService.ApplicationSettings.CheckUpdates)
            {
                Task.Run(() =>
                {
                    var updateCheckResult = _updaterService.CheckUpdates();
                    if (updateCheckResult.Equals(UpdateCheckResult.UpdateAvailable))
                    {
                        _eventAggregator.PublishOnUIThreadAsync(new ShowDialogMessage
                        {
                            Title = Resources.Strings.S_MSG_UPDATE_TITLE,
                            Content = Resources.Strings.S_MSG_UPDATE_CONTENT
                        });
                    }
                });
            }
            //Check local game version
            GameVersion = _settingsService.GetGameVersion();

            _logger.LogDebug("ShellViewModel", "Finished Shell initialization");
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
            _settingsService.UpdateThemeAndAccent(message.Theme, message.Accent);
            LogoImage = message.Theme == ThemeStyle.BaseLight ? Constants.LogoLight : Constants.LogoDark;
        }

        public void Handle(ServerQueryFinished message)
        {
            var gameVersion = _settingsService.GetGameVersion();
            var serverVersion = message.Server.ServerInfo?.GameVersion;
            if (string.IsNullOrEmpty(serverVersion) || string.IsNullOrEmpty(gameVersion)) return;

            var gameVersionInfo = gameVersion.Split('.');
            var serverVersionInfo = serverVersion.Split('.');
            if (gameVersionInfo.Length != 3 || serverVersionInfo.Length != 3) return;

            int gameBuild = Convert.ToInt32(gameVersionInfo[2]);
            int serverBuild = Convert.ToInt32(serverVersionInfo[2]);
            if (gameBuild == serverBuild)
            {
                ShowVersionMismatch = Visibility.Hidden;
                VersionMismatchTooltip = string.Empty;
            }
            else
            {
                ShowVersionMismatch = Visibility.Visible;
                VersionMismatchTooltip = string.Format(Resources.Strings.S_VERSION_MISMATCH, gameVersion, serverVersion);
            }
        }

        #endregion

        #region UI Actions

        public void ButtonSettings()
        {
            _windowManager.ShowDialog(IoC.Get<SettingsViewModel>());
        }

        public void ButtonAbout()
        {
            _windowManager.ShowDialog(IoC.Get<AboutViewModel>());
        }

        public void TrayIcon_Click()
        {
            WindowState = WindowState.Normal;
        }

        public void OnClose(ConsoleCancelEventArgs e)
        {
            _logger.LogDebug("ShellViewModel", "Close event detected. Terminating all processes");
            Environment.Exit(Environment.ExitCode); //Close background thread on shell closed
        }

        #endregion
    }
}