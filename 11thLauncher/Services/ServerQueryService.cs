using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using QueryMaster;
using QueryMaster.GameServer;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;
using Server = _11thLauncher.Models.Server;
using ServerInfo = _11thLauncher.Models.ServerInfo;

namespace _11thLauncher.Services
{
    public class ServerQueryService : IServerQueryService
    {
        public void GetServerStatus(Server server)
        {
            server.ServerStatus = ServerStatus.Checking;

            try
            {
                QueryMaster.GameServer.Server gameServer = ServerQuery.GetServerInstance(EngineType.Source, GetServerIp(server.Address), server.QueryPort);
                QueryMaster.GameServer.ServerInfo info = gameServer.GetInfo();
                if (info != null)
                {
                    ServerInfo serverInfo = new ServerInfo
                    {
                        Name = info.Name,
                        Description = info.Description,
                        Ping = info.Ping,
                        Map = info.Map,
                        GameVersion = info.GameVersion,
                        Players = info.Players,
                        MaxPlayers = info.MaxPlayers,
                        PlayerList = new List<string>()
                    };

                    var players = gameServer.GetPlayers();
                    if (players != null)
                    {
                        foreach (PlayerInfo p in players)
                        {
                            serverInfo.PlayerList.Add(p.Name);
                        }
                    }

                    server.ServerStatus = ServerStatus.Online;
                    server.ServerInfo = serverInfo;
                }
                else
                {
                    server.ServerStatus = ServerStatus.Offline;
                }
                gameServer.Dispose();
            }
            catch (SocketException)
            {
                server.ServerStatus = ServerStatus.Offline;
                server.ServerInfo = null;
            }
        }

        public string GetServerVersion(Server server)
        {
            string remoteVersion = null;

            if (server.ServerInfo != null)
            {
                remoteVersion = server.ServerInfo.GameVersion;
            }
            else
            {
                GetServerStatus(server);
                if (server.ServerInfo?.GameVersion != null)
                {
                    remoteVersion = server.ServerInfo.GameVersion;
                }
            }

            return remoteVersion;
        }

        /// <summary>
        /// Resolve and return the IPv4 address the given address
        /// </summary>
        /// <param name="url">Address of the server</param>
        /// <returns>IPv4 address of the server</returns>
        private static string GetServerIp(string url)
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

            if (address == null)
            {
                throw new SocketException();
            }

            return address.ToString();
        }
    }
}