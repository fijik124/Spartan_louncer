using System.Diagnostics;
using System.IO;
using _11thLauncher.Accessors.Contracts;

namespace _11thLauncher.Accessors
{
    public class ProcessAccessor : IProcessAccessor
    {
        public Process StartProcess(string fileName)
        {
            return Process.Start(fileName);
        }

        public bool Start(Process process)
        {
            return process.Start();
        }

        public void WaitForExit(Process process)
        {
            process.WaitForExit();
        }

        public StreamReader GetStandardOutput(Process process)
        {
            return process.StandardOutput;
        }

        public StreamReader GetStandardError(Process process)
        {
            return process.StandardError;
        }

        public Process[] GetProcessesByName(string name)
        {
            return Process.GetProcessesByName(name);
        }
    }
}
