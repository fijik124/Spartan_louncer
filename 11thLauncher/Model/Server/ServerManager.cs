using System.Net;
using System.Net.Sockets;
using Caliburn.Micro;
using QueryMaster;
using QueryMaster.GameServer;
using _11thLauncher.Messages;

namespace _11thLauncher.Model.Server
{
    public class ServerManager
    {
        private readonly IEventAggregator _eventAggregator;
        private static readonly IPAddress Address = null;

        public ServerManager(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        /// <summary>
        /// Compare the local game version with the server version and callback the form to show if it doesn't match
        /// </summary>
        public void GetServerVersion()
        {
            foreach (ushort serverPort in Constants.ServerPorts) //servers not in constatns?
            {
                try
                {
                    QueryMaster.GameServer.Server server = ServerQuery.GetServerInstance(EngineType.Source, GetServerIp().ToString(), serverPort);

                    ServerInfo config = server.GetInfo();
                    server.Dispose();

                    if (config != null)
                    {
                        var remoteVersion = config.GameVersion; //TODO handle null
                        _eventAggregator.PublishOnUIThread(new ServerVersionMessage { ServerVersion = remoteVersion });
                    }

                }
                catch (SocketException) { }
            }
        }

        /// <summary>
        /// Resolve and return the IPv4 address of 11thmeu.es
        /// </summary>
        /// <returns>IPv4 address of the server</returns>
        private static IPAddress GetServerIp()
        {
            IPAddress address = null;
            if (Address == null)
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Constants.ServerUrl);

                //Find IPv4 address
                foreach (IPAddress addr in ipHostInfo.AddressList)
                {
                    if (addr.AddressFamily == AddressFamily.InterNetwork)
                    {
                        address = addr;
                    }
                }
            }
            else
            {
                //Address was resolved previously, return it directly
                address = Address;
            }

            return address;
        }
    }
}
