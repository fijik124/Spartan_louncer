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

namespace _11thLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
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

        /// <summary>
        /// Use the current parameters and configurations to start the game or copy the start line to the clipboard
        /// </summary>
        /// <param name="multiplayer">Join the multiplayer server?</param>
        /// <param name="startProcess">Start the game or just copy the line to clipboard?</param>
        /// <param name="headless">Start headless client?</param>
        private string StartArmA3(bool multiplayer, bool startProcess, bool headless)
        {
            ////Save the current parameters to profile
            ////SaveProfile();

            //string launchParams = "";

            ////Parameters
            //if (Profiles.GetParameter("noFilePatching", false))
            //{
                //launchParams += " -filePatching";
            //}
            //if (Profiles.GetParameter("skipSplashScreen", false))
                //launchParams += " -noSplash";
            //if (Profiles.GetParameter("windowsXPMode", false))
                //launchParams += " -winxp";
            //if (Profiles.GetParameter("noPause", false))
                //launchParams += " -noPause";
            //if (Profiles.GetParameter("showScriptErrors", false))
                //launchParams += " -showScriptErrors";
            //if (Profiles.GetParameter("emptyWorld", false))
                //launchParams += " -world=empty";
            //if (Profiles.GetParameter("skipIntro", false))
                //launchParams += " -skipIntro";
            //if (Profiles.GetParameter("windowedMode", false))
                //launchParams += " -window";
            //if (Profiles.GetParameter("noCB", false))
                //launchParams += " -noCB";
            //if (Profiles.GetParameter("noLogs", false))
                //launchParams += " -noLogs";
            //if (Profiles.GetParameter("hyperthreading", false))
                //launchParams += " -enableHT";
            //if (Profiles.GetParameter("maxMemory", false))
            //{
                //switch (Profiles.GetParameter("maxMemoryValue", "0"))
                //{
                    //case "0":
                        //launchParams += " -maxMem=768";
                        //break;
                    //case "1":
                        //launchParams += " -maxMem=1024";
                        //break;
                    //case "2":
                        //launchParams += " -maxMem=2048";
                        //break;
                    //case "3":
                        //launchParams += " -maxMem=4096";
                        //break;
                    //case "4":
                        //launchParams += " -maxMem=8192";
                        //break;
                    //default:
                        //launchParams += " -maxMem=768";
                        //break;
                //}
            //}
            //if (Profiles.GetParameter("maxVMemory", false))
            //{
                //switch (Profiles.GetParameter("maxVMemoryValue", "0"))
                //{
                    //case "0":
                        //launchParams += " -maxVRAM=128";
                        //break;
                    //case "1":
                        //launchParams += " -maxVRAM=256";
                        //break;
                    //case "2":
                        //launchParams += " -maxVRAM=512";
                        //break;
                    //case "3":
                        //launchParams += " -maxVRAM=1024";
                        //break;
                    //case "4":
                        //launchParams += " -maxVRAM=2048";
                        //break;
                    //case "5":
                        //launchParams += " -maxVRAM=4096";
                        //break;
                    //case "6":
                        //launchParams += " -maxVRAM=8192";
                        //break;
                    //default:
                        //launchParams += " -maxVRAM=1024";
                        //break;
                //}
            //}
            //if (Profiles.GetParameter("cpuCount", false))
            //{
                //int value = Profiles.GetParameter("cpuCountValue", 0) + 1;
                //launchParams += " -cpuCount=" + value;
            //}
            //if (Profiles.GetParameter("extraThreads", false))
            //{
                //switch (Profiles.GetParameter("extraThreadsValue", "0"))
                //{
                    //case "0":
                        //launchParams += " -exThreads=0";
                        //break;
                    //case "1":
                        //launchParams += " -exThreads=1";
                        //break;
                    //case "2":
                        //launchParams += " -exThreads=3";
                        //break;
                    //case "3":
                        //launchParams += " -exThreads=5";
                        //break;
                    //case "4":
                        //launchParams += " -exThreads=7";
                        //break;
                    //default:
                        //launchParams += " -exThreads=0";
                        //break;
                //}
            //}
            //if (Profiles.GetParameter("extraParameters", "").Length > 0)
            //{
                //launchParams += " " + Profiles.GetParameter("extraParameters", "");
            //}

            //if (Profiles.GetParameter("memoryAllocator", false))
            //{
                //int allocatorIndex = Profiles.GetParameter("memoryAllocatorValue", -1);
                ////if (allocatorIndex > Settings.Allocators.Count)
                ////{
                ////    allocatorIndex = 0;
                ////}
                //launchParams += " -malloc=" + comboBox_malloc.Items[allocatorIndex];
            //}

            ////Addons
            //string addonParams = "";
            //if (Profiles.ProfileAddons.Count > 0)
            //{
                //foreach (KeyValuePair<string,string> addon in Profiles.ProfileAddons)
                //{
                    //if (bool.Parse(addon.Value))
                    //{
                        //addonParams += addon.Key + ";";
                    //}
                //}
            //}
            //if (addonParams.Length > 0)
            //{
                //launchParams += " -mod=" + addonParams;
            //}

            ////Headless client
            //if (headless)
            //{
                //launchParams += " -client";
            //}

            ////Server connection
            //if (multiplayer)
            //{
                //if (Profiles.ProfileServerInfo["server"].Length > 0)
                    //launchParams += " -connect=" + Profiles.ProfileServerInfo["server"].Trim();
                //if (Profiles.ProfileServerInfo["port"].Length > 0)
                    //launchParams += " -port=" + Profiles.ProfileServerInfo["port"].Trim();
                //if (Profiles.ProfileServerInfo["pass"].Length > 0)
                    //launchParams += " -password=" + Profiles.ProfileServerInfo["pass"].Trim();
            //}

            //Process process = new Process {StartInfo = {FileName = Settings.Arma3Path + "\\arma3.exe"}};
            //if ((new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator))
            //{
                //process.StartInfo.Verb = "runas";
            //}

            //if (launchParams.Length > 0)
            //{
                //process.StartInfo.Arguments = launchParams;
            //}

            ////Start process
            //if (startProcess)
            //{
                //if (!(string.IsNullOrEmpty(Settings.Arma3Path)))
                //{
                    //process.Start();

                    ////Set priority
                    //if (Profiles.GetParameter("priority", false))
                    //{
                        //switch (Profiles.GetParameter("priorityValue", 0))
                        //{
                            //case 0:
                                //process.PriorityClass = ProcessPriorityClass.Normal;
                                //break;
                            //case 1:
                                //process.PriorityClass = ProcessPriorityClass.AboveNormal;
                                //break;
                            //case 2:
                                //process.PriorityClass = ProcessPriorityClass.High;
                                //break;
                            //default:
                                //process.PriorityClass = ProcessPriorityClass.Normal;
                                //break;
                        //}
                    //}
                //}
                //else
                //{
                    //this.ShowMessageAsync("Error de lanzamiento", "La ruta del ejecutable de ArmA 3 no está configurada");
                //}
            //}

            //return process.StartInfo.FileName + " " + launchParams;
            return null;
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
