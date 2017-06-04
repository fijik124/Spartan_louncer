namespace _11thLauncher.Model.Server
{
    public class ServerInfo
    {
        public int Players;
        public int MaxPlayers;

        public string CurrentPlayers => Players + "/" + MaxPlayers;
    }
}
