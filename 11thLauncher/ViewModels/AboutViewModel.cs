namespace _11thLauncher.ViewModels
{
    public class AboutViewModel
    {
        public string Author => string.Format(Resources.Strings.S_LABEL_APP_AUTHOR, Constants.Author);

        public string Build => string.Format(Resources.Strings.S_LABEL_BUILD, Constants.AssemblyVersion, 
            Constants.BuildCodeName, Constants.BuildDate.ToShortDateString());
    }
}
