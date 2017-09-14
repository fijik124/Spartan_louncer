using Caliburn.Micro;
using Newtonsoft.Json;

namespace _11thLauncher.Models
{
    public class Server : PropertyChangedBase
    {
        #region Fields

        private string _name;
        private string _address;
        private ushort _port;
        private bool _isDefault;
        private bool _isEnabled;
        private ServerStatus _serverStatus = ServerStatus.Unknown;
        private ServerInfo _serverInfo;

        #endregion

        #region Properties

        [JsonProperty]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyOfPropertyChange();
            }
        }

        [JsonProperty]
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                NotifyOfPropertyChange();
            }
        }

        [JsonProperty]
        public ushort Port
        {
            get => _port;
            set
            {
                _port = value;
                NotifyOfPropertyChange();
            }
        }

        [JsonIgnore]
        public ushort QueryPort => (ushort)(_port + 1);

        /// <summary>
        /// Indicates if this server is an application default.
        /// </summary>
        [JsonIgnore]
        public bool IsDefault
        {
            get => _isDefault;
            set
            {
                _isDefault = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Indicates if this server is enabled to be shown on the UI.
        /// </summary>
        [JsonProperty]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange();
            }
        }

        [JsonIgnore]
        public ServerStatus ServerStatus
        {
            get => _serverStatus;
            set
            {
                _serverStatus = value;
                NotifyOfPropertyChange();
            }
        }

        public ServerInfo ServerInfo
        {
            get => _serverInfo;
            set
            {
                _serverInfo = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            var item = obj as Server;

            return item != null && Name.Equals(item.Name) && Address.Equals(item.Address) && Port.Equals(item.Port);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() * Address.GetHashCode() * Port.GetHashCode();
        }

        #endregion
    }
}
