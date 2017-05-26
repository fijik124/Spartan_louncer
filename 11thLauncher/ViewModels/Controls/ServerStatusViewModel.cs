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
        private BindableCollection<Model.Server.Server> _servers;

        public ServerStatusViewModel(IEventAggregator eventAggregator, SettingsManager settingsManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _settingsManager = settingsManager;

            Servers = new BindableCollection<Model.Server.Server>();
        }

        #region Message handling

        public void Handle(SettingsLoadedMessage message)
        {
            Servers = _settingsManager.Servers;
        }

        #endregion

        public BindableCollection<Model.Server.Server> Servers
        {
            get { return _servers; }
            set
            {
                _servers = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
