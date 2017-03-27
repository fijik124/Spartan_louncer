using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;

namespace _11thLauncher.Processes
{
    public static class Util
    {
        private static readonly bool RunningAsAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        
    }
}
