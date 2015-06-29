using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;

namespace _11thLauncher.Net
{
    class Updater
    {
        public static bool Updated = false;
        public static bool UpdateFailed = false;

        private static readonly string _versionURL = "http://raw.githubusercontent.com/11thmeu/launcher/master/bin/version";
        private static readonly string _downloadBaseURL = "https://raw.githubusercontent.com/11thmeu/launcher/master/bin/";
        private static readonly string _updaterPath = Path.Combine(Path.GetTempPath(), "11thLauncherUpdater.exe");

        private static readonly string _currentBuildType = "dev";
        private static readonly string _currentVersion = "200_dev28062015";
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
                using (Stream stream = client.OpenRead(_versionURL))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string versionRaw = reader.ReadToEnd();
                    string[] versionData = versionRaw.Split('\n');
                    if (_currentBuildType.Equals("stable"))
                    {
                        _latestVersion = versionData[1];
                    }
                    else
                    {
                        _latestVersion = versionData[3];
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
            File.WriteAllBytes(_updaterPath, Properties.Resources._11thLauncherUpdater);

            //Execute updater
            Process p = new Process();
            p.StartInfo.FileName = _updaterPath;
            p.StartInfo.Arguments = string.Format("\"{0}\"", Path.GetFullPath(Assembly.GetExecutingAssembly().Location)) + " " + (_downloadBaseURL + "11thLauncher" + _latestVersion + ".zip");
            p.Start();

            Application.Current.Shutdown();
        }

        /// <summary>
        /// Delete the program updater if it exists in the system
        /// </summary>
        public static void RemoveUpdater()
        {
            File.Delete(_updaterPath);
        }
    }
}
