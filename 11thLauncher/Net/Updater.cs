using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;

namespace _11thLauncher.Net
{
    static class Updater
    {
        public static bool Updated = false;
        public static bool UpdateFailed = false;

        private const string VersionUrl = "http://raw.githubusercontent.com/11thmeu/launcher/master/bin/version";
        private const string DownloadBaseUrl = "https://raw.githubusercontent.com/11thmeu/launcher/master/bin/";
        private static readonly string UpdaterPath = Path.Combine(Path.GetTempPath(), "11thLauncherUpdater.exe");

        private static readonly string _currentVersion = "210";
        private static string _latestVersion = "";

        /// <summary>
        /// Check if there is a new version available
        /// </summary>
        /// <param name="manualCheck">The check has been called manually, show message if there are no updates</param>
        public static void CheckVersion(bool manualCheck)
        {
            MainWindow.UpdateForm("UpdateStatusBar", new object[] { "Comprobando actualizaciones" });

            try
            {
                WebClient client = new WebClient();
                using (Stream stream = client.OpenRead(VersionUrl))
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string versionRaw = reader.ReadToEnd();
                            string[] versionData = versionRaw.Split('\n');
                            _latestVersion = versionData[1];
                        }
                    }

                if (_latestVersion != _currentVersion)
                {
                    MainWindow.UpdateForm("ShowUpdateNotification", new object[] { _latestVersion, true });
                } else if (manualCheck)
                {
                    MainWindow.UpdateForm("ShowUpdateNotification", new object[] { _latestVersion, false });
                }
            }
            catch (Exception)
            {
                MainWindow.UpdateForm("ShowUpdateNotification", new object[] { null, false });
            }
        }

        /// <summary>
        /// Extract and execute external updater, then close the application
        /// </summary>
        public static void ExecuteUpdater()
        {
            //Extract updater
            File.WriteAllBytes(UpdaterPath, Properties.Resources._11thLauncherUpdater);

            //Execute updater
            Process p = new Process
            {
                StartInfo =
                {
                    FileName = UpdaterPath,
                    Arguments =
                        $"\"{Path.GetFullPath(Assembly.GetExecutingAssembly().Location)}\"" + " " +
                        (DownloadBaseUrl + "11thLauncher" + _latestVersion + ".zip")
                }
            };
            p.Start();

            Application.Current.Shutdown();
        }

        /// <summary>
        /// Delete the program updater if it exists in the system
        /// </summary>
        public static void RemoveUpdater()
        {
            File.Delete(UpdaterPath);
        }
    }
}
