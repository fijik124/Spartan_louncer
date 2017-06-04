using System.Threading;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Model.Server;
using _11thLauncher.Model.Settings;

namespace _11thLauncher.ViewModels.Controls
{
    public class ServerStatusViewModel : PropertyChangedBase, IHandle<SettingsLoadedMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly SettingsManager _settingsManager;
        private BindableCollection<Server> _servers;

        public ServerStatusViewModel(IEventAggregator eventAggregator, SettingsManager settingsManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _settingsManager = settingsManager;

            Servers = new BindableCollection<Server>();
        }

        #region Message handling

        public void Handle(SettingsLoadedMessage message)
        {
            Servers = _settingsManager.Servers;
            //TODO DEBUG
            foreach (Server server in Servers)
            {
                new Thread(() => ServerManager.CheckServerStatus(server)).Start();
            }
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
