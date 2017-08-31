using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.ViewModels.Controls
{
    public class RepositoryStatusViewModel : PropertyChangedBase, IHandle<RepositoriesLoadedMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IAddonSyncService _addonSyncService;
        private readonly ISettingsService _settingsService;

        private string _arma3SyncIcon = ApplicationConfig.Arma3SyncIconDisabled;
        private BindableCollection<Repository> _repositories;

        public RepositoryStatusViewModel(IEventAggregator eventAggregator, IAddonSyncService addonSyncService, ISettingsService settingsService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _addonSyncService = addonSyncService;
            _settingsService = settingsService;

            _repositories = new BindableCollection<Repository>();
        }

        public string Arma3SyncIcon
        {
            get => _arma3SyncIcon;
            set
            {
                _arma3SyncIcon = value;
                NotifyOfPropertyChange();
            }
        }

        public BindableCollection<Repository> Repositories
        {
            get => _repositories;
            set
            {
                _repositories = value;
                NotifyOfPropertyChange();
            }
        }

        public bool Arma3SyncConfigured => !string.IsNullOrEmpty(_settingsService.ApplicationSettings.Arma3SyncPath);

        public void Handle(RepositoriesLoadedMessage message)
        {
            if (string.IsNullOrEmpty(_settingsService.ApplicationSettings.Arma3SyncPath)) return;

            Arma3SyncIcon = ApplicationConfig.Arma3SyncIconEnabled;
            Repositories = message.Repositories;

            if (!_settingsService.ApplicationSettings.CheckRepository) return; //Exit if startup check is disabled

            Task.Run(() =>
            {
                foreach (var repository in Repositories)
                {
                    CheckRepository(repository);
                }
            });
        }

        public void StartArma3Sync()
        {
            if (string.IsNullOrEmpty(_settingsService.ApplicationSettings.Arma3SyncPath)) return;

            _addonSyncService.StartAddonSync(_settingsService.ApplicationSettings.Arma3SyncPath);
        }

        public void CheckRepositoryStatus(Repository repository)
        {
            if (Repositories.All(r => r.Status != RepositoryStatus.Checking))
            {
                Task.Run(() =>
                {
                    CheckRepository(repository);
                });
            }
        }

        private void CheckRepository(Repository repository)
        {
            _eventAggregator.PublishOnUIThread(new UpdateStatusBarMessage(AsyncAction.CheckRepositoryStatus, true));
            _addonSyncService.CheckRepository(_settingsService.ApplicationSettings.Arma3SyncPath, _settingsService.ApplicationSettings.JavaPath, repository);
            _eventAggregator.PublishOnUIThread(new UpdateStatusBarMessage(AsyncAction.CheckRepositoryStatus, false));
        }
    }
}
