using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.ViewModels
{
    public class SettingsViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogCoordinator _dialogCoordinator;

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
        private bool _minimizeNotification;

        private readonly ISettingsService _settingsService;

        private string _selectedLanguage;

        private bool _restarting;

        public SettingsViewModel(IEventAggregator eventAggregator, IDialogCoordinator dialogCoordinator, ISettingsService settingsService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _dialogCoordinator = dialogCoordinator;
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

        private void Load()
        {
            //General
            GamePath = _settingsService.ApplicationSettings.Arma3Path;

            //Interface
            SelectedLanguage = Languages.FirstOrDefault(x => x.Equals(_settingsService.ApplicationSettings.Language)) ?? Constants.Languages.First();
            SelectedTheme = _settingsService.ApplicationSettings.ThemeStyle;
            SelectedAccent = _settingsService.ApplicationSettings.AccentColor;
            MinimizeNotification = _settingsService.ApplicationSettings.MinimizeNotification;
        }

        private void Save()
        {
            //General
            _settingsService.ApplicationSettings.Arma3Path = GamePath;

            //Interface
            _settingsService.ApplicationSettings.Language = SelectedLanguage;
            _settingsService.ApplicationSettings.ThemeStyle = SelectedTheme;
            _settingsService.ApplicationSettings.AccentColor = SelectedAccent;
            _settingsService.ApplicationSettings.MinimizeNotification = MinimizeNotification;

            _settingsService.Write();
        }

        #region UI Actions

        public void SelectGamePath()
        {

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
                Resources.Strings.S_MSG_RESET_CONTENT, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
                {
                    AffirmativeButtonText = Resources.Strings.S_LABEL_OK,
                    NegativeButtonText = Resources.Strings.S_LABEL_CANCEL
                });

            if (result != MessageDialogResult.Affirmative) return;

            _settingsService.Delete();
            _restarting = true;
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
        }

        public void OnClose(ConsoleCancelEventArgs e)
        {
            if (_restarting) return;
            Save();
        }

        #endregion
    }
}
