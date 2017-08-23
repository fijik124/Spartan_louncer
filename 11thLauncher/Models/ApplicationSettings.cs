namespace _11thLauncher.Models
{
    public class ApplicationSettings
    {
        public bool CheckUpdates = true;
        public bool CheckServers = true;
        public bool CheckRepository = false;
        public string Language = "en-US";
        public string Arma3Path = null;
        public StartAction StartAction = StartAction.Nothing;
        public bool MinimizeNotification = false;
        public ThemeStyle ThemeStyle = 0;
        public AccentColor AccentColor = 0;
        public string JavaPath = "";
        public string Arma3SyncPath = "";
    }
}
