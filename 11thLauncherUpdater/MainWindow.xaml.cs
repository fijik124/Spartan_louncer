using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows;
using System.Threading;
using System.Diagnostics;

namespace _11thLauncherUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _executionPath;
        private string _zipFile = Path.GetTempPath() + "11thLauncher.zip";
        private Uri _downloadURI;

        public MainWindow(string executionPath, string downloadURI)
        {
            InitializeComponent();

            _executionPath = executionPath;
            _downloadURI = new Uri(downloadURI);

            DownloadUpdate();
        }

        private void DownloadUpdate()
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += (s, e) =>
                {
                    //Update download status
                    label_status.Content = "Descargando actualización: " + (e.BytesReceived / 1000) + "Kb de " + (e.TotalBytesToReceive / 1000) + "Kb (" + e.ProgressPercentage + "%)";
                    progressBar.Value = e.ProgressPercentage;
                };
                client.DownloadFileCompleted += (s, e) =>
                {
                    //Process finished download
                    ProcessFile();
                };

                //Delete zip file if it exists
                File.Delete(_zipFile);

                //Download file
                client.DownloadFileAsync(_downloadURI, _zipFile);
            }
            catch (Exception)
            {
                //Cleanup, delete downloaded file if it exists
                File.Delete(_zipFile);

                //Start old launcher
                Process p = new Process();
                p.StartInfo.FileName = _executionPath;
                p.StartInfo.Arguments = "-updateFailed";
                p.Start();

                //Close updater
                Application.Current.Shutdown();
            }
        }

        private void ProcessFile()
        {
            //Wait to make sure the launcher has closed
            Thread.Sleep(1000);

            //Delete original launcher
            File.Delete(_executionPath);

            //Extract zip file and delete it
            ZipFile.ExtractToDirectory(_zipFile, Path.GetDirectoryName(_executionPath));
            File.Delete(_zipFile);

            //Wait a bit to show finalization
            Thread.Sleep(3000);

            //Start new launcher
            Process p = new Process();
            p.StartInfo.FileName = _executionPath;
            p.StartInfo.Arguments = "-updated";
            p.Start();

            //Close updater
            Application.Current.Shutdown();
        }
    }
}
