using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace _11thLauncherUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _executionPath;
        private readonly string _zipFile = Path.GetTempPath() + "11thLauncher.zip";
        private readonly string _exeFile = Path.GetTempPath() + "11thLauncher.exe";
        private readonly Uri _downloadUri;
        private const string VersionUri = "http://raw.githubusercontent.com/11thmeu/launcher/master/bin/version";

        public MainWindow(string executionPath, string downloadUri)
        {
            InitializeComponent();

            _executionPath = executionPath;
            _downloadUri = new Uri(downloadUri);
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
                client.DownloadFileAsync(_downloadUri, _zipFile);
            }
            catch (Exception)
            {
                //Cleanup, delete downloaded file if it exists
                File.Delete(_zipFile);
                File.Delete(_exeFile);

                //Start old launcher
                Process p = new Process
                {
                    StartInfo =
                    {
                        FileName = _executionPath,
                        Arguments = "-updateFailed"
                    }
                };
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

                //Download version file to check hash
                string expectedHash = "";
                WebClient client = new WebClient();
                using (Stream stream = client.OpenRead(VersionUri))
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string versionRaw = reader.ReadToEnd();
                            string[] versionData = versionRaw.Split('\n');
                            expectedHash = versionData[5];
                        }
                    }

                //Verify hash
                string hexHash;
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(_exeFile))
                    {
                        byte[] rawHash = md5.ComputeHash(stream);

                        //Convert hash to string
                        StringBuilder stringbuilder = new StringBuilder();
                        foreach (byte t in rawHash)
                        {
                            stringbuilder.Append(t.ToString("x2"));
                        }
                        hexHash = stringbuilder.ToString();
                    }
                }

                //If hash doesn't match throw exception
                if (hexHash != expectedHash) throw new InvalidDataException();

                //Delete original launcher and move new one
                File.Delete(_executionPath);
                File.Move(_exeFile, _executionPath);

                //Wait a bit to show finalization
                Thread.Sleep(3000);

                //Start new launcher
                Process p = new Process
                {
                    StartInfo =
                    {
                        FileName = _executionPath,
                        Arguments = "-updated"
                    }
                };
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
                    Process p = new Process
                    {
                        StartInfo =
                        {
                            FileName = _executionPath,
                            Arguments = "-updateFailed"
                        }
                    };
                    p.Start();
                }

                //Close updater
                Application.Current.Shutdown();
            }
        }
    }
}
