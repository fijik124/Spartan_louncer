using System.Threading;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Model;
using _11thLauncher.Model.Server;
using _11thLauncher.Model.Settings;

namespace _11thLauncher.ViewModels.Controls
{
    public class ServerStatusViewModel : PropertyChangedBase, IHandle<SettingsLoadedMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly SettingsManager _settingsManager;
        private readonly ServerManager _serverManager;
        private BindableCollection<Server> _servers;

        public ServerStatusViewModel(IEventAggregator eventAggregator, SettingsManager settingsManager, ServerManager serverManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _settingsManager = settingsManager;
            _serverManager = serverManager;

            Servers = new BindableCollection<Server>();
        }

        #region Message handling

        public void Handle(SettingsLoadedMessage message)
        {
            Servers = _settingsManager.Servers;
            //TODO DEBUG
            //foreach (Server server in Servers)
            //{
                //new Thread(() => ServerManager.CheckServerStatus(server)).Start();
            //}
        }

        #endregion

        #region UI Actions

        public void CheckServerStatus(Server server)
        {
            if (server.ServerStatus != ServerStatus.Checking)
            {
                new Thread(() => _serverManager.CheckServerStatus(server)).Start();
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
