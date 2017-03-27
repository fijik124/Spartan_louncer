using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;
using _11thLauncher.Model;

namespace _11thLauncher.Net
{
    public static class Updater
    {
        public static bool Updated = false;
        public static bool UpdateFailed = false;

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
                using (Stream stream = client.OpenRead(Constants.VersionUrl))
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string versionRaw = reader.ReadToEnd();
                            string[] versionData = versionRaw.Split('\n');
                            _latestVersion = versionData[1];
                        }
                    }

                if (_latestVersion != Constants.CurrentVersion)
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
            File.WriteAllBytes(Constants.UpdaterPath, Properties.Resources._11thLauncherUpdater);

            var appPath = Assembly.GetExecutingAssembly().Location;
            var fullPath = "";
            if (appPath != null)
            {
                fullPath = Path.GetFullPath(appPath);
            }

            //Execute updater
            var p = new Process
            {
                StartInfo =
                {
                    FileName = Constants.UpdaterPath,
                    Arguments =
                        $"\"{fullPath}\"" + " " +
                        (Constants.DownloadBaseUrl + "11thLauncher" + _latestVersion + ".zip")

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
            File.Delete(Constants.UpdaterPath);
        }
    }
}
