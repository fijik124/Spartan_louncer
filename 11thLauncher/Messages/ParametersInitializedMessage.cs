using Caliburn.Micro;
using _11thLauncher.Models;

namespace _11thLauncher.Messages
{
    public class ParametersInitializedMessage
    {
        public readonly BindableCollection<LaunchParameter> Parameters;

        public ParametersInitializedMessage(BindableCollection<LaunchParameter> parameters)
        {
            Parameters = parameters;
        }
    }
}
