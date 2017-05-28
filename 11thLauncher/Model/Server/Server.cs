using Newtonsoft.Json;

namespace _11thLauncher.Model.Server
{
    public class Server
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public ushort Port { get; set; }

        /// <summary>
        /// Indicates if this server is an application default.
        /// </summary>
        public bool IsDefault { get; set; }
        [JsonIgnore]
        public ServerStatus ServerStatus { get; set; }
    }
}
