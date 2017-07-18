using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using _11thLauncher.Configuration;

namespace _11thLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        internal static MainWindow Form;

        private delegate void UpdateFormCallBack(string method, object[] parameters);
        //internal ObservableCollection<Addon> Addons = new ObservableCollection<Addon>();

        public MainWindow()
        {

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Add profiles
            UpdateProfiles();
            //comboBox_profiles.SelectedIndex = comboBox_profiles.Items.IndexOf(Profiles.DefaultProfile); //Select default profile

            //Set accent
            //AccentColor a = ((AccentColor) Settings.Accent);
            //ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(((AccentColor)Settings.Accent).ToString()), ThemeManager.GetAppTheme("BaseLight"));

            //Set groupboxes preferences
            //groupBox_servers.IsExpanded = Settings.ServersGroupBox;
            //groupBox_repository.IsExpanded = Settings.RepositoryGroupBox;

            //If just updated, remove updater and show notification
            //if (Updater.Updated)
            //{
                //Updater.RemoveUpdater();
                //this.ShowMessageAsync("Aplicación actualizada", "La aplicación ha sido actualizada con éxito");
            //}

            ////Notify if update failed
            //if (Updater.UpdateFailed)
            //{
                //Updater.RemoveUpdater();
                //this.ShowMessageAsync("Error al actualizar", "Se ha producido un error al actualizar la aplicación, vuelve a intentarlo más tarde");
            //}

            //Check Java presence
            //Repository.CheckJava();

            //Check Arma3Sync configuration
            //if ((Repository.JavaVersion != "" || Settings.JavaPath != "") && Settings.Arma3SyncPath != "" && Settings.Arma3SyncRepository != "")
            //{
                //image_arma3Sync.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/a3sEnabled.png"));
                //image_arma3Sync.ToolTip = "Arma3Sync está configurado. Click para iniciar";
                //tile_repositoryStatus.IsEnabled = true;
                //tile_repositoryStatus.Background = new SolidColorBrush(Colors.Orange);
                //tile_repositoryStatus.ToolTip = "Click para comprobar estado";
            //}

            //Check repository for updates
            //if (Settings.CheckRepository)
            //{
                //tile_repositoryStatus.IsEnabled = false;
                //new Thread(Repository.CheckRepository).Start();
            //}

            //Check servers status
            //if (Settings.CheckServers)
            //{
                //new Thread(Servers.CheckServers).Start();
            //}

            //Check for updates
            //if (Settings.CheckUpdates)
            //{
                //new Thread(() => Updater.CheckVersion(false)).Start();
            //}

            //Check processes
            //var steamProcess = Util.GetSteamProcess();
            //if (steamProcess.Running)
            //{
                //label_steamProcess.Content = steamProcess.Name;
                ////TODO...
            //}
            //var ts3Process = Util.GetTeamspeakProcess();


            //Check local game version against server version
            //label_gameVersion.Content = Settings.GetGameVersion();
            //new Thread(Servers.CompareServerVersion).Start();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Form = null;
        }

        private void comboBox_profiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (comboBox_profiles.SelectedIndex == -1) return;

            //Clear addon presets comboBox selection
            //comboBox_addons.SelectedIndex = -1;

            //Read selected profile
            //Profiles.ReadProfile(comboBox_profiles.SelectedItem.ToString());

            //Update the display with the profile
            //UpdateForProfile();
        }

        private void button_launch_Click(object sender, RoutedEventArgs e)
        {
            //var multiplayer = false;
            //var headless = false;
            //switch (button_launch.SelectedIndex)
            //{
            //    case 0:
            //    default:
            //        break;
            //    case 1:
            //        multiplayer = true;
            //        break;
            //    case 2:
            //        headless = true;
            //        break;
            //}

            //StartArmA3(multiplayer, true, headless);

            //if (Settings.StartClose)
            //{
            //    Application.Current.Shutdown();
            //}

            //if (Settings.StartMinimize)
            //{
            //    WindowState = WindowState.Minimized;
            //}
        }

        private void button_clip_Click(object sender, RoutedEventArgs e)
        {
            //var multiplayer = false;
            //var headless = false;
            //switch (button_launch.SelectedIndex)
            //{
            //    case 0:
            //    default:
            //        break;
            //    case 1:
            //        multiplayer = true;
            //        break;
            //    case 2:
            //        headless = true;
            //        break;
            //}
    
            //Clipboard.SetText(StartArmA3(multiplayer, false, headless));
        }

        private void image_arma3Sync_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (Repository.JavaVersion != "" && Settings.Arma3SyncPath != "")
            //{
                //Repository.StartArmA3Sync();
            //}
        }

        private void tile_repositoryStatus_Click(object sender, RoutedEventArgs e)
        {
            //label_repositoryRevision.Content = "";
            //tile_repositoryStatus.IsEnabled = false;
            //tile_repositoryStatus.ToolTip = "Comprobando estado";
            //new Thread(Repository.CheckRepository).Start();
        }

        private void button_defaultProfile_Click(object sender, RoutedEventArgs e)
        {
            //if (listBox_profiles.SelectedIndex == -1)
            //{
                //this.ShowMessageAsync("Error de selección", "No has seleccionado ningún perfil");
            //} else
            //{
                //string selectedProfile = listBox_profiles.SelectedItem.ToString();
                //if (selectedProfile.StartsWith("♥")) selectedProfile = selectedProfile.Remove(0, 2);

                //Profiles.DefaultProfile = selectedProfile;
                //Settings.Write();

                ////Refill profile list
                //listBox_profiles.Items.Clear();
                //foreach (string profile in Profiles.UserProfiles)
                //{
                    //if (profile.Equals(Profiles.DefaultProfile))
                    //{
                        //int index = listBox_profiles.Items.Add("♥ " + profile);
                        //listBox_profiles.SelectedItem = index;
                    //}
                    //else
                    //{
                        //listBox_profiles.Items.Add(profile);
                    //}
                //}
            //}
        }

        private void button_createProfile_Click(object sender, RoutedEventArgs e)
        {
            //textBox_newProfile.IsEnabled = true;
            //textBox_newProfile.Text = "Nuevo perfil";
            //button_saveNewProfile.IsEnabled = true;
            //textBox_newProfile.Select(0, textBox_newProfile.Text.Length);
            //textBox_newProfile.Focus();
        }

        private void button_deleteProfile_Click(object sender, RoutedEventArgs e)
        {
            //if (listBox_profiles.SelectedIndex == -1)
            //{
                //this.ShowMessageAsync("Error de borrado", "No has seleccionado ningún perfil");
            //}
            //else
            //{
                ////Check profile name to remove default profile marker
                //string selectedProfile = listBox_profiles.SelectedItem.ToString();
                //if (selectedProfile.StartsWith("♥")) selectedProfile = selectedProfile.Remove(0, 2);

                //if (selectedProfile.Equals("Predeterminado"))
                //{
                    //this.ShowMessageAsync("Error de borrado", "No puedes borrar el perfil predeterminado");
                //}
                //else
                //{
                    ////Check if deleted profile is default
                    //if (selectedProfile.Equals(Profiles.DefaultProfile))
                    //{
                        //Profiles.DefaultProfile = "Predeterminado";
                    //}
                    //Profiles.DeleteProfile(selectedProfile);
                    //UpdateProfiles();
                    ////comboBox_profiles.SelectedIndex = 0;
                //}
            //}
        }

        private void button_saveNewProfile_Click(object sender, RoutedEventArgs e)
        {
            ////Clean profile name string
            //string profileName = textBox_newProfile.Text.Replace("♥", "").Trim();

            ////Check that name is not repeated
            //if (Profiles.UserProfiles.Contains(profileName))
            //{
                //this.ShowMessageAsync("Error de creación de perfil", "Ya existe un perfil con el nombre indicado");
            //}
            //else
            //{
                //Profiles.UserProfiles.Add(profileName);
                //Settings.Write();
                //Profiles.WriteProfile(profileName);
                //UpdateProfiles();
                //textBox_newProfile.Text = "";
                //textBox_newProfile.IsEnabled = false;
                //button_saveNewProfile.IsEnabled = false;
            //}
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
            SaveProfile();

            string launchParams = "";

            //Parameters
            if (Profiles.GetParameter("noFilePatching", false))
            {
                launchParams += " -filePatching";
            }
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
                launchParams += " -cpuCount=" + value;
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

            if (Profiles.GetParameter("memoryAllocator", false))
            {
                int allocatorIndex = Profiles.GetParameter("memoryAllocatorValue", -1);
                if (allocatorIndex > Settings.Allocators.Count)
                {
                    allocatorIndex = 0;
                }
                launchParams += " -malloc=" + comboBox_malloc.Items[allocatorIndex];
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

            Process process = new Process {StartInfo = {FileName = Settings.Arma3Path + "\\arma3.exe"}};
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
                if (!(string.IsNullOrEmpty(Settings.Arma3Path)))
                {
                    process.Start();

                    //Set priority
                    if (Profiles.GetParameter("priority", false))
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
                }
                else
                {
                    this.ShowMessageAsync("Error de lanzamiento", "La ruta del ejecutable de ArmA 3 no está configurada");
                }
            }

            return process.StartInfo.FileName + " " + launchParams;
        }

        /// <summary>
        /// Update the lists of profiles
        /// </summary>
        private void UpdateProfiles()
        {
            //comboBox_profiles.Items.Clear();
            //listBox_profiles.Items.Clear();
            //foreach (string profile in Profiles.UserProfiles)
            //{
            //    comboBox_profiles.Items.Add(profile);
            //    if (profile.Equals(Profiles.DefaultProfile))
            //    {
            //        listBox_profiles.Items.Add("★ " + profile);
            //    }
            //    else
            //    {
            //        listBox_profiles.Items.Add(profile);
            //    }
            //}
            //comboBox_profiles.SelectedIndex = 0;
        }

        /// <summary>
        /// Update the parameters with the current profile
        /// </summary>
        private void UpdateForProfile()
        {
            //Set parameters values
            //button_launch.SelectedIndex = Profiles.GetParameter("launchOption", 0);

            //checkBox_noFilePatching.IsChecked = Profiles.GetParameter("noFilePatching", false);
            //checkBox_skipSplash.IsChecked = Profiles.GetParameter("skipSplashScreen", false);
            //checkBox_windowedMode.IsChecked = Profiles.GetParameter("windowsXPMode", false);
            //checkBox_noPause.IsChecked = Profiles.GetParameter("noPause", false);
            //checkBox_showScriptErrors.IsChecked = Profiles.GetParameter("showScriptErrors", false);
            //checkBox_emptyWorld.IsChecked = Profiles.GetParameter("emptyWorld", false);
            //checkBox_skipIntro.IsChecked = Profiles.GetParameter("skipIntro", false);
            //checkBox_windowedMode.IsChecked = Profiles.GetParameter("windowedMode", false);
            //checkBox_noMulticore.IsChecked = Profiles.GetParameter("noCB", false);
            //checkBox_noLogs.IsChecked = Profiles.GetParameter("noLogs", false);
            //checkBox_hyperthreading.IsChecked = Profiles.GetParameter("hyperthreading", false);

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

            //Check number of allocators when loading in case they were changed
            checkBox_malloc.IsChecked = Profiles.GetParameter("memoryAllocator", false);
            int allocatorIndex = Profiles.GetParameter("memoryAllocatorValue", -1);
            if (allocatorIndex > Settings.Allocators.Count)
            {
                allocatorIndex = 0;
            }
            comboBox_malloc.SelectedIndex = allocatorIndex;

            textBox_additionalParameters.Text = Profiles.GetParameter("extraParameters", "");

            //Clear addons list
            //Addons.Clear();

            //Add the addons and their status from the profile
            //foreach (string addon in Profiles.ProfileAddons.Keys)
            //{
            //    if (Configuration.Addons.LocalAddons.Contains(addon)){
            //        Addons.Add(new Addon() { Name = addon, Enabled = bool.Parse(Profiles.ProfileAddons[addon]) });
            //    }
            //}

            //Add missing addons not present in the profile
            //foreach (string addon in Configuration.Addons.LocalAddons)
            //{
            //    if (!Profiles.ProfileAddons.ContainsKey(addon))
            //    {
            //        Addons.Add(new Addon { Name = addon, Enabled = false });
            //    }
            //}

            //Load server state
            //textBox_serverAddress.Text = Profiles.ProfileServerInfo["server"];
            //textBox_serverPort.Text = Profiles.ProfileServerInfo["port"];
            //passwordBox_serverPassword.Password = Profiles.ProfileServerInfo["pass"];
        }

        /// <summary>
        /// Update the current profile values with the current parameters
        /// </summary>
        private void SaveProfile()
        {
            //Save parameters
            //Profiles.ProfileParameters["launchOption"] = button_launch.SelectedIndex.ToString();

            //Profiles.ProfileParameters["noFilePatching"] = checkBox_noFilePatching.IsChecked.ToString();
            //Profiles.ProfileParameters["skipSplashScreen"] = checkBox_skipSplash.IsChecked.ToString();
            //Profiles.ProfileParameters["windowsXPMode"] = checkBox_winXPmode.IsChecked.ToString();
            //Profiles.ProfileParameters["noPause"] = checkBox_noPause.IsChecked.ToString();
            //Profiles.ProfileParameters["showScriptErrors"] = checkBox_showScriptErrors.IsChecked.ToString();
            //Profiles.ProfileParameters["emptyWorld"] = checkBox_emptyWorld.IsChecked.ToString();
            //Profiles.ProfileParameters["skipIntro"] = checkBox_skipIntro.IsChecked.ToString();
            //Profiles.ProfileParameters["windowedMode"] = checkBox_windowedMode.IsChecked.ToString();
            //Profiles.ProfileParameters["noCB"] = checkBox_noMulticore.IsChecked.ToString();
            //Profiles.ProfileParameters["noLogs"] = checkBox_noLogs.IsChecked.ToString();
            //Profiles.ProfileParameters["hyperthreading"] = checkBox_hyperthreading.IsChecked.ToString();

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
            Profiles.ProfileParameters["memoryAllocator"] = checkBox_malloc.IsChecked.ToString();
            Profiles.ProfileParameters["memoryAllocatorValue"] = comboBox_malloc.SelectedIndex.ToString();

            Profiles.ProfileParameters["extraParameters"] = textBox_additionalParameters.Text;

            //Save addons state
            Profiles.ProfileAddons.Clear();
            //foreach (Addon addon in Addons)
            //{
            //    Profiles.ProfileAddons.Add(addon.Name, addon.Enabled.ToString());
            //}

            //Save server state
            //Profiles.ProfileServerInfo["server"] = textBox_serverAddress.Text;
            //Profiles.ProfileServerInfo["port"] = textBox_serverPort.Text;
            //Profiles.ProfileServerInfo["pass"] = passwordBox_serverPassword.Password;

            //Profiles.WriteProfile(comboBox_profiles.SelectedItem.ToString());
        }

        /// <summary>
        /// Show a new update notification
        /// </summary>
        /// <param name="version">Latest version available</param>
        /// <param name="newVersion">Is there a new version?</param>
        private async void ShowUpdateNotification(string version, bool newVersion)
        {
            //UpdateStatusBar();
            if (newVersion)
            {
                MessageDialogResult result = await this.ShowMessageAsync("Actualización disponible", "Hay una nueva versión disponible para la aplicación (" + version + "), ¿deseas actualizar ahora?", MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Affirmative)
                {
                    //Updater.ExecuteUpdater();
                }
            } else
            {
                if (version == null)
                {
                    await this.ShowMessageAsync("Error de conexión", "Se ha producido un error de conexión al comprobar actualizaciones");
                }
                else
                {
                    await this.ShowMessageAsync("No hay actualizaciones disponibles", "Dispones de la última versión");
                }
            }
        }
    }
}
