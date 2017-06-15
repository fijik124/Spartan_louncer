using System.Threading;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Model;
using _11thLauncher.Model.Server;
using _11thLauncher.Model.Settings;
using _11thLauncher.Services;

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
                new Thread(() =>
                {
                    _eventAggregator.PublishOnUIThread(new UpdateStatusBarMessage(AsyncAction.CheckServerStatus, true));
                    _serverQueryService.CheckServerStatus(server);
                    _eventAggregator.PublishOnUIThread(new UpdateStatusBarMessage(AsyncAction.CheckServerStatus, false));
                }).Start();
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
