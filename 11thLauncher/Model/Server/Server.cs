using Caliburn.Micro;
using Newtonsoft.Json;

namespace _11thLauncher.Model.Server
{
    public class Server : PropertyChangedBase
    {
        private string _name;
        private string _address;
        private ushort _port;
        private bool _isDefault;
        private ServerStatus _serverStatus = ServerStatus.Unknown;
        private ServerInfo _serverInfo;

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
        [JsonProperty]
        public bool IsDefault
        {
            get => _isDefault;
            set
            {
                _isDefault = value;
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
        
        public override bool Equals(object obj)
        {
            var item = obj as Server;

            return item != null && Name.Equals(item.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
