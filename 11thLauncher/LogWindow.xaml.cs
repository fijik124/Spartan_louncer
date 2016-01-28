using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using MahApps.Metro.Controls;
using _11thLauncher.LogViewer;

namespace _11thLauncher
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : MetroWindow
    {
        internal static LogWindow Form;
        private delegate void PrintLineCallBack(string text);

        public LogWindow()
        {
            InitializeComponent();
            Form = this;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            new Thread(PrintFile).Start();

            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = Logger.GetLatestFile().DirectoryName,
                NotifyFilter = NotifyFilters.LastWrite,
                IncludeSubdirectories = false,
                Filter = Path.GetFileName(Logger.GetLatestFile().FullName)
            };
            watcher.Changed += OnFileChange;
            watcher.EnableRaisingEvents = true;
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Form = null;
        }

        private void OnFileChange(object source, FileSystemEventArgs e)
        {
            string[] lines = Logger.ReadNewLines();
            foreach (string line in lines)
            {
                PrintLine(line);
            }
        }

        private void PrintFile()
        {
            string[] lines = Logger.Readfile();
            foreach (string line in lines)
            {
                PrintLine(line);
            }
        }

        private void PrintLine(string text)
        {
            if (Form != null)
            {
                if (Form.Dispatcher.CheckAccess())
                {
                    richTextBox.AppendText(text);
                    richTextBox.AppendText("\n");
                }
                else
                {
                    Dispatcher.Invoke(() => PrintLine(text));
                }
            }
        }
    }
}
