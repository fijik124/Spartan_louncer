using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using QueryMaster;
using QueryMaster.GameServer;
using _11thLauncher.Configuration;
using _11thLauncher.Model;

namespace _11thLauncher.Net
{
    static class Servers
    {
        public static List<string> ServerInfo;
        public static List<string> ServerPlayers;

        public static object Address { get; private set; }




        /// <summary>
        /// Check if the servers are online and call the form to update stattus
        /// </summary>
        public static void CheckServers()
        {
            for (int i = 0; i < Constants.ServerPorts.Length; i++)
            {
                bool status = false;
                string players = "-/-";

                MainWindow.UpdateForm("UpdateStatusBar", new object[] { "Comprobando servidor " + (i + 1) });

                try
                {
                    QueryMaster.GameServer.Server server = ServerQuery.GetServerInstance(EngineType.Source, GetServerIp().ToString(), Constants.ServerPorts[i]);
                    ServerInfo info = server.GetInfo();
                    if (info != null)
                    {
                        players = info.Players + "/" + info.MaxPlayers;
                        status = true;
                    }
                    server.Dispose();
                }
                catch (SocketException)
                {
                    status = false;
                }

                MainWindow.UpdateForm("UpdateServerStatus", new object[] { i, status, players });
            }
        }

        /// <summary>
        /// Check if the server with the given index is online and call the form to update stattus
        /// </summary>
        /// <param name="indexobj">Server index</param>
        public static void CheckServer(object indexobj)
        {
            int index = (int)indexobj;

            MainWindow.UpdateForm("UpdateStatusBar", new object[] { "Comprobando servidor " + (index + 1) });

            bool status = false;
            string players = "-/-";

            try
            {
                QueryMaster.GameServer.Server server = ServerQuery.GetServerInstance(EngineType.Source, GetServerIp().ToString(), Constants.ServerPorts[index]);
                ServerInfo info = server.GetInfo();
                if (info != null)
                {
                    players = info.Players + "/" + info.MaxPlayers;
                    status = true;
                }
                server.Dispose();
            }
            catch (SocketException)
            {
                status = false;
            }

            MainWindow.UpdateForm("UpdateServerStatus", new object[] { index, status, players });
        }

        /// <summary>
        /// Query the server with the given index and callback the form to show its information
        /// </summary>
        /// <param name="indexobj">Server index</param>
        public static void QueryServerInfo(object indexobj)
        {
            int index = (int)indexobj;

            ServerInfo = new List<string>();
            ServerPlayers = new List<string>();

            MainWindow.UpdateForm("UpdateStatusBar", new object[] { "Solicitando información del servidor " + (index + 1) });

            bool exception = false;

            try
            {
                QueryMaster.GameServer.Server server = ServerQuery.GetServerInstance(EngineType.Source, GetServerIp().ToString(), Constants.ServerPorts[index]);


                ServerInfo info = server.GetInfo();
                IReadOnlyCollection<PlayerInfo> players = server.GetPlayers();
                server.Dispose();

                ServerInfo.Add(info.Name);
                ServerInfo.Add(info.Description);
                ServerInfo.Add(info.Ping.ToString());
                ServerInfo.Add(info.Map);
                ServerInfo.Add(info.Players.ToString());
                ServerInfo.Add(info.MaxPlayers.ToString());
                ServerInfo.Add(info.GameVersion);

                foreach (PlayerInfo p in players)
                {
                    ServerPlayers.Add(p.Name);
                }
            }
            catch (SocketException)
            {
                ServerInfo = new List<string> { "-", "-", "-", "-", "-", "-", "-" };
                ServerPlayers = new List<string>();
                exception = true;
            }

            MainWindow.UpdateForm("UpdateServerInfo", new object[] { index, exception });
        }
        


        /// <summary>
        /// Resolve and return the IPv4 address of 11thmeu.es
        /// </summary>
        /// <returns>IPv4 address of the server</returns>
        private static IPAddress GetServerIp()
        {
            IPAddress address = null;
            //if (Address == null)
            //{
                //IPHostEntry ipHostInfo = Dns.GetHostEntry("www.11thmeu.es");

                ////Find IPv4 address
                //foreach (IPAddress addr in ipHostInfo.AddressList)
                //{
                    //if (addr.AddressFamily == AddressFamily.InterNetwork)
                    //{
                        //address = addr;
                    //}
                //}
            //} else
            //{
                ////Address was resolved previously, return it directly
                //address = Address;
            //}

            return address;
        }
    }
}
