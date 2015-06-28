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

        public LogWindow()
        {
            InitializeComponent();
            Form = this;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //AppendText(richTextBox, "Texto \n", "Red");
            //AppendText(richTextBox, "Texto", "Green");

            new Thread(new ThreadStart(PrintFile)).Start();

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = Logger.getLatestFile().DirectoryName;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.IncludeSubdirectories = false;
            watcher.Filter = Path.GetFileName(Logger.getLatestFile().FullName);
            watcher.Changed += new FileSystemEventHandler(OnFileChange);
            watcher.EnableRaisingEvents = true;
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Form = null;
        }

        private void OnFileChange(object source, FileSystemEventArgs e)
        {
            string[] lines = Logger.readNewLines();
            foreach (string line in lines)
            {
                PrintLine(line);
            }
        }

        private void PrintFile()
        {
            string[] lines = Logger.readfile();
            foreach (string line in lines)
            {
                PrintLine(line);
                //Form.Dispatcher.Invoke(PrintLine);
            }
        }

        private void PrintLine(string text)
        {
            BrushConverter bc = new BrushConverter();
            TextRange tr = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentEnd);
            tr.Text = text;
            try
            {
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertFromString("Black"));
            }
            catch (FormatException) { }
            richTextBox.AppendText("\n");
        }
    }
}
