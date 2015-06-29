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
        private readonly string _zipFile = Path.GetTempPath() + "11thLauncher.zip";
        private readonly string _exeFile = Path.GetTempPath() + "11thLauncher.exe";
        private Uri _downloadURI;

        public MainWindow(string executionPath, string downloadURI)
        {
            InitializeComponent();

            _executionPath = executionPath;
            _downloadURI = new Uri(downloadURI);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
                    label_status.Content = "Descargando actualización: " + (e.BytesReceived / 1000) + "KB de " + (e.TotalBytesToReceive / 1000) + "KB (" + e.ProgressPercentage + "%)";
                    progressBar.Value = e.ProgressPercentage;
                };
                client.DownloadFileCompleted += (s, e) =>
                {
                    //Process finished download
                    ProcessFile();
                };

                //Delete previous temp files if they exist
                File.Delete(_zipFile);
                File.Delete(_exeFile);

                //Download file
                client.DownloadFileAsync(_downloadURI, _zipFile);
            }
            catch (Exception)
            {
                //Cleanup, delete downloaded file if it exists
                File.Delete(_zipFile);
                File.Delete(_exeFile);

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
            try
            {
                //Wait to make sure the launcher has closed
                Thread.Sleep(1000);

                //Extract zip file and delete it
                ZipFile.ExtractToDirectory(_zipFile, Path.GetTempPath());
                File.Delete(_zipFile);

                //Delete original launcher and move new one
                File.Delete(_executionPath);
                File.Move(_exeFile, _executionPath);

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
            catch (Exception)
            {
                //Cleanup, delete temp files
                File.Delete(_zipFile);
                File.Delete(_exeFile);

                //Try to start launcher
                if (File.Exists(_executionPath))
                {
                    Process p = new Process();
                    p.StartInfo.FileName = _executionPath;
                    p.StartInfo.Arguments = "-updateFailed";
                    p.Start();
                }

                //Close updater
                Application.Current.Shutdown();
            }
        }
    }
}
