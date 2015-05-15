using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using QueryMaster;

namespace _11thLauncher
{
    class Net
    {
        public static bool[] serversStatus = new bool[3] { false, false, false };
        public static List<String> serverInfo;
        public static List<String> serverPlayers;
        public static String serverMods;
        public static Boolean queryException;

        private static ushort[] servers = new ushort[3] { 2303, 2323, 2333 }; //Query port = server port + 1

        //Check if there is a server online in the given port
        public static void checkServers()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry("www.11thmeu.es");
            IPAddress address = null;

            //Find IPv4 address
            foreach (IPAddress addr in ipHostInfo.AddressList)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    address = addr;
                }
            }

            for (int i = 0; i < servers.Length; i++)
			{
                try
                {
                    Server server = ServerQuery.GetServerInstance(EngineType.Source, address.ToString(), servers[i]);
                    ServerInfo info = server.GetInfo();
                    serversStatus[i] = true;
                }
                catch (SocketException) 
                {
                    serversStatus[i] = false;
                }
                Program.form.updateStatus(i);
			}
        }

        public static void queryServerInfo(object input)
        {
            int index = (int)input;
            IPHostEntry ipHostInfo = Dns.GetHostEntry("www.11thmeu.es");
            IPAddress address = null;

            //Find IPv4 address
            foreach (IPAddress addr in ipHostInfo.AddressList)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    address = addr;
                }
            }

            Server server = ServerQuery.GetServerInstance(EngineType.Source, address.ToString(), servers[index]);

            serverInfo = new List<String>();
            serverPlayers = new List<String>();
            serverMods = "";

            try
            {
                ServerInfo info = server.GetInfo();
                IReadOnlyCollection<Player>  players = server.GetPlayers();
                IReadOnlyCollection<Rule> rules = server.GetRules();

                serverInfo.Add(info.Name);
                serverInfo.Add(info.Description);
                serverInfo.Add(info.Ping.ToString());
                serverInfo.Add(info.Map);
                serverInfo.Add(info.Players.ToString());
                serverInfo.Add(info.MaxPlayers.ToString());
                serverInfo.Add(info.GameVersion);

                foreach (Player p in players)
                {
                    serverPlayers.Add(p.Name);
                }

                String mods = "";
                foreach (Rule r in rules)
                {
                    mods += r.Value;
                }

                List<String> split = mods.Split(';').ToList<String>();
                split.RemoveAt(split.Count - 1);
                bool skip = false;
                mods = "";
                foreach (string s in split)
                {
                    if (skip)
                    {
                        skip = false;
                        continue;
                    }
                    else
                    {
                        mods += s + "; ";
                        skip = true;
                    }
                }
                serverMods = mods;

                Program.form.showInfo(index);
            }
            catch (SocketException) 
            {
                serverInfo = new List<String> { "-", "-", "-", "-", "-", "-", "-" };
                serverPlayers = new List<String>();
                serverMods = "";
                queryException = true;
                Program.form.showInfo(index);
                MetroFramework.MetroMessageBox.Show(Program.form, "Error de comunicación con el servidor seleccionado", "Tiempo de espera agotado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
