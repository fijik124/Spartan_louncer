using System;
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
                    QueryMaster.GameServer.Server server = ServerQuery.GetServerInstance(EngineType.Source, GetServerIp(Constants.ServerUrl).ToString(), serverPort);

                    QueryMaster.GameServer.ServerInfo config = server.GetInfo();
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
        private static IPAddress GetServerIp(string url)
        {
            IPAddress address = null;
            IPHostEntry ipHostInfo = Dns.GetHostEntry(url);

            //Find IPv4 address
            foreach (IPAddress addr in ipHostInfo.AddressList)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    address = addr;
                }
            }

            return address;
        }

        public static void CheckServerStatus(Server server)
        {
            //TODO statusbar start

            server.ServerStatus = ServerStatus.Unknown;
            string players = "-/-"; //TODO

            try
            {
                QueryMaster.GameServer.Server gameServer = ServerQuery.GetServerInstance(EngineType.Source, GetServerIp(server.Address).ToString(), server.Port);
                QueryMaster.GameServer.ServerInfo info = gameServer.GetInfo();
                if (info != null)
                {
                    ServerInfo serverInfo = new ServerInfo()
                    {
                       Players = info.Players,
                       MaxPlayers = info.MaxPlayers
                    };

                    //ServerInfo info = server.GetInfo();
                    //IReadOnlyCollection<PlayerInfo> players = server.GetPlayers();
                    //server.Dispose();

                    //ServerInfo.Add(info.Name);
                    //ServerInfo.Add(info.Description);
                    //ServerInfo.Add(info.Ping.ToString());
                    //ServerInfo.Add(info.Map);
                    //ServerInfo.Add(info.Players.ToString());
                    //ServerInfo.Add(info.MaxPlayers.ToString());
                    //ServerInfo.Add(info.GameVersion);

                    //foreach (PlayerInfo p in players)
                    //{
                        //ServerPlayers.Add(p.Name);
                    //}


                    server.ServerStatus = ServerStatus.Online;
                    server.ServerInfo = serverInfo;
                }
                gameServer.Dispose();
            }
            catch (SocketException)
            {
                server.ServerStatus = ServerStatus.Offline;
            }

            //TODO update statusbar
        }
    }
}
