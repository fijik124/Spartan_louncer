using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using _11thLauncher.Models;

namespace _11thLauncher.Util
{
    public class Logger : ILogger
    {
        private const string LogPattern = "log{0}.log";
        private readonly string _logDirectory;
        private readonly int _maxRolledFiles;
        private readonly long _maxSize;
        private readonly string _logFile;
        private readonly List<string> _logCache;

        public Logger()
        {
            _logDirectory = ApplicationConfig.ConfigPath;
            _maxRolledFiles = ApplicationConfig.LogRolledFiles;
            _maxSize = ApplicationConfig.LogSizeLimit;
            _logFile = Path.Combine(_logDirectory, string.Format(LogPattern, string.Empty));
            _logCache = new List<string>();
        }

        public void LogDebug(string component, string message)
        {
            if (ApplicationConfig.MaxLogLevel.Equals(LogLevel.DEBUG))
                WriteMessageToLog(LogLevel.DEBUG, component, message);
        }

        public void LogInfo(string component, string message)
        {
            if (ApplicationConfig.MaxLogLevel >= LogLevel.INFO)
                WriteMessageToLog(LogLevel.INFO, component, message);
        }

        public void LogException(string component, string message, Exception e = null)
        {
            if (ApplicationConfig.MaxLogLevel >= LogLevel.ERROR)
                WriteExceptionToLog(component, message, e);
        }

        private void WriteMessageToLog(LogLevel logLevel, string component, string message)
        {
            string logLevelStr = $"[{logLevel}]".PadRight(7);
            string componentStr = component.PadRight(16);
            string content = string.Format("{1} - {2} {3} - {4}{0}",
                Environment.NewLine,
                DateTime.Now,
                logLevelStr,
                componentStr,
                message);
            WriteToLog(content);
        }

        private void WriteExceptionToLog(string component, string message, Exception e)
        {
            string logLevelStr = $"[{LogLevel.ERROR}]".PadRight(7);
            string componentStr = component.PadRight(16);
            string content = $"{DateTime.Now} - {logLevelStr} {componentStr} - {message} {Environment.NewLine}";

            if (e != null)
                content += e + Environment.NewLine;

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
                lock (_logFile)
                {
                    if (!Directory.Exists(ApplicationConfig.ConfigPath))
                    {
                        AddToCache(content); //First initialization, keep info in cache
                        return;
                    }

                    if (_logCache.Count != 0)
                    {
                        ProcessCache(); //Cache not empty, process it before continuing
                    }

                    RollLogFile();
                    File.AppendAllText(_logFile, content, Encoding.UTF8);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void AddToCache(string content)
        {
            _logCache.Add(content);
        }

        private void ProcessCache()
        {
            var cache = new List<string>(_logCache);
            _logCache.Clear();

            foreach (var line in cache)
            {
                WriteToLog(line);
            }
        }
    }
}
