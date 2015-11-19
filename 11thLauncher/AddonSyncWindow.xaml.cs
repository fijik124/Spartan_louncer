using System;
using System.Windows;
using MahApps.Metro.Controls;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using _11thLauncher.AddonSync;
using System.Threading;
using _11thLauncher.AddonSync.Domain;

namespace _11thLauncher
{
    /// <summary>
    /// Interaction logic for AddonSyncWindow.xaml
    /// </summary>
    public partial class AddonSyncWindow : MetroWindow
    {
        internal static AddonSyncWindow Form;

        private delegate void UpdateFormCallBack(string method, object[] parameters);
        //SyncTreeDirectory syncTreeObject;

        //Check/Download variables
        private int currentProgress;
        private int maxProgress;
        private Thread workThread;

        public AddonSyncWindow()
        {
            InitializeComponent();
            Form = this;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //ADDONSYNC CUSTOM TESTS
            Console.WriteLine(@"Testing addonsync system !!!");
            Repository r = new Repository();
            r.ReadLocalRepository();


            Console.WriteLine(@"Finished addonsync test !!!");
            /*
            //ADDONSYNC TESTS 
            Console.WriteLine("Testing addonsync system !!!");

            syncTreeObject = new SyncTreeDirectory();
            XmlSerializer serializer = new XmlSerializer(typeof(SyncTreeDirectory));
            FileStream fs = new FileStream(@"C:\Users\Javier\Downloads\syncEXP.xml", FileMode.Open);
            syncTreeObject = (SyncTreeDirectory)serializer.Deserialize(fs);

            treeView.ItemsSource = syncTreeObject.List;

            SyncTreeDirectory ace = (SyncTreeDirectory)syncTreeObject.List[0];
            SyncTreeDirectory addons = (SyncTreeDirectory)ace.List[0];
            SyncTreeLeaf ballistics = (SyncTreeLeaf)addons.List[0];

            //ballistics.ComputeLocalHash(false);

            Console.WriteLine("Finished addonsync test !!!");
            */
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Form = null;
        }

        private void button_checkAddons_Click(object sender, RoutedEventArgs e)
        {
            button_checkAddons.IsEnabled = false;
            button_cancelCheck.IsEnabled = true;
            button_verifyAddons.IsEnabled = false;
            button_deleteExtraFiles.IsEnabled = false;

            button_startDownload.IsEnabled = false;

            currentProgress = 0;
            //maxProgress = syncTreeObject.CountLeaves();
            progressBar_checkAddons.Value = 0;
            progressBar_checkAddons.Maximum = maxProgress;

            UpdateStatusBar("Comprobando archivo 1 de " + maxProgress);

            //new Thread(() => syncTreeObject.CheckFiles(false));
            //syncTreeObject.CheckFiles(false);
            //workThread = new Thread(new ParameterizedThreadStart(syncTreeObject.CheckFiles));
            //workThread.Start(false);
        }

        private void checkBox_selectAll_Checked(object sender, RoutedEventArgs e)
        {
            /*
            foreach (SyncTreeNode node in syncTreeObject.List)
            {
                node.IsSelected = true;
            }
            */
        }

        private void checkBox_selectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            /*
            foreach (SyncTreeNode node in syncTreeObject.List)
            {
                node.IsSelected = false;
            }
            */
        }

        /// <summary>
        /// Callback method for updating the display
        /// </summary>
        /// <param name="method">Method to execute</param>
        /// <param name="parameters">Method parameters</param>
        public static void UpdateForm(string method, object[] parameters)
        {
            if (Form == null) return;
            if (Form.Dispatcher.CheckAccess())
            {
                typeof(AddonSyncWindow).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Form, parameters);
            }
            else
            {
                Form.Dispatcher.Invoke(new UpdateFormCallBack(UpdateForm), method, parameters);
            }
        }

        /// <summary>
        /// Update the status bar contents
        /// </summary>
        /// <param name="text">(optional) Text to show</param>
        private void UpdateStatusBar(string text = "")
        {
            if (string.IsNullOrEmpty(text))
            {
                statusBar.Text = "Listo";
                progressBar_statusBar.IsIndeterminate = false;
            }
            else
            {
                statusBar.Text = text;
                progressBar_statusBar.IsIndeterminate = true;
            }
        }

        private void UpdateCheckStatus(string name, bool computingHash)
        {
            currentProgress++;
            progressBar_checkAddons.Value = currentProgress;

            if (computingHash)
            {
                UpdateStatusBar("[" + currentProgress + "/" + maxProgress + "] " + "Calculando hash de " + name);
            } else
            {
                UpdateStatusBar("[" + currentProgress + "/" + maxProgress + "] " + "Comprobando archivo " + name);
            }

            if (currentProgress == maxProgress)
            {
                UpdateStatusBar();

                button_checkAddons.IsEnabled = true;
                button_cancelCheck.IsEnabled = false;
                button_verifyAddons.IsEnabled = true;
                button_deleteExtraFiles.IsEnabled = true;

                button_startDownload.IsEnabled = true;
            }
        }

        private void button_cancelCheck_Click(object sender, RoutedEventArgs e)
        {
            workThread.Abort();

            button_checkAddons.IsEnabled = true;
            button_cancelCheck.IsEnabled = false;
        }
    }
}
