using _11thLauncher.Model;
using _11thLauncher.Model.Parameter;
using Caliburn.Micro;

namespace _11thLauncher.ViewModels.Controls
{
    public class ParametersViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _eventAggregator;

        private BindableCollection<LaunchParameter> _parameters;
        public BindableCollection<LaunchParameter> Parameters
        {
            get { return _parameters; }
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

            Parameters = Constants.Parameters;
        }
    }
}
