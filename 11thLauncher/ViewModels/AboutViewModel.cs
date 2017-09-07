using System;
using System.Reflection;

namespace _11thLauncher.ViewModels
{
    public class AboutViewModel
    {
        private readonly string _assemblyVersion;

        public AboutViewModel()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            _assemblyVersion = string.Join(".", version.Major, version.Minor, version.Build);
        }

        #region Properties

        public string Author => string.Format(Resources.Strings.S_LABEL_APP_AUTHOR, ApplicationConfig.Author);

        public string Build => string.Format(Resources.Strings.S_LABEL_BUILD, _assemblyVersion,
            ApplicationConfig.BuildCodeName, ApplicationConfig.BuildDate.ToShortDateString());

        #endregion
    }
}