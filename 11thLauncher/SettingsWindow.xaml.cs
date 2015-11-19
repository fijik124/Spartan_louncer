using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using _11thLauncher.Configuration;
using _11thLauncher.Net;

namespace _11thLauncher
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        private bool _restarting = false;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //
            //Load settings
            //

            //General
            checkBox_checkUpdates.IsChecked = Settings.CheckUpdates;
            checkBox_checkServers.IsChecked = Settings.CheckServers;
            checkBox_checkRepository.IsChecked = Settings.CheckRepository;
            textBox_gamePath.Text = Settings.Arma3Path;
            if (Settings.StartClose)
            {
                comboBox_startAction.SelectedIndex = 1;
            }
            else if (Settings.StartMinimize)
            {
                comboBox_startAction.SelectedIndex = 2;
            }

            //Interface
            comboBox_accent.SelectedIndex = Settings.Accent;
            if (Settings.MinimizeNotification)
            {
                comboBox_minimize.SelectedIndex = 1;
            }
            checkBox_serversGroupBox.IsChecked = Settings.ServersGroupBox;
            checkBox_repositoryGroupBox.IsChecked = Settings.RepositoryGroupBox;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //If application is restarting, dont save settings
            if (_restarting) return;

            //
            //Save current settings
            //

            //General
            Settings.CheckUpdates = (bool)checkBox_checkUpdates.IsChecked;
            Settings.CheckServers = (bool)checkBox_checkServers.IsChecked;
            Settings.CheckRepository = (bool)checkBox_checkRepository.IsChecked;
            //If game path has changed, read addons
            if (Settings.Arma3Path != textBox_gamePath.Text && textBox_gamePath.Text != "")
            {
                Settings.Arma3Path = textBox_gamePath.Text;
                MainWindow.Form.Addons.Clear();
                Addons.ReadAddons();
                foreach (string addon in Addons.LocalAddons)
                {
                    MainWindow.Form.Addons.Add(new Addon() { Enabled = false, Name = addon });
                }
            }
            Settings.StartClose = false;
            Settings.StartMinimize = false;
            if (comboBox_startAction.SelectedIndex == 1)
            {
                Settings.StartClose = true;
            }
            else if (comboBox_startAction.SelectedIndex == 2)
            {
                Settings.StartMinimize = true;
            }

            //Repository
            Settings.JavaPath = textBox_javaPath.Text;
            Settings.Arma3SyncPath = textBox_a3sPath.Text;
            if (comboBox_repository.SelectedIndex == -1)
            {
                Settings.Arma3SyncRepository = "";
            } else
            {
                Settings.Arma3SyncRepository = comboBox_repository.SelectedItem.ToString();
            }
            /* TODO
            if (!MainWindow.Form.tile_repositoryStatus.IsEnabled)
            {
                //Check if repository is correctly configured to allow checking
                if ((Repository.JavaVersion != "" || Settings.JavaPath != "") && Settings.Arma3SyncPath != "" && Settings.Arma3SyncRepository != "")
                {
                    MainWindow.Form.image_arma3Sync.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/a3sEnabled.png"));
                    MainWindow.Form.image_arma3Sync.ToolTip = "Arma3Sync está configurado. Click para iniciar";
                    MainWindow.Form.tile_repositoryStatus.IsEnabled = true;
                    MainWindow.Form.tile_repositoryStatus.Background = new SolidColorBrush(Colors.Orange);
                    MainWindow.Form.tile_repositoryStatus.ToolTip = "Click para comprobar estado";
                }
            } else
            {
                //Check if configuration is incorrect to disable repository checking
                if ((Repository.JavaVersion == "" && Settings.JavaPath == "") || Settings.Arma3SyncPath == "" || Settings.Arma3SyncRepository == "")
                {
                    MainWindow.Form.image_arma3Sync.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/a3sDisabled.png"));
                    MainWindow.Form.image_arma3Sync.ToolTip = "Arma3Sync no está configurado, configuralo en la ventana de opciones";
                    MainWindow.Form.tile_repositoryStatus.IsEnabled = false;
                    MainWindow.Form.tile_repositoryStatus.Background = new SolidColorBrush(Colors.Gray);
                    MainWindow.Form.tile_repositoryStatus.ToolTip = null;
                }
            }*/

            //Interface
            Settings.Accent = comboBox_accent.SelectedIndex;
            Settings.MinimizeNotification = false;
            if (comboBox_minimize.SelectedIndex == 1)
            {
                Settings.MinimizeNotification = true;
            }
            Settings.ServersGroupBox = (bool)checkBox_serversGroupBox.IsChecked;
            Settings.RepositoryGroupBox = (bool)checkBox_repositoryGroupBox.IsChecked;

            Settings.Write();
        }

        private void comboBox_accent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Settings.Accents[comboBox_accent.SelectedIndex]), ThemeManager.GetAppTheme("BaseLight"));
            Settings.Accent = comboBox_accent.SelectedIndex;
        }

        private void button_selectGamePath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            string path = "";

            dialog.FileName = "arma3.exe";
            dialog.Filter = "Ejecutables (*.exe) | *.exe";

            if (dialog.ShowDialog() == true)
            {
                path = Path.GetDirectoryName(dialog.FileName);
            }

            if (!path.Equals(""))
            {
                textBox_gamePath.Text = path;
            }
        }

        private void button_selectJavaPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            string path = "";

            dialog.FileName = "java.exe";
            dialog.Filter = "Ejecutables (*.exe) | *.exe";

            if (dialog.ShowDialog() == true)
            {
                path = dialog.FileName;
            }

            if (!path.Equals(""))
            {
                Settings.JavaPath = path;
                textBox_javaPath.Text = path;
            }
        }

        private void button_selectA3sPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            string path = "";

            dialog.FileName = "ArmA3Sync.exe";
            dialog.Filter = "Ejecutables (*.exe) | *.exe";

            if (dialog.ShowDialog() == true)
            {
                path = Path.GetDirectoryName(dialog.FileName);
            }

            if (!path.Equals(""))
            {
                Settings.Arma3SyncPath = path;
                textBox_a3sPath.Text = path;
                //List<string> repositories = Repository.ListRepositories(); TODO
                //comboBox_repository.ItemsSource = repositories;
            }
        }

        private async void button_deleteConfiguration_Click(object sender, RoutedEventArgs e)
        {
            MessageDialogResult result = await this.ShowMessageAsync("Confirmar borrado", "¿Estás seguro de que quieres borrar toda la información y perfiles guardados?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                Settings.Delete();
                _restarting = true;
                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
            }
        }
    }
}
