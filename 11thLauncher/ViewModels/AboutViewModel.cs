using System.Reflection;
using _11thLauncher.Properties;

namespace _11thLauncher.ViewModels
{
    public class AboutViewModel
    {
        public string Author => string.Format(Resources.S_LABEL_APP_AUTHOR, Constants.Author);

        public string Build => string.Format(Resources.S_LABEL_BUILD, Assembly.GetExecutingAssembly().GetName().Version,
            Constants.BuildCodeName, Constants.BuildDate.ToShortDateString());
    }
}
