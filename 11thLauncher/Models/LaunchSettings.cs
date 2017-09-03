namespace _11thLauncher.Models
{
    public class LaunchSettings
    {
        #region Properties

        public LaunchOption LaunchOption = LaunchOption.Normal; 
        public LaunchPlatform Platform = LaunchPlatform.X86;
        public string Server = string.Empty;
        public string Port = string.Empty;
        public string Password = string.Empty;

        #endregion
    }
}