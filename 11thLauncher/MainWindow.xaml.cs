using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using _11thLauncher.Configuration;
using _11thLauncher.Net;

namespace _11thLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        internal static MainWindow Form;

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private delegate void UpdateFormCallBack(string method, object[] parameters);
        internal ObservableCollection<Addon> addons = new ObservableCollection<Addon>();

        public MainWindow()
        {
            InitializeComponent();
            Form = this;

            //Initialize system tray icon
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.BalloonTipTitle = "11th Launcher";
            notifyIcon.Icon = Properties.Resources.icon;
            notifyIcon.Text = "11th Launcher";
            notifyIcon.Visible = false;
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_Click);
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.ConfigExists())
            {
                Settings.Read();
            } else
            {
                if (!Settings.ReadPath())
                {
                    this.ShowMessageAsync("Ruta de ejecución", "La ruta de ejecución no ha sido autodetectada, indicala manualmente en la ventana de opciones antes de empezar a utilizar la aplicación", MessageDialogStyle.Affirmative);
                }

                //Create default profile
                Profiles.CreateDefault();

                //Save settings
                Settings.Write();
            }

            //Add local addons
            Addons.ReadAddons();
            foreach (string addon in Addons.LocalAddons)
            {
                addons.Add(new Addon() { Enabled = false, Name = addon });
            }
            listBox_addons.ItemsSource = addons;

            //Add profiles
            UpdateProfiles();
            comboBox_profiles.SelectedIndex = comboBox_profiles.Items.IndexOf(Profiles.DefaultProfile); //Select default profile

            //Set accent
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Settings.Accents[Settings.Accent]), ThemeManager.GetAppTheme("BaseLight"));

            //Set groupboxes preferences
            groupBox_servers.IsExpanded = Settings.ServersGroupBox;
            groupBox_repository.IsExpanded = Settings.RepositoryGroupBox;

            //If just updated, remove updater and show notification
            if (Updater.Updated)
            {
                Updater.RemoveUpdater();
                this.ShowMessageAsync("Aplicación actualizada", "La aplicación ha sido actualizada con éxito", MessageDialogStyle.Affirmative);
            }

            //Notify if update failed
            if (Updater.UpdateFailed)
            {
                Updater.RemoveUpdater();
                this.ShowMessageAsync("Error al actualizar", "Se ha producido un error al actualizar la aplicación, vuelve a intentarlo más tarde", MessageDialogStyle.Affirmative);
            }

            //Check Java presence
            Repository.CheckJava();

            //Check Arma3Sync configuration
            if ((Repository.JavaVersion != "" || Settings.JavaPath != "") && Settings.Arma3SyncPath != "" && Settings.Arma3SyncRepository != "")
            {
                image_arma3Sync.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/a3sEnabled.png"));
                image_arma3Sync.ToolTip = "Arma3Sync está configurado. Click para iniciar";
                tile_repositoryStatus.IsEnabled = true;
                tile_repositoryStatus.Background = new SolidColorBrush(Colors.Orange);
                tile_repositoryStatus.ToolTip = "Click para comprobar estado";
            }

            //Check repository for updates
            if (Settings.CheckRepository)
            {
                tile_repositoryStatus.IsEnabled = false;
                new Thread(new ThreadStart(Repository.CheckRepository)).Start();
            }

            //Check servers status
            if (Settings.CheckServers)
            {
                new Thread(new ThreadStart(Servers.CheckServers)).Start();
            }

            //Check for updates
            if (Settings.CheckUpdates)
            {
                new Thread(() => Updater.CheckVersion(false)).Start();
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Form = null;
        }

        private void MetroWindow_StateChanged(object sender, EventArgs e)
        {
            if (Settings.MinimizeNotification)
            {
                if (WindowState == WindowState.Minimized)
                {
                    ShowInTaskbar = false;
                    notifyIcon.Visible = true;
                }
                else
                {
                    ShowInTaskbar = true;
                    notifyIcon.Visible = false;
                }
            }
        }

        private void notifyIcon_Click(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void menu_start_sp_Click(object sender, RoutedEventArgs e)
        {
            StartArmA3(false, true, false);

            if (Settings.StartClose)
            {
                Application.Current.Shutdown();
            }

            if (Settings.StartMinimize)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void menu_start_mp_Click(object sender, RoutedEventArgs e)
        {
            StartArmA3(true, true, false);

            if (Settings.StartClose)
            {
                Application.Current.Shutdown();
            }

            if (Settings.StartMinimize)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void menu_start_hc_Click(object sender, RoutedEventArgs e)
        {
            StartArmA3(true, true, true);

            if (Settings.StartClose)
            {
                Application.Current.Shutdown();
            }

            if (Settings.StartMinimize)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void menu_exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void menu_logViewer_Click(object sender, RoutedEventArgs e)
        {
            LogWindow logWindow = new LogWindow();
            logWindow.Owner = this;
            logWindow.Show();
        }

        private void menu_settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }

        private void menu_updates_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => Updater.CheckVersion(true)).Start();
        }

        private void menu_about_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }

        private void comboBox_profiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_profiles.SelectedIndex == -1) return;

            //Clear addon presets comboBox selection
            comboBox_addons.SelectedIndex = -1;

            //Read selected profile
            Profiles.ReadProfile(comboBox_profiles.SelectedItem.ToString());

            //Update the display with the profile
            UpdateForProfile();
        }

        private void button_saveProfile_Click(object sender, RoutedEventArgs e)
        {
            UpdateProfileValues();
            Profiles.WriteProfile(comboBox_profiles.SelectedItem.ToString());
        }

        private void comboBox_addons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_addons.SelectedIndex == -1) return;

            List<string> presetAddons;

            //Clear addons list
            addons.Clear();

            switch (comboBox_addons.SelectedIndex)
            {
                //Guerra moderna
                case 0:
                    presetAddons = new List<string>(new string[] { "@cba", "@ace", "@tfar", "@meu", "@meu_maps", "@fx", "@jsrs" });
                    break;
                //ALiVE
                case 1:
                    presetAddons = new List<string>(new string[] { "@cba", "@ace", "@tfar", "@meu", "@meu_maps", "@fx", "@jsrs", "@alive" });
                    break;
                //default
                default:
                    presetAddons = new List<string>(new string[] { "@cba", "@ace", "@tfar", "@meu", "@meu_maps", "@fx", "@jsrs" });
                    break;
            }

            //Add the active addons
            foreach (string addon in presetAddons)
            {
                if (Addons.LocalAddons.Contains(addon))
                {
                    addons.Add(new Addon() { Name = addon, Enabled = true });
                }
            }

            //Add inactive addons
            foreach (string addon in Addons.LocalAddons)
            {
                if (!presetAddons.Contains(addon))
                {
                    addons.Add(new Addon() { Name = addon, Enabled = false });
                }
            }
        }

        private void button_moveUp_Click(object sender, RoutedEventArgs e)
        {
            int index = listBox_addons.SelectedIndex;
            if (index != -1 && index != 0)
            {
                addons.Move(index, index - 1);
            }
        }

        private void button_moveDown_Click(object sender, RoutedEventArgs e)
        {
            int index = listBox_addons.SelectedIndex;
            if (index != -1 && index != listBox_addons.Items.Count - 1)
            {
                addons.Move(index, index + 1);
            }
        }

        private void button_selectAll_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < listBox_addons.Items.Count; i++)
            {
                ((Addon)listBox_addons.Items[i] as Addon).Enabled = true;
                listBox_addons.Items.Refresh();
            }
        }

        private void button_selectNone_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < listBox_addons.Items.Count; i++)
            {
                ((Addon)listBox_addons.Items[i] as Addon).Enabled = false;
                listBox_addons.Items.Refresh();
            }
        }

        private void button_startSP_Click(object sender, RoutedEventArgs e)
        {
            StartArmA3(false, true, false);

            if (Settings.StartClose)
            {
                Application.Current.Shutdown();
            }

            if (Settings.StartMinimize)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void button_startMP_Click(object sender, RoutedEventArgs e)
        {
            StartArmA3(true, true, false);

            if (Settings.StartClose)
            {
                Application.Current.Shutdown();
            }

            if (Settings.StartMinimize)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void button_startHC_Click(object sender, RoutedEventArgs e)
        {
            StartArmA3(true, true, true);

            if (Settings.StartClose)
            {
                Application.Current.Shutdown();
            }

            if (Settings.StartMinimize)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void button_clipSP_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(StartArmA3(false, false, false));
        }

        private void button_clipMP_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(StartArmA3(true, false, false));
        }

        private void button_clipHC_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(StartArmA3(true, false, true));
        }

        private void image_coopStatus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            image_coopStatus.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/unknown.png"));
            image_coopStatus.ToolTip = "Comprobando estado";
            new Thread(new ParameterizedThreadStart(Servers.CheckServer)).Start(0);
        }

        private void image_academyStatus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            image_academyStatus.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/unknown.png"));
            image_academyStatus.ToolTip = "Comprobando estado";
            new Thread(new ParameterizedThreadStart(Servers.CheckServer)).Start(1);
        }

        private void image_aliveStatus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            image_aliveStatus.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/unknown.png"));
            image_aliveStatus.ToolTip = "Comprobando estado";
            new Thread(new ParameterizedThreadStart(Servers.CheckServer)).Start(2);
        }

        private void button_serverCoop_Click(object sender, RoutedEventArgs e)
        {
            textBox_serverAddress.Text = "11thmeu.es";
            textBox_serverPort.Text = "2302";
        }

        private void button_serverAcademy_Click(object sender, RoutedEventArgs e)
        {
            textBox_serverAddress.Text = "11thmeu.es";
            textBox_serverPort.Text = "2322";
        }

        private void button_serverAlive_Click(object sender, RoutedEventArgs e)
        {
            textBox_serverAddress.Text = "11thmeu.es";
            textBox_serverPort.Text = "2332";
        }

        private void image_arma3Sync_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Repository.JavaVersion != "" && Settings.Arma3SyncPath != "")
            {
                Repository.StartArmA3Sync();
            }
        }

        private void tile_repositoryStatus_Click(object sender, RoutedEventArgs e)
        {
            label_repositoryRevision.Content = "";
            tile_repositoryStatus.IsEnabled = false;
            tile_repositoryStatus.ToolTip = "Comprobando estado";
            new Thread(new ThreadStart(Repository.CheckRepository)).Start();
        }

        private void button_checkStatus_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = comboBox_server.SelectedIndex;
            if (selectedIndex == -1) return;

            textBox_serverName.Text = "";
            textBox_serverMission.Text = "";
            textBox_serverPing.Text = "";
            textBox_serverMap.Text = "";
            textBox_serverPlayers.Text = "";
            textBox_serverVersion.Text = "";
            listBox_serverPlayers.Items.Clear();
            textBox_serverMods.Text = "";

            button_checkStatus.IsEnabled = false;

            new Thread(new ParameterizedThreadStart(Servers.QueryServerInfo)).Start(selectedIndex);
        }

        private void checkBox_maxMemory_Checked(object sender, RoutedEventArgs e)
        {
            comboBox_maxMemory.IsEnabled = true;
            comboBox_maxMemory.SelectedIndex = 0;
        }

        private void checkBox_maxMemory_Unchecked(object sender, RoutedEventArgs e)
        {
            comboBox_maxMemory.IsEnabled = false;
            comboBox_maxMemory.SelectedIndex = -1;
        }

        private void checkBox_maxVMemory_Checked(object sender, RoutedEventArgs e)
        {
            comboBox_maxVMemory.IsEnabled = true;
            comboBox_maxVMemory.SelectedIndex = 0;
        }

        private void checkBox_maxVMemory_Unchecked(object sender, RoutedEventArgs e)
        {
            comboBox_maxVMemory.IsEnabled = false;
            comboBox_maxVMemory.SelectedIndex = -1;
        }

        private void checkBox_cpuCount_Checked(object sender, RoutedEventArgs e)
        {
            comboBox_cpuCount.IsEnabled = true;
            comboBox_cpuCount.SelectedIndex = 0;
        }

        private void checkBox_cpuCount_Unchecked(object sender, RoutedEventArgs e)
        {
            comboBox_cpuCount.IsEnabled = false;
            comboBox_cpuCount.SelectedIndex = -1;
        }

        private void checkBox_extraThreads_Checked(object sender, RoutedEventArgs e)
        {
            comboBox_extraThreads.IsEnabled = true;
            comboBox_extraThreads.SelectedIndex = 0;
        }

        private void checkBox_extraThreads_Unchecked(object sender, RoutedEventArgs e)
        {
            comboBox_extraThreads.IsEnabled = false;
            comboBox_extraThreads.SelectedIndex = -1;
        }

        private void checkBox_priority_Checked(object sender, RoutedEventArgs e)
        {
            comboBox_priority.IsEnabled = true;
            comboBox_priority.SelectedIndex = 0;
        }

        private void checkBox_priority_Unchecked(object sender, RoutedEventArgs e)
        {
            comboBox_priority.IsEnabled = false;
            comboBox_priority.SelectedIndex = -1;
        }

        private void button_defaultProfile_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_profiles.SelectedIndex == -1)
            {
                this.ShowMessageAsync("Error de selección", "No has seleccionado ningún perfil", MessageDialogStyle.Affirmative);
            } else
            {
                Profiles.DefaultProfile = listBox_profiles.SelectedItem.ToString();
                Settings.Write();
            }
        }

        private void button_createProfile_Click(object sender, RoutedEventArgs e)
        {
            textBox_newProfile.IsEnabled = true;
            textBox_newProfile.Text = "Nuevo perfil";
            button_saveNewProfile.IsEnabled = true;
            textBox_newProfile.Select(0, textBox_newProfile.Text.Length);
            textBox_newProfile.Focus();
        }

        private void button_deleteProfile_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_profiles.SelectedIndex == -1)
            {
                this.ShowMessageAsync("Error de borrado", "No has seleccionado ningún perfil", MessageDialogStyle.Affirmative);
            }
            else
            {
                if (listBox_profiles.SelectedItem.ToString().Equals("Predeterminado"))
                {
                    this.ShowMessageAsync("Error de borrado", "No puedes borrar el perfil predeterminado", MessageDialogStyle.Affirmative);
                }
                else
                {
                    //Check if deleted profile is default
                    if (listBox_profiles.SelectedItem.ToString().Equals(Profiles.DefaultProfile))
                    {
                        Profiles.DefaultProfile = "Predeterminado";
                    }
                    Profiles.DeleteProfile(listBox_profiles.SelectedItem.ToString());
                    UpdateProfiles();
                    comboBox_profiles.SelectedIndex = 0;
                }
            }
        }

        private void button_saveNewProfile_Click(object sender, RoutedEventArgs e)
        {
            //Check that name is not repeated
            if (Profiles.UserProfiles.Contains(textBox_newProfile.Text.Trim()))
            {
                this.ShowMessageAsync("Error de creación de perfil", "Ya existe un perfil con el nombre indicado", MessageDialogStyle.Affirmative);
            }
            else
            {
                Profiles.UserProfiles.Add(textBox_newProfile.Text.Trim());
                Settings.Write();
                Profiles.WriteProfile(textBox_newProfile.Text.Trim());
                UpdateProfiles();
                textBox_newProfile.Text = "";
                textBox_newProfile.IsEnabled = false;
                button_saveNewProfile.IsEnabled = false;
            }
        }

        /// <summary>
        /// Use the current parameters and configurations to start the game or copy the start line to the clipboard
        /// </summary>
        /// <param name="multiplayer">Join the multiplayer server?</param>
        /// <param name="startProcess">Start the game or just copy the line to clipboard?</param>
        /// <param name="headless">Start headless client?</param>
        private string StartArmA3(bool multiplayer, bool startProcess, bool headless)
        {
            //Save the current parameters to profile
            UpdateProfileValues();

            string launchParams = "";

            //Parameters
            if (Profiles.GetParameter("noFilePatching", false))
                launchParams += " -noFilePatching";
            if (Profiles.GetParameter("skipSplashScreen", false))
                launchParams += " -noSplash";
            if (Profiles.GetParameter("windowsXPMode", false))
                launchParams += " -winxp";
            if (Profiles.GetParameter("noPause", false))
                launchParams += " -noPause";
            if (Profiles.GetParameter("showScriptErrors", false))
                launchParams += " -showScriptErrors";
            if (Profiles.GetParameter("emptyWorld", false))
                launchParams += " -world=empty";
            if (Profiles.GetParameter("skipIntro", false))
                launchParams += " -skipIntro";
            if (Profiles.GetParameter("windowedMode", false))
                launchParams += " -window";
            if (Profiles.GetParameter("noCB", false))
                launchParams += " -noCB";
            if (Profiles.GetParameter("noLogs", false))
                launchParams += " -noLogs";
            if (Profiles.GetParameter("hyperthreading", false))
                launchParams += " -enableHT";
            if (Profiles.GetParameter("maxMemory", false))
            {
                switch (Profiles.GetParameter("maxMemoryValue", "0"))
                {
                    case "0":
                        launchParams += " -maxMem=768";
                        break;
                    case "1":
                        launchParams += " -maxMem=1024";
                        break;
                    case "2":
                        launchParams += " -maxMem=2048";
                        break;
                    case "3":
                        launchParams += " -maxMem=4096";
                        break;
                    case "4":
                        launchParams += " -maxMem=8192";
                        break;
                    default:
                        launchParams += " -maxMem=768";
                        break;
                }
            }
            if (Profiles.GetParameter("maxVMemory", false))
            {
                switch (Profiles.GetParameter("maxVMemoryValue", "0"))
                {
                    case "0":
                        launchParams += " -maxVRAM=128";
                        break;
                    case "1":
                        launchParams += " -maxVRAM=256";
                        break;
                    case "2":
                        launchParams += " -maxVRAM=512";
                        break;
                    case "3":
                        launchParams += " -maxVRAM=1024";
                        break;
                    case "4":
                        launchParams += " -maxVRAM=2048";
                        break;
                    case "5":
                        launchParams += " -maxVRAM=4096";
                        break;
                    case "6":
                        launchParams += " -maxVRAM=8192";
                        break;
                    default:
                        launchParams += " -maxVRAM=1024";
                        break;
                }
            }
            if (Profiles.GetParameter("cpuCount", false))
            {
                int value = Profiles.GetParameter("cpuCountValue", 0) + 1;
                launchParams += " -cpuCount=" + value.ToString();
            }
            if (Profiles.GetParameter("extraThreads", false))
            {
                switch (Profiles.GetParameter("extraThreadsValue", "0"))
                {
                    case "0":
                        launchParams += " -exThreads=0";
                        break;
                    case "1":
                        launchParams += " -exThreads=1";
                        break;
                    case "2":
                        launchParams += " -exThreads=3";
                        break;
                    case "3":
                        launchParams += " -exThreads=5";
                        break;
                    case "4":
                        launchParams += " -exThreads=7";
                        break;
                    default:
                        launchParams += " -exThreads=0";
                        break;
                }
            }
            if (Profiles.GetParameter("extraParameters", "").Length > 0)
            {
                launchParams += " " + Profiles.GetParameter("extraParameters", "");
            }

            //Addons
            string addonParams = "";
            if (Profiles.ProfileAddons.Count > 0)
            {
                foreach (KeyValuePair<string,string> addon in Profiles.ProfileAddons)
                {
                    if (bool.Parse(addon.Value))
                    {
                        addonParams += addon.Key + ";";
                    }
                }
            }
            if (addonParams.Length > 0)
            {
                launchParams += " -mod=" + addonParams;
            }

            //Headless client
            if (headless)
            {
                launchParams += " -client";
            }

            //Server connection
            if (multiplayer)
            {
                if (Profiles.ProfileServerInfo["server"].Length > 0)
                    launchParams += " -connect=" + Profiles.ProfileServerInfo["server"].Trim();
                if (Profiles.ProfileServerInfo["port"].Length > 0)
                    launchParams += " -port=" + Profiles.ProfileServerInfo["port"].Trim();
                if (Profiles.ProfileServerInfo["pass"].Length > 0)
                    launchParams += " -password=" + Profiles.ProfileServerInfo["pass"].Trim();
            }

            Process process = new Process();
            process.StartInfo.FileName = Settings.Arma3Path + "\\arma3.exe";
            if ((new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator))
            {
                process.StartInfo.Verb = "runas";
            }

            if (launchParams.Length > 0)
            {
                process.StartInfo.Arguments = launchParams;
            }

            //Start process
            if (startProcess)
            {
                process.Start();
            }

            //Set priority
            if (Profiles.GetParameter("priority", false) && startProcess)
            {
                switch (Profiles.GetParameter("priorityValue", 0))
                {
                    case 0:
                        process.PriorityClass = ProcessPriorityClass.Normal;
                        break;
                    case 1:
                        process.PriorityClass = ProcessPriorityClass.AboveNormal;
                        break;
                    case 2:
                        process.PriorityClass = ProcessPriorityClass.High;
                        break;
                    default:
                        process.PriorityClass = ProcessPriorityClass.Normal;
                        break;
                }
            }

            return process.StartInfo.FileName + " " + launchParams;
        }

        /// <summary>
        /// Update the lists of profiles
        /// </summary>
        private void UpdateProfiles()
        {
            comboBox_profiles.Items.Clear();
            listBox_profiles.Items.Clear();
            foreach (string profile in Profiles.UserProfiles)
            {
                comboBox_profiles.Items.Add(profile);
                listBox_profiles.Items.Add(profile);
            }
            comboBox_profiles.SelectedIndex = 0;
        }

        /// <summary>
        /// Update the parameters with the current profile
        /// </summary>
        private void UpdateForProfile()
        {
            //Set parameters values
            checkBox_noFilePatching.IsChecked = Profiles.GetParameter("noFilePatching", false);
            checkBox_skipSplash.IsChecked = Profiles.GetParameter("skipSplashScreen", false);
            checkBox_windowedMode.IsChecked = Profiles.GetParameter("windowsXPMode", false);
            checkBox_noPause.IsChecked = Profiles.GetParameter("noPause", false);
            checkBox_showScriptErrors.IsChecked = Profiles.GetParameter("showScriptErrors", false);
            checkBox_emptyWorld.IsChecked = Profiles.GetParameter("emptyWorld", false);
            checkBox_skipIntro.IsChecked = Profiles.GetParameter("skipIntro", false);
            checkBox_windowedMode.IsChecked = Profiles.GetParameter("windowedMode", false);
            checkBox_noMulticore.IsChecked = Profiles.GetParameter("noCB", false);
            checkBox_noLogs.IsChecked = Profiles.GetParameter("noLogs", false);
            checkBox_hyperthreading.IsChecked = Profiles.GetParameter("hyperthreading", false);

            checkBox_maxMemory.IsChecked = Profiles.GetParameter("maxMemory", false);
            comboBox_maxMemory.SelectedIndex = Profiles.GetParameter("maxMemoryValue", -1);
            checkBox_maxVMemory.IsChecked = Profiles.GetParameter("maxVMemory", false);
            comboBox_maxVMemory.SelectedIndex = Profiles.GetParameter("maxVMemoryValue", -1);
            checkBox_cpuCount.IsChecked = Profiles.GetParameter("cpuCount", false);
            comboBox_cpuCount.SelectedIndex = Profiles.GetParameter("cpuCountValue", -1);
            checkBox_priority.IsChecked = Profiles.GetParameter("priority", false);
            comboBox_priority.SelectedIndex = Profiles.GetParameter("priorityValue", -1);
            checkBox_extraThreads.IsChecked = Profiles.GetParameter("extraThreads", false);
            comboBox_extraThreads.SelectedIndex = Profiles.GetParameter("extraThreadsValue", -1);
            textBox_additionalParameters.Text = Profiles.GetParameter("extraParameters", "");

            //Clear addons list
            addons.Clear();

            //Add the addons and their status from the profile
            foreach (string addon in Profiles.ProfileAddons.Keys)
            {
                if (Addons.LocalAddons.Contains(addon)){
                    addons.Add(new Addon() { Name = addon, Enabled = bool.Parse(Profiles.ProfileAddons[addon]) });
                }
            }

            //Add missing addons not present in the profile
            foreach (string addon in Addons.LocalAddons)
            {
                if (!Profiles.ProfileAddons.ContainsKey(addon))
                {
                    addons.Add(new Addon() { Name = addon, Enabled = false });
                }
            }

            //Load server state
            textBox_serverAddress.Text = Profiles.ProfileServerInfo["server"];
            textBox_serverPort.Text = Profiles.ProfileServerInfo["port"];
            passwordBox_serverPassword.Password = Profiles.ProfileServerInfo["pass"];
        }

        /// <summary>
        /// Update the current profile values with the current parameters
        /// </summary>
        private void UpdateProfileValues()
        {
            //Save parameters
            Profiles.ProfileParameters["noFilePatching"] = checkBox_noFilePatching.IsChecked.ToString();
            Profiles.ProfileParameters["skipSplashScreen"] = checkBox_skipSplash.IsChecked.ToString();
            Profiles.ProfileParameters["windowsXPMode"] = checkBox_winXPmode.IsChecked.ToString();
            Profiles.ProfileParameters["noPause"] = checkBox_noPause.IsChecked.ToString();
            Profiles.ProfileParameters["showScriptErrors"] = checkBox_showScriptErrors.IsChecked.ToString();
            Profiles.ProfileParameters["emptyWorld"] = checkBox_emptyWorld.IsChecked.ToString();
            Profiles.ProfileParameters["skipIntro"] = checkBox_skipIntro.IsChecked.ToString();
            Profiles.ProfileParameters["windowedMode"] = checkBox_windowedMode.IsChecked.ToString();
            Profiles.ProfileParameters["noCB"] = checkBox_noMulticore.IsChecked.ToString();
            Profiles.ProfileParameters["noLogs"] = checkBox_noLogs.IsChecked.ToString();
            Profiles.ProfileParameters["hyperthreading"] = checkBox_hyperthreading.IsChecked.ToString();

            Profiles.ProfileParameters["maxMemory"] = checkBox_maxMemory.IsChecked.ToString();
            Profiles.ProfileParameters["maxMemoryValue"] = comboBox_maxMemory.SelectedIndex.ToString();
            Profiles.ProfileParameters["maxVMemory"] = checkBox_maxVMemory.IsChecked.ToString();
            Profiles.ProfileParameters["maxVMemoryValue"] = comboBox_maxVMemory.SelectedIndex.ToString();
            Profiles.ProfileParameters["cpuCount"] = checkBox_cpuCount.IsChecked.ToString();
            Profiles.ProfileParameters["cpuCountValue"] = comboBox_cpuCount.SelectedIndex.ToString();
            Profiles.ProfileParameters["priority"] = checkBox_priority.IsChecked.ToString();
            Profiles.ProfileParameters["priorityValue"] = comboBox_priority.SelectedIndex.ToString();
            Profiles.ProfileParameters["extraThreads"] = checkBox_extraThreads.IsChecked.ToString();
            Profiles.ProfileParameters["extraThreadsValue"] = comboBox_extraThreads.SelectedIndex.ToString();

            Profiles.ProfileParameters["extraParameters"] = textBox_additionalParameters.Text;

            //Save addons state
            Profiles.ProfileAddons.Clear();
            foreach (Addon addon in addons)
            {
                Profiles.ProfileAddons.Add(addon.Name, addon.Enabled.ToString());
            }

            //Save server state
            Profiles.ProfileServerInfo["server"] = textBox_serverAddress.Text;
            Profiles.ProfileServerInfo["port"] = textBox_serverPort.Text;
            Profiles.ProfileServerInfo["pass"] = passwordBox_serverPassword.Password;
        }

        /// <summary>
        /// Callback method for updating the display
        /// </summary>
        /// <param name="method">Method to execute</param>
        /// <param name="parameters">Method parameters</param>
        public static void UpdateForm(string method, object[] parameters)
        {
            if (Form != null)
            {
                if (Form.Dispatcher.CheckAccess())
                {
                    typeof(MainWindow).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Form, parameters);
                } else
                {
                    Form.Dispatcher.Invoke(new UpdateFormCallBack(UpdateForm), method, parameters);
                }
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

        /// <summary>
        /// Update the repository status with the given info
        /// </summary>
        /// <param name="revision">Repository revision</param>
        /// <param name="buildDate">Repository build date</param>
        /// <param name="updated">Is the local repository up to date?</param>
        private void UpdateRepositoryStatus(string revision, DateTime? buildDate, bool updated)
        {
            tile_repositoryStatus.IsEnabled = true;

            if (!revision.Equals(""))
            {
                label_repositoryRevision.Content = revision;
            }

            if (buildDate == DateTime.MinValue)
            {
                tile_repositoryStatus.Background = new SolidColorBrush(Colors.Orange);
                tile_repositoryStatus.Content = "Desconocido";
                tile_repositoryStatus.ToolTip = "No se ha podido comprobar el repositorio en el servidor. Click para volver a comprobar.";
                label_repositoryRevision.ToolTip = null;
            } else
            {
                label_repositoryRevision.Content += " (" + buildDate.Value.ToShortDateString() + ")";
                label_repositoryRevision.ToolTip = "Compilado el " + buildDate.Value.ToShortDateString() + " a las " + buildDate.Value.ToShortTimeString();
                if (updated)
                {
                    tile_repositoryStatus.Background = new SolidColorBrush(Colors.Green);
                    tile_repositoryStatus.Content = "Actualizado";
                    tile_repositoryStatus.ToolTip = "Tienes la última versión del repositorio. Click para volver a comprobar.";
                } else
                {
                    tile_repositoryStatus.Background = new SolidColorBrush(Colors.Red);
                    tile_repositoryStatus.Content = "No actualizado";
                    tile_repositoryStatus.ToolTip = "No tienes la última versión del repositorio. Click para volver a comprobar.";
                }
            }
            UpdateStatusBar();
        }

        /// <summary>
        /// Update the status for the server with the given index
        /// </summary>
        /// <param name="index">Index of the server</param>
        /// <param name="status">Is the server online or offline</param>
        private void UpdateServerStatus(int index, bool status)
        {
            Image[] serverStatusImages = new Image[3] { image_coopStatus, image_academyStatus, image_aliveStatus };

            if (status)
            {
                serverStatusImages[index].Source = new BitmapImage(new Uri("pack://application:,,,/Resources/on.png"));
                serverStatusImages[index].ToolTip = "Online. Click para volver a comprobar";
            } else
            {
                serverStatusImages[index].Source = new BitmapImage(new Uri("pack://application:,,,/Resources/off.png"));
                serverStatusImages[index].ToolTip = "Desconocido / Offline. Click para volver a comprobar";
            }
            UpdateStatusBar();
        }

        /// <summary>
        /// Update the information of the server with the given index
        /// </summary>
        /// <param name="index">Index of the server</param>
        /// <param name="exception">Has there been an exception when querying the server?</param>
        private void UpdateServerInfo(int index, bool exception)
        {

            textBox_serverName.Text = "";
            textBox_serverMission.Text = "";
            textBox_serverPing.Text = "";
            textBox_serverMap.Text = "";
            textBox_serverPlayers.Text = "";
            textBox_serverVersion.Text = "";
            listBox_serverPlayers.Items.Clear();
            textBox_serverMods.Text = "";

            textBox_serverName.Text = Servers.ServerInfo[0];
            textBox_serverMission.Text = Servers.ServerInfo[1];
            textBox_serverPing.Text = Servers.ServerInfo[2];
            textBox_serverMap.Text = Servers.ServerInfo[3];
            textBox_serverPlayers.Text = Servers.ServerInfo[4] + "/" + Servers.ServerInfo[5];
            textBox_serverVersion.Text = Servers.ServerInfo[6];
            foreach (string player in Servers.ServerPlayers)
            {
                listBox_serverPlayers.Items.Add(player);
            }
            textBox_serverMods.Text = Servers.ServerMods;

            button_checkStatus.IsEnabled = true;
            UpdateStatusBar();

            if (exception)
            {
                this.ShowMessageAsync("Error", "Error de comunicación con el servidor", MessageDialogStyle.Affirmative);
            }
        }

        /// <summary>
        /// Show a new update notification
        /// </summary>
        /// <param name="version">Latest version available</param>
        private async void ShowUpdateNotification(string version, bool newVersion)
        {
            UpdateStatusBar();
            if (newVersion)
            {
                MessageDialogResult result = await this.ShowMessageAsync("Actualización disponible", "Hay una nueva versión disponible para la aplicación (" + version + "), ¿deseas actualizar ahora?", MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Affirmative)
                {
                    Updater.ExecuteUpdater();
                }
            } else
            {
                if (version == null)
                {
                    await this.ShowMessageAsync("Error de conexión", "Se ha producido un error de conexión al comprobar actualizaciones", MessageDialogStyle.Affirmative);
                }
                else
                {
                    await this.ShowMessageAsync("No hay actualizaciones disponibles", "Dispones de la última versión", MessageDialogStyle.Affirmative);
                }
            }
        }
    }
}
