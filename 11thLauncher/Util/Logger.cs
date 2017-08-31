using System;
using System.IO;
using System.Linq;
using System.Text;
using _11thLauncher.Models;

namespace _11thLauncher.Util
{
    public class Logger : ILogger
    {
        private const string LogPattern = "log{0}.log";
        private readonly LogLevel _maxLogLevel;
        private readonly string _logDirectory;
        private readonly int _maxRolledFiles;
        private readonly long _maxSize;
        private readonly string _logFile;

        public Logger()
        {
            _maxLogLevel = Constants.MaxLogLevel;
            _logDirectory = Constants.ConfigPath;
            _maxRolledFiles = Constants.LogRolledFiles;
            _maxSize = Constants.LogSizeLimit;
            _logFile = Path.Combine(_logDirectory, string.Format(LogPattern, ""));
        }

        public void LogDebug(string component, string message)
        {
            if (_maxLogLevel.Equals(LogLevel.DEBUG))
                WriteMessageToLog(LogLevel.DEBUG, component, message);
        }

        public void LogInfo(string component, string message)
        {
            if (_maxLogLevel >= LogLevel.INFO)
                WriteMessageToLog(LogLevel.INFO, component, message);
        }

        public void LogException(string component, string message, Exception e)
        {
            if (_maxLogLevel >= LogLevel.ERROR)
                WriteExceptionToLog(component, message, e);
        }

        private void WriteMessageToLog(LogLevel logLevel, string component, string message)
        {
            string logLevelStr = $"[{logLevel}]".PadRight(7);
            string content = string.Format("{1} - {2} {3} - {4}{0}",
                Environment.NewLine,
                DateTime.Now,
                logLevelStr,
                component,
                message);
            WriteToLog(content);
        }

        private void WriteExceptionToLog(string component, string message, Exception e)
        {
            string logLevelStr = $"[{LogLevel.ERROR}]".PadRight(7);
            string content = string.Format("{1} - {2} {3} - {4} {0}{5}{0}",
                Environment.NewLine,
                DateTime.Now,
                logLevelStr,
                component,
                message,
                e);
            WriteToLog(content);
        }

        private void RollLogFile()
        {
            if (!File.Exists(_logFile)) return;

            var size = new FileInfo(_logFile).Length;
            if (size <= _maxSize) return;

            var logFiles = Directory.GetFiles(_logDirectory, string.Format(LogPattern, "?"), SearchOption.TopDirectoryOnly).ToList();
            if (logFiles.Count <= 0) return;

            logFiles.Sort();
            if (logFiles.Count >= _maxRolledFiles)
            {
                File.Delete(logFiles[logFiles.Count - 1]);
                logFiles.RemoveAt(logFiles.Count - 1);
            }

            for (int i = logFiles.Count; i > 0; i--)
                File.Move(logFiles[i - 1], Path.Combine(_logDirectory, string.Format(LogPattern, i)));
        }

        private void WriteToLog(string content)
        {
#if DEBUG
            Console.Write(content);
#endif
            try
            {
                if (!Directory.Exists(Constants.ConfigPath)) return;
                lock (_logFile)
                {
                    RollLogFile();
                    File.AppendAllText(_logFile, content, Encoding.UTF8);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
