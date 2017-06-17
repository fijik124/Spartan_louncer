using System.Collections.Generic;

namespace _11thLauncher.Models
{
    public class ServerInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long Ping { get; set; }
        public string Map { get; set; }
        public string GameVersion { get; set; }
        public int Players { get; set; }
        public int MaxPlayers { get; set; }
        public string CurrentPlayers => Players + "/" + MaxPlayers;
        public List<string> PlayerList { get; set; }
    }
}
