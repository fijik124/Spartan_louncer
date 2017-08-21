using Caliburn.Micro;
using _11thLauncher.Models;

namespace _11thLauncher.Services.Contracts
{
    public interface IParameterService
    {
        BindableCollection<LaunchParameter> Parameters { get; }

        /// <summary>
        /// Create the parameter variables and read the memory allocators available in the ArmA 3 Dll folder
        /// </summary>
        /// <param name="arma3Path">Game path</param>
        void InitializeParameters(string arma3Path);
    }
}
