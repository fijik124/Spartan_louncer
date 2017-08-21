using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Models;

namespace _11thLauncher.ViewModels.Controls
{
    public class ParametersViewModel : PropertyChangedBase, IHandle<ParametersInitializedMessage>, IHandle<ProfileLoadedMessage>
    {
        private readonly IEventAggregator _eventAggregator;

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

        public ParametersViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        #region Message handling

        public void Handle(ParametersInitializedMessage message)
        {
            Parameters = message.Parameters;
            foreach (LaunchParameter parameter in Parameters)
            {
                parameter.PropertyChanged += Parameter_StatusChanged;
            }
        }

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
