using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.ViewModels.Controls
{
    public class ParametersViewModel : PropertyChangedBase, IHandle<ProfileLoadedMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IParameterService _parameterService;

        private BindableCollection<LaunchParameter> _parameters;
        public BindableCollection<LaunchParameter> Parameters
        {
            get => _parameters;
            set
            {
                _parameters = value;
                NotifyOfPropertyChange();
            }
        }

        public ParametersViewModel(IEventAggregator eventAggregator, IParameterService parameterService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _parameterService = parameterService;

            Parameters = _parameterService.Parameters;
            foreach (LaunchParameter parameter in Parameters)
            {
                parameter.PropertyChanged += Parameter_StatusChanged;
            }
        }

        #region Message handling

        public void Handle(ProfileLoadedMessage message)
        {
            //Reset all parameters before loading
            foreach (LaunchParameter parameter in Parameters)
            {
                parameter.SetStatus(false);
            }

            foreach (var parameter in Parameters)
            {
                var profileParameter = message.Parameters.FirstOrDefault(parameter.Equals);
                parameter.CopyStatus(profileParameter);
            }

            CollectionViewSource.GetDefaultView(Parameters).Refresh();
        }

        #endregion

        #region UI Actions

        private void Parameter_StatusChanged(object sender, PropertyChangedEventArgs e)
        {
            _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
        }

        #endregion
    }
}
