using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _11thLauncher.LogViewer
{
    public partial class LogViewerForm : Form
    {
        public LogViewerForm()
        {
            InitializeComponent();
            Program.viewer = this;
        }

        private void LogViewerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.viewer = null;
        }

        private void LogViewerForm_Shown(object sender, EventArgs e)
        {
            string[] lines = Logger.readfile();
            foreach (string line in lines)
            {
                PrintLine(line);
            }

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = Logger.getLatestFile().DirectoryName;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.IncludeSubdirectories = false;
            watcher.Filter = Path.GetFileName(Logger.getLatestFile().FullName);
            watcher.Changed += new FileSystemEventHandler(OnFileChange);
            watcher.EnableRaisingEvents = true;

            label_viewerStatus.Text = "Monitorizando el archivo: " + Logger.getLatestFile().Name;
            picture_logStatus.Image = global::_11thLauncher.Properties.Resources.image_856487;
        }

        private void OnFileChange(object source, FileSystemEventArgs e)
        {
            string[] lines = Logger.readNewLines();
            foreach (string line in lines)
            {
                PrintLine(line);
            }
        }

        private delegate void PrintLineCallback(String line);
        public void PrintLine(String line)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new PrintLineCallback(PrintLine), line);
            }
            else
            {
                int lineStartIndex = logTextBox.TextLength;
                logTextBox.AppendText(line);

                foreach (TextPattern pattern in Logger.patterns)
                {
                    MatchCollection matches = pattern.getMatches(line);
                    foreach (Match m in matches)
                    {
                        logTextBox.SelectionStart = lineStartIndex + m.Index;
                        logTextBox.SelectionLength = m.Length;

                        logTextBox.SelectionColor = pattern.Color;
                        logTextBox.SelectionFont = pattern.Font;
                    }
                }

                logTextBox.AppendText("\n");
                logTextBox.ScrollToCaret();
            }
        }
    }
}