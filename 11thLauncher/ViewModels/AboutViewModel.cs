using System;
using System.Reflection;
using System.Threading.Tasks;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.ViewModels
{
    public class AboutViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IUpdaterService _updaterService;

        private readonly string _assemblyVersion;

        public AboutViewModel(IEventAggregator eventAggregator, IUpdaterService updaterService)
        {
            _eventAggregator = eventAggregator;
            _updaterService = updaterService;

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            _assemblyVersion = string.Join(",", version.Major, version.Minor, version.Build);
        }

        public string Author => string.Format(Resources.Strings.S_LABEL_APP_AUTHOR, Constants.Author);

        public string Build => string.Format(Resources.Strings.S_LABEL_BUILD, _assemblyVersion, 
            Constants.BuildCodeName, Constants.BuildDate.ToShortDateString());

        #region UI Actions

        public void ButtonUpdate()
        {
            Task.Run(() =>
            {
                var updateCheckResult = _updaterService.CheckUpdates();
                if (updateCheckResult.Equals(UpdateCheckResult.UpdateAvailable))
                {
                    _eventAggregator.PublishOnUIThreadAsync(new ShowDialogMessage
                    {
                        Title = Resources.Strings.S_MSG_UPDATE_TITLE,
                        Content = Resources.Strings.S_MSG_UPDATE_CONTENT
                    });
                }
                else if (updateCheckResult.Equals(UpdateCheckResult.NoUpdateAvailable))
                {
                    //TODO show no updates message
                }
                else
                {
                    //TODO show error checking updates message
                }
            });
        }

        #endregion
    }
}
