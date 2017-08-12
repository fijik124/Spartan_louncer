using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using _11thLauncher.Accessors.Contracts;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.ViewModels
{
    public class SettingsViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IFileAccessor _fileAccessor;
        private readonly ISettingsService _settingsService;

        private bool _checkUpdates = true;
        private bool _checkServers = true;
        private bool _checkRepository;
        private string _gamePath;
        private StartAction _startAction;

        private string _javaVersion;
        private string _javaPath;
        private string _syncPath;
        //TODO repository

        private ThemeStyle _theme;
        private AccentColor _accent;
        private bool _minimizeNotification;

        private string _selectedLanguage;

        private bool _restarting;

        public SettingsViewModel(IEventAggregator eventAggregator, IDialogCoordinator dialogCoordinator, IFileAccessor fileAccessor, ISettingsService settingsService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _dialogCoordinator = dialogCoordinator;
            _fileAccessor = fileAccessor;
            _settingsService = settingsService;

            Load();
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

        public string JavaVersion
        {
            get => _javaVersion;
            set
            {
                _javaVersion = value;
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

        public bool JavaDetected => !string.IsNullOrEmpty(JavaVersion);

        public string SyncPath
        {
            get => _syncPath;
            set
            {
                _syncPath = value;
                NotifyOfPropertyChange();
            }
        }

        public IEnumerable<string> Languages => Constants.Languages;

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
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

        public bool MinimizeNotification
        {
            get => _minimizeNotification;
            set
            {
                _minimizeNotification = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Load the application settings from the settings service
        /// </summary>
        private void Load()
        {
            //Startup
            CheckUpdates = _settingsService.ApplicationSettings.CheckUpdates;
            CheckServers = _settingsService.ApplicationSettings.CheckServers;
            CheckRepository = _settingsService.ApplicationSettings.CheckRepository;

            //Game
            GamePath = _settingsService.ApplicationSettings.Arma3Path;

            //Repository
            JavaVersion = _settingsService.JavaVersion;
            JavaPath = _settingsService.ApplicationSettings.JavaPath;
            SyncPath = _settingsService.ApplicationSettings.Arma3SyncPath;

            //Interface
            SelectedLanguage = Languages.FirstOrDefault(x => x.Equals(_settingsService.ApplicationSettings.Language)) ?? Constants.Languages.First();
            SelectedTheme = _settingsService.ApplicationSettings.ThemeStyle;
            SelectedAccent = _settingsService.ApplicationSettings.AccentColor;
            MinimizeNotification = _settingsService.ApplicationSettings.MinimizeNotification;
        }

        /// <summary>
        /// Save the application settings to the settings service and write to disk
        /// </summary>
        private void Save()
        {
            //Startup
            _settingsService.ApplicationSettings.CheckUpdates = CheckUpdates;
            _settingsService.ApplicationSettings.CheckServers = CheckServers;
            _settingsService.ApplicationSettings.CheckRepository = CheckRepository;

            //Game
            _settingsService.ApplicationSettings.Arma3Path = GamePath;

            //Repository
            _settingsService.ApplicationSettings.JavaPath = JavaPath;
            _settingsService.ApplicationSettings.Arma3SyncPath = SyncPath;

            //Interface
            _settingsService.ApplicationSettings.Language = SelectedLanguage;
            _settingsService.ApplicationSettings.ThemeStyle = SelectedTheme;
            _settingsService.ApplicationSettings.AccentColor = SelectedAccent;
            _settingsService.ApplicationSettings.MinimizeNotification = MinimizeNotification;

            _settingsService.Write();
        }

        /// <summary>
        /// Shut down the launcher and start it again.
        /// </summary>
        private void RestartApplication()
        {
            _restarting = true;
            Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }

        #region UI Actions

        public async void SelectGamePath()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = Resources.Strings.S_BROWSE_GAME_FOLDER;
                if (!string.IsNullOrEmpty(GamePath) && _fileAccessor.DirectoryExists(GamePath))
                {
                    dialog.SelectedPath = GamePath;
                }

                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.OK) return;

                string selectedPath = dialog.SelectedPath;
                if (string.IsNullOrEmpty(selectedPath) || !_fileAccessor.DirectoryExists(selectedPath)) return;

                //Check if selected folder contains game executable
                if (!_fileAccessor.FileExists(Path.Combine(selectedPath, Constants.GameExecutable32)))
                {
                    await _dialogCoordinator.ShowMessageAsync(this, Resources.Strings.S_MSG_INCORRECT_PATH_TITLE,
                        Resources.Strings.S_MSG_INCORRECT_PATH_CONTENT, MessageDialogStyle.Affirmative, new MetroDialogSettings
                        {
                            AffirmativeButtonText = Resources.Strings.S_LABEL_OK
                        });
                    return;
                }
                
                GamePath = selectedPath;
            }
        }

        public async void SelectJavaPath()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = Resources.Strings.S_BROWSE_JAVA_FOLDER;
                if (!string.IsNullOrEmpty(JavaPath) && _fileAccessor.DirectoryExists(JavaPath))
                {
                    dialog.SelectedPath = JavaPath;
                }

                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.OK) return;

                string selectedPath = dialog.SelectedPath;
                if (string.IsNullOrEmpty(selectedPath) || !_fileAccessor.DirectoryExists(selectedPath)) return;

                //Check if selected folder contains java binary folder or executable
                if (_fileAccessor.FileExists(Path.Combine(selectedPath, Constants.JavaExecutable)))
                {
                    JavaPath = selectedPath;
                }
                else if (_fileAccessor.FileExists(Path.Combine(selectedPath, Constants.JavaRuntimeBinaryFolder, Constants.JavaExecutable)))
                {
                    JavaPath = Path.Combine(selectedPath, Constants.JavaRuntimeBinaryFolder);
                }
                else
                {
                    await _dialogCoordinator.ShowMessageAsync(this, Resources.Strings.S_MSG_INCORRECT_JAVA_PATH_TITLE,
                        Resources.Strings.S_MSG_INCORRECT_JAVA_PATH_CONTENT, MessageDialogStyle.Affirmative, new MetroDialogSettings
                        {
                            AffirmativeButtonText = Resources.Strings.S_LABEL_OK
                        });
                }
            }
        }
        public async void SelectArma3SyncPath()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = Resources.Strings.S_BROWSE_ARMA3SYNC_FOLDER;
                if (!string.IsNullOrEmpty(SyncPath) && _fileAccessor.DirectoryExists(SyncPath))
                {
                    dialog.SelectedPath = SyncPath;
                }

                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.OK) return;

                string selectedPath = dialog.SelectedPath;
                if (string.IsNullOrEmpty(selectedPath) || !_fileAccessor.DirectoryExists(selectedPath)) return;

                //Check if selected folder contains arma3sync executable
                if (!_fileAccessor.FileExists(Path.Combine(selectedPath, Constants.Arma3SyncExecutable)))
                {
                    await _dialogCoordinator.ShowMessageAsync(this, Resources.Strings.S_MSG_INCORRECT_ARMA3SYNC_PATH_TITLE,
                        Resources.Strings.S_MSG_INCORRECT_ARMA3SYNC_PATH_CONTENT, MessageDialogStyle.Affirmative, new MetroDialogSettings
                        {
                            AffirmativeButtonText = Resources.Strings.S_LABEL_OK
                        });
                    return;
                }

                SyncPath = selectedPath;
            }
        }

        public async void Language_SelectionChanged()
        {
            MessageDialogResult result = await _dialogCoordinator.ShowMessageAsync(this, Resources.Strings.S_MSG_CHANGE_LANGUAGE_TITLE,
                Resources.Strings.S_MSG_CHANGE_LANGUAGE_CONTENT, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
                {
                    AffirmativeButtonText = Resources.Strings.S_LABEL_YES,
                    NegativeButtonText = Resources.Strings.S_LABEL_NO
                });

            if (result != MessageDialogResult.Affirmative) return;

            _settingsService.ApplicationSettings.Language = SelectedLanguage;
            _settingsService.Write();
            RestartApplication();
        }

        public void Theme_SelectionChanged()
        {
            _eventAggregator.PublishOnCurrentThread(new ThemeChangedMessage(SelectedTheme, SelectedAccent));
        }

        public void Accent_SelectionChanged()
        {
            _eventAggregator.PublishOnCurrentThread(new ThemeChangedMessage(SelectedTheme, SelectedAccent));
        }

        public async void DeleteSettings()
        {
            MessageDialogResult result = await _dialogCoordinator.ShowMessageAsync(this, Resources.Strings.S_MSG_RESET_TITLE, 
                Resources.Strings.S_MSG_RESET_CONTENT, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
                {
                    AffirmativeButtonText = Resources.Strings.S_LABEL_OK,
                    NegativeButtonText = Resources.Strings.S_LABEL_CANCEL
                });

            if (result != MessageDialogResult.Affirmative) return;

            _settingsService.Delete();
            RestartApplication();
        }

        public void OnClose(ConsoleCancelEventArgs e)
        {
            if (_restarting) return;
            Save();
        }

        #endregion
    }
}
