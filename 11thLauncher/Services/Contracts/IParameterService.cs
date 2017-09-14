using Caliburn.Micro;
using _11thLauncher.Models;

namespace _11thLauncher.Services.Contracts
{
    public interface IParameterService
    {
        BindableCollection<LaunchParameter> Parameters { get; }

        /// <summary>
        /// Create the parameter variables and read memory allocators
        /// </summary>
        /// <param name="arma3Path">Game path</param>
        void InitializeParameters(string arma3Path);

        /// <summary>
        /// Read the memory allocators from the game ArmA 3 game folder
        /// </summary>
        /// <param name="arma3Path"></param>
        void ReadMemoryAllocators(string arma3Path);
    }
}
