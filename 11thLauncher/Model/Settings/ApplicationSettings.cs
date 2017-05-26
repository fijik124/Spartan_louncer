namespace _11thLauncher.Model.Settings
{
    public class ApplicationSettings
    {
        public string Arma3Path = null;
        public bool MinimizeNotification = false;
        public int Accent = 0;

        //TODO process and move up
        public string JavaPath = "";
        public string Arma3SyncPath = "";
        public string Arma3SyncRepository = "";
        public bool StartClose = false;
        public bool StartMinimize = false;
        public bool CheckUpdates = true;
        public bool CheckServers = true;
        public bool CheckRepository = false;
        public bool ServersGroupBox = true;
        public bool RepositoryGroupBox = true;
    }
}
