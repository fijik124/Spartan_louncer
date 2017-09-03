﻿using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using QueryMaster;
using QueryMaster.GameServer;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;
using _11thLauncher.Util;
using Server = _11thLauncher.Models.Server;
using ServerInfo = _11thLauncher.Models.ServerInfo;

namespace _11thLauncher.Services
{
    public class ServerQueryService : AbstractService, IServerQueryService
    {
        public ServerQueryService(ILogger logger) : base(logger) { }

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
                    Logger.LogDebug("ServerQueryService", $"Server '{server.Name}' queried succesfully");
                }
                else
                {
                    server.ServerStatus = ServerStatus.Offline;
                    server.ServerInfo = null;
                    Logger.LogDebug("ServerQueryService", $"Unable to query server '{server.Name}'");
                }
                gameServer.Dispose();
            }
            catch (SocketException e)
            {
                server.ServerStatus = ServerStatus.Offline;
                server.ServerInfo = null;
                Logger.LogException("ServerQueryService", "Error checking server status", e);
            }
        }

        /// <summary>
        /// Resolve and return the IPv4 address the given address
        /// </summary>
        /// <param name="url">Address of the server</param>
        /// <returns>IPv4 address of the server</returns>
        private string GetServerIp(string url)
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

            Logger.LogDebug("ServerQueryService", $"Resolved IP address of {address}");
            return address.ToString();
        }
    }
}