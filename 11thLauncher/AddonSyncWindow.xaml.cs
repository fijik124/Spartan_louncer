using System;
using System.Windows;
using MahApps.Metro.Controls;
using _11thLauncher.AddonSync;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Media;

namespace _11thLauncher
{
    /// <summary>
    /// Interaction logic for AddonSyncWindow.xaml
    /// </summary>
    public partial class AddonSyncWindow : MetroWindow
    {
        internal static AddonSyncWindow Form;
        SyncTreeDirectory syncTreeObject;

        public AddonSyncWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {


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

            ballistics.ComputeLocalHash();

            Console.WriteLine("Finished addonsync test !!!");
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Form = null;
        }
    }
}
