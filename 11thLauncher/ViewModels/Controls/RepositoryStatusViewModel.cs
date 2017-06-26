using Caliburn.Micro;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.ViewModels.Controls
{
    public class RepositoryStatusViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IAddonSyncService _addonSyncService;
        private BindableCollection<Repository> _repositories;

        public RepositoryStatusViewModel(IEventAggregator eventAggregator, IAddonSyncService addonSyncService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _addonSyncService = addonSyncService;

            Repositories = new BindableCollection<Repository>();
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
    }
}
