using _11thLauncher.Model.Process;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;

namespace _11thLauncher.Model.Shell
{
    public class ProcessManager
    {
        private readonly bool _runningAsAdmin;

        public ProcessManager()
        {
            _runningAsAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        public ProcessInfo GetSteamProcess()
        {
            ProcessInfo process = new ProcessInfo();

            var steamProcess = System.Diagnostics.Process.GetProcessesByName("Steam").FirstOrDefault();
            if (steamProcess != null)
            {
                process.Name = steamProcess.ProcessName;
                process.Elevated = IsElevated(steamProcess);
                process.Running = true;
            }

            return process;
        }

        public ProcessInfo GetTeamspeakProcess()
        {
            ProcessInfo process = new ProcessInfo();

            var ts3Process = System.Diagnostics.Process.GetProcessesByName("ts3client_win32").FirstOrDefault();
            if (ts3Process == null)
            {
                ts3Process = System.Diagnostics.Process.GetProcessesByName("ts3client_win64").FirstOrDefault();
                if (ts3Process != null)
                {
                    process.Is64Bit = true;
                }
            }

            if (ts3Process != null)
            {
                process.Name = ts3Process.ProcessName;
                process.Elevated = IsElevated(ts3Process);
                process.Running = true;
            }

            return process;
        }

        #region Private methods

        private bool IsElevated(System.Diagnostics.Process process)
        {
            bool elevated = false;
            if (_runningAsAdmin)
            {
                elevated = UacHelper.IsProcessElevated(process);
            }
            else
            {
                try
                {
                    var dmy = process.HasExited;
                }
                catch (Win32Exception)
                {
                    elevated = true;
                }
            }

            return elevated;
        }

        #endregion
    }
}
