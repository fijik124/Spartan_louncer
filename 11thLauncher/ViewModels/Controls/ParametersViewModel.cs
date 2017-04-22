using _11thLauncher.Model.Parameter;
using Caliburn.Micro;

namespace _11thLauncher.ViewModels.Controls
{
    public class ParametersViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ParameterManager _parameterManager;

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

        public ParametersViewModel(IEventAggregator eventAggregator, ParameterManager parameterManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _parameterManager = parameterManager;

            Parameters = _parameterManager.Parameters;
        }
    }
}
