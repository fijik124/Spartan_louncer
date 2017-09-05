using System;

namespace _11thLauncher.Util
{
    public interface ILogger
    {
        void LogDebug(string component, string message);
        void LogInfo(string component, string message);
        void LogException(string component, string message, Exception e = null);
    }
}
