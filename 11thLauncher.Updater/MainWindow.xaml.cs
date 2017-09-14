using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Linq;

namespace _11thLauncher.Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _executionPath;
        private readonly string _zipFile = Path.GetTempPath() + "11thLauncher.zip";
        private readonly string _exeFile = Path.GetTempPath() + "11thLauncher.exe";
        private readonly string _md5File = Path.GetTempPath() + "11thLauncher.zip.md5";
        private readonly Uri _downloadUri;
        private readonly Uri _hashDownloadUri;

        public MainWindow(string executionPath, string downloadUri, string hashUri)
        {
            InitializeComponent();

            _executionPath = executionPath;
            _downloadUri = new Uri(downloadUri);
            _hashDownloadUri = new Uri(hashUri);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DownloadHashFile();
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
                    label_status.Content = (e.BytesReceived / 1000) + "KB / " + (e.TotalBytesToReceive / 1000) + "KB (" + e.ProgressPercentage + "%)";
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
                CleanUp();
            }
        }

        private void ProcessFile()
        {
            try
            {
                //Wait to make sure the launcher has closed
                Thread.Sleep(1000);

                //Get hash from downloaded hash file
                string expectedHash = File.ReadAllLines(_md5File).First().Split(' ').First();
                File.Delete(_md5File);

                //Verify hash
                string hexHash;
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(_zipFile))
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

                //Extract zip file and delete it
                ZipFile.ExtractToDirectory(_zipFile, Path.GetTempPath());
                File.Delete(_zipFile);

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
                CleanUp();
            }
        }

        private void DownloadHashFile()
        {
            try
            {
                WebClient client = new WebClient();

                //Delete previous temp file if it exists
                File.Delete(_md5File);

                //Download file
                client.DownloadFile(_hashDownloadUri, _md5File);
            }
            catch (Exception)
            {
                CleanUp();
            }
        }

        private void CleanUp()
        {
            //Cleanup, delete temp files
            File.Delete(_md5File);
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
