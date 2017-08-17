using System.Threading.Tasks;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.ViewModels.Controls
{
    public class ServerStatusViewModel : PropertyChangedBase, IHandle<SettingsLoadedMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISettingsService _settingsService;
        private readonly IServerQueryService _serverQueryService;
        private BindableCollection<Server> _servers;

        public ServerStatusViewModel(IEventAggregator eventAggregator, ISettingsService settingsService, IServerQueryService serverQueryService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _settingsService = settingsService;
            _serverQueryService = serverQueryService;

            Servers = new BindableCollection<Server>();
        }

        #region Message handling

        public void Handle(SettingsLoadedMessage message)
        {
            Servers = _settingsService.Servers;

            if (!_settingsService.ApplicationSettings.CheckServers) return; //Exit if startup check is disabled

            foreach (Server server in Servers)
            {
                CheckServerStatus(server);
            }
        }

        #endregion

        #region UI Actions

        public void CheckServerStatus(Server server)
        {
            if (server.ServerStatus != ServerStatus.Checking)
            {
                Task.Run(() =>
                {
                    _eventAggregator.PublishOnUIThread(new UpdateStatusBarMessage(AsyncAction.CheckServerStatus, true));
                    _serverQueryService.GetServerStatus(server);
                    _eventAggregator.PublishOnUIThread(new UpdateStatusBarMessage(AsyncAction.CheckServerStatus, false));
                });
            }
        }

        public void FillServerInfo(Server server)
        {
            _eventAggregator.PublishOnCurrentThread(new FillServerInfoMessage(server));
        }

        #endregion

        public BindableCollection<Server> Servers
        {
            get => _servers;
            set
            {
                _servers = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
