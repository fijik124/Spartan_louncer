namespace _11thLauncher.Models
{
    public class ApplicationSettings
    {
        public string Language = "en-US";
        public string Arma3Path = null;
        public bool MinimizeNotification = false;
        public ThemeStyle ThemeStyle = 0;
        public AccentColor AccentColor = 0;
        public string JavaPath = "";
        public string Arma3SyncPath = "";

        //TODO process and move up
        public string Arma3SyncRepository = "";
        public bool StartClose = false;
        public bool StartMinimize = false;
    }
}
