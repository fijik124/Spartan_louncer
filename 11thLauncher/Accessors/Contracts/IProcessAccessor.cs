using System.Diagnostics;
using System.IO;

namespace _11thLauncher.Accessors.Contracts
{
    public interface IProcessAccessor
    {
        Process StartProcess(string fileName);

        bool Start(Process process);

        void WaitForExit(Process process);

        StreamReader GetStandardOutput(Process process);

        StreamReader GetStandardError(Process process);

        Process[] GetProcessesByName(string name);
    }
}
