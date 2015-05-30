using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;

using _11thLauncher.LogViewer;

namespace _11thLauncher
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        public Form1()
        {
            InitializeComponent();
            this.ActiveControl = this.picture_logo;
            Program.form = this;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.form = null;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //Check privileges
            if (Util.hasPrivileges())
            {
                this.metroStyleManager1.Style = MetroFramework.MetroColorStyle.Green;
                this.tile_privileges.Text = "Administrador";
                this.metroToolTip1.SetToolTip(this.tile_privileges, "      La aplicación se está ejecutando con permisos de administrador      ");
            }
            else
            {
                this.metroStyleManager1.Style = MetroFramework.MetroColorStyle.Red;
                this.tile_privileges.Text = "Usuario";
                this.metroToolTip1.SetToolTip(this.tile_privileges, "      La aplicación se está ejecutando sin permisos de administrador      ");
            }

            //If no configuration saved, create default configuration
            if (!Util.configExists())
            {
                Util.readPath();
                if (Util.arma3Path.Equals(""))
                {
                    MetroFramework.MetroMessageBox.Show(this, "La ruta de ejecución no ha sido autodetectada, indicala manualmente en la pestaña opciones antes de empezar a utilizar la aplicación", "Ruta de ejecución", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                //Read addons from paths
                Util.readAddons(this.checkedListBox_arma3Addons);

                //Set default values
                this.radioButton_minimizeTaskbar.Checked = true;
                this.radioButton_startNothing.Checked = true;
                this.updateValues();

                //Add and select the default profile
                Util.profiles.Add("Predeterminado");
                Util.writeProfile("Predeterminado");
                Util.defaultProfile = "Predeterminado";
                this.updateDisplay();

                //Save configuration
                Util.writeParameters();
            }
            //Configuration exists, read from xml
            else
            {
                Util.readParameters();

                this.radioButton_minimizeTaskbar.Checked = true;
                this.radioButton_startNothing.Checked = true;

                //Read addons from paths
                Util.readAddons(this.checkedListBox_arma3Addons);
                this.updateDisplay();
                this.sortAddons();
            }

            //Set saved selected profile
            this.combo_profiles.SelectedIndex = this.combo_profiles.Items.IndexOf(Util.defaultProfile);
            this.Refresh();

            //Initialize log viewer
            Logger.init();

            //Check for updates
            new Thread(new ThreadStart(Version.checkVersion)).Start();;

            //Check servers status
            new Thread(new ThreadStart(Net.checkServers)).Start();
        }

        private void updateDisplay()
        {
            //Update path
            this.textBox_arma3Path.Text = Util.arma3Path;

            //Update profiles
            this.combo_profiles.Items.Clear();
            this.combo_profiles.Items.AddRange(Util.profiles.ToArray());
            this.combo_profiles.SelectedIndex = 0;
            this.listBox_profiles.Items.Clear();
            this.listBox_profiles.Items.AddRange(Util.profiles.ToArray());

            //Update configuration
            this.radioButton_minimizeNotificationBar.Checked = Util.minimizeNotification;
            this.radioButton_startClose.Checked = Util.startClose;
            this.radioButton_startMinimize.Checked = Util.startMinimize;
        }

        private void updateValues()
        {
            //Save parameters state
            Util.parameters = new Dictionary<String, String>();
            Util.parameters.Add("noFilePatching", check_noPatching.Checked.ToString());
            Util.parameters.Add("skipSplashScreen", check_skipSplash.Checked.ToString());
            Util.parameters.Add("windowsXPMode", check_winXP.Checked.ToString());
            Util.parameters.Add("noPause", check_noPause.Checked.ToString());
            Util.parameters.Add("showScriptErrors", check_showErrors.Checked.ToString());
            Util.parameters.Add("emptyWorld", check_emptyWorld.Checked.ToString());
            Util.parameters.Add("skipIntro", check_skipIntro.Checked.ToString());
            Util.parameters.Add("windowedMode", check_windowMode.Checked.ToString());
            Util.parameters.Add("noCB", check_noCB.Checked.ToString());
            Util.parameters.Add("noLogs", check_noLogs.Checked.ToString());
            Util.parameters.Add("hyperthreading", check_HT.Checked.ToString());
            Util.parameters.Add("maxMemory", check_maxMemory.Checked.ToString());
            Util.parameters.Add("maxMemoryValue", combo_maxMemory.SelectedIndex.ToString());
            Util.parameters.Add("maxVMemory", check_videoMemory.Checked.ToString());
            Util.parameters.Add("maxVMemoryValue", combo_maxVMemory.SelectedIndex.ToString());
            Util.parameters.Add("cpuCount", check_CPUCount.Checked.ToString());
            Util.parameters.Add("cpuCountValue", combo_CPUCount.SelectedIndex.ToString());
            Util.parameters.Add("priority", check_priority.Checked.ToString());
            Util.parameters.Add("priorityValue", combo_priority.SelectedIndex.ToString());
            Util.parameters.Add("extraThreads", check_exThreads.Checked.ToString());
            Util.parameters.Add("extraThreadsValue", combo_threads.SelectedIndex.ToString());
            Util.parameters.Add("extraParameters", textBox_parameters.Text);

            //Save addons state
            Util.arma3AddonsList.Clear();
            for (int i = 0; i < checkedListBox_arma3Addons.Items.Count; i++)
            {
                Util.arma3AddonsList.Add(checkedListBox_arma3Addons.Items[i].ToString());
                Util.arma3Addons[checkedListBox_arma3Addons.Items[i].ToString()] = checkedListBox_arma3Addons.GetItemChecked(i).ToString();
            }

            //Save server state
            Util.arma3ServerInfo = new Dictionary<String, String>();
            Util.arma3ServerInfo.Add("server", textBox_arma3server.Text);
            Util.arma3ServerInfo.Add("port", textBox_arma3port.Text);
            Util.arma3ServerInfo.Add("pass", textBox_arma3password.Text);

            //Save configuration
            Util.startClose = this.radioButton_startClose.Checked;
            Util.startMinimize = this.radioButton_startMinimize.Checked;
            Util.minimizeNotification = this.radioButton_minimizeNotificationBar.Checked;
        }

        //Server status
        private delegate void UpdateStatusCallback(int index);
        public void updateStatus(int index)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateStatusCallback(updateStatus), index);
            }
            else
            {
                PictureBox[] pictureBoxArray = new PictureBox[3] { image_coopA3, image_academiaA3, image_aliveA3 };
                if (Net.serversStatus[index])
                {
                    pictureBoxArray[index].Image = global::_11thLauncher.Properties.Resources.on;
                    this.metroToolTip1.SetToolTip(pictureBoxArray[index], "Online");
                }
                else
                {
                    pictureBoxArray[index].Image = global::_11thLauncher.Properties.Resources.off;
                    this.metroToolTip1.SetToolTip(pictureBoxArray[index], "Desconocido / Offline");
                }
            }
        }

        //Server info
        private delegate void ShowInfoCallback(int index);
        public void showInfo(int index)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ShowInfoCallback(showInfo),index);
            }
            else
            {
                this.textBox_serverName.Text = "";
                this.textBox_serverDesc.Text = "";
                this.textBox_serverPing.Text = "";
                this.textBox_serverMap.Text = "";
                this.textBox_serverPlayers.Text = "";
                this.textBox_serverVersion.Text = "";
                this.listBox_serverPlayers.Items.Clear();
                this.textBox_serverMods.Text = "";
                this.textBox_serverName.Text = Net.serverInfo[0];
                this.textBox_serverDesc.Text = Net.serverInfo[1];
                this.textBox_serverPing.Text = Net.serverInfo[2];
                this.textBox_serverMap.Text = Net.serverInfo[3];
                this.textBox_serverPlayers.Text = Net.serverInfo[4] + "/" + Net.serverInfo[5];
                this.textBox_serverVersion.Text = Net.serverInfo[6];
                foreach (object obj in Net.serverPlayers)
                    this.listBox_serverPlayers.Items.Add(obj);
                this.textBox_serverMods.Text = Net.serverMods;
                if (Net.queryException)
                    this.picture_serverQuery.Image = global::_11thLauncher.Properties.Resources.error;
                else
                    this.picture_serverQuery.Image = global::_11thLauncher.Properties.Resources.info;
            }
        }

        //Update notification
        private delegate void ShowUpdateNotificationCallback();
        public void showUpdateNotification()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ShowUpdateNotificationCallback(showUpdateNotification));
            }
            else
            {
                String downloadURL = Version.getLatestDownload();
                DialogResult result = MetroFramework.MetroMessageBox.Show(this, "Hay una nueva versión disponible para la aplicación (" + Version.latestVersion  + "), ¿quieres descargarla?", "Actualización disponible", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    Process.Start(downloadURL);
                }
            }
        }


        //-----------------------------------------------
        //Form controls listeners
        //-----------------------------------------------

        //Parameters
        private void check_maxMemory_CheckedChanged(object sender, EventArgs e)
        {
            if (check_maxMemory.Checked)
            {
                combo_maxMemory.Enabled = true;
                combo_maxMemory.SelectedIndex = 0;
            }
            else
            {
                combo_maxMemory.Enabled = false;
                combo_maxMemory.SelectedIndex = -1;
            }
        }
        private void check_videoMemory_CheckedChanged(object sender, EventArgs e)
        {
            if (check_videoMemory.Checked)
            {
                combo_maxVMemory.Enabled = true;
                combo_maxVMemory.SelectedIndex = 0;
            }
            else
            {
                combo_maxVMemory.Enabled = false;
                combo_maxVMemory.SelectedIndex = -1;
            }
        }
        private void check_CPUCount_CheckedChanged(object sender, EventArgs e)
        {
            if (check_CPUCount.Checked)
            {
                combo_CPUCount.Enabled = true;
                combo_CPUCount.SelectedIndex = 0;
            }
            else
            {
                combo_CPUCount.Enabled = false;
                combo_CPUCount.SelectedIndex = -1;
            }
        }
        private void check_priority_CheckedChanged(object sender, EventArgs e)
        {
            if (check_priority.Checked)
            {
                combo_priority.Enabled = true;
                combo_priority.SelectedIndex = 0;
            }
            else
            {
                combo_priority.Enabled = false;
                combo_priority.SelectedIndex = -1;
            }
        }
        private void check_exThreads_CheckedChanged(object sender, EventArgs e)
        {
            if (check_exThreads.Checked)
            {
                combo_threads.Enabled = true;
                combo_threads.SelectedIndex = 0;
            }
            else
            {
                combo_threads.Enabled = false;
                combo_threads.SelectedIndex = -1;
            }
        }

        //ArmA 3 addons list
        bool arma3addons_allowCheck { get; set; }
        private void combo_arma3addons_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<String> normal = new List<String>(new String[] { "@cba", "@ace", "@tfar", "@meu", "@meu_maps", "@fx", "@jsrs" });
            List<String> alive = new List<String>(new String[] { "@cba", "@ace", "@tfar", "@meu", "@meu_maps", "@fx", "@jsrs", "@alive" });

            arma3addons_allowCheck = true;

            //Clear addons
            checkedListBox_arma3Addons.Items.Clear();

            switch (combo_arma3addons.SelectedIndex)
            {
                //Guerra moderna
                case 0:
                    foreach (String addon in normal)
                    {
                        if (Util.arma3AddonsList.Contains(addon))
                        {
                            int index = checkedListBox_arma3Addons.Items.Add(addon);
                            checkedListBox_arma3Addons.SetItemChecked(index, true);
                        }
                    }
                    break;
                //ALiVE
                case 1:
                    foreach (String addon in alive)
                    {
                        if (Util.arma3AddonsList.Contains(addon))
                        {
                            int index = checkedListBox_arma3Addons.Items.Add(addon);
                            checkedListBox_arma3Addons.SetItemChecked(index, true);
                        }
                    }
                    break;
                default:
                    break;
            }

            //Add addons not included in selected profile
            foreach (String addon in Util.arma3AddonsList)
            {
                if (!checkedListBox_arma3Addons.Items.Contains(addon))
                {
                    checkedListBox_arma3Addons.Items.Add(addon);
                }
            }

            arma3addons_allowCheck = false;
        }
        private void checkedListBox_arma3Addons_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!arma3addons_allowCheck)
                e.NewValue = e.CurrentValue; //check state change was not through authorized actions
        }
        private void checkedListBox_arma3Addons_MouseDown(object sender, MouseEventArgs e)
        {
            Boolean arma3addons_checking = false;

            //Checkbox detection
            Point loc = this.checkedListBox_arma3Addons.PointToClient(Cursor.Position);
            for (int i = 0; i < this.checkedListBox_arma3Addons.Items.Count; i++)
            {
                Rectangle rec = this.checkedListBox_arma3Addons.GetItemRectangle(i);
                rec.Width = 16; //checkbox itself has a default width of about 16 pixels

                if (rec.Contains(loc))
                {
                    arma3addons_allowCheck = true;
                    bool newValue = !this.checkedListBox_arma3Addons.GetItemChecked(i);
                    this.checkedListBox_arma3Addons.SetItemChecked(i, newValue);//check 
                    arma3addons_allowCheck = false;
                    arma3addons_checking = true;

                    return;
                }
            }

            //Drag detection (only apply drag if not selecting listbox)
            if (!arma3addons_checking && this.checkedListBox_arma3Addons.SelectedItem != null)
            {
                this.checkedListBox_arma3Addons.DoDragDrop(this.checkedListBox_arma3Addons.SelectedItem, DragDropEffects.Move);
            }
        }
        private void checkedListBox_arma3Addons_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;

            const Single scrollRegion = 20;

            Point pt =  checkedListBox_arma3Addons.PointToClient(Cursor.Position);

            if ((pt.Y + scrollRegion) > checkedListBox_arma3Addons.Height)
            {
                //Scroll down
                SendMessage(checkedListBox_arma3Addons.Handle, (int)277, (int)1, 0);
            }
            else if (pt.Y < scrollRegion)
            {
                //Scroll up
                SendMessage(checkedListBox_arma3Addons.Handle, (int)277, (int)0, 0);
            }
        }
        private void checkedListBox_arma3Addons_DragDrop(object sender, DragEventArgs e)
        {
            Point point = checkedListBox_arma3Addons.PointToClient(new Point(e.X, e.Y));
            int index = this.checkedListBox_arma3Addons.IndexFromPoint(point);
            if (index < 0) index = this.checkedListBox_arma3Addons.Items.Count - 1;
            object data = e.Data.GetData("System.String", true);
            this.checkedListBox_arma3Addons.Items.Remove(data);
            this.checkedListBox_arma3Addons.Items.Insert(index, data);
        }
        private void sortAddons()
        {
            foreach (String addon in checkedListBox_arma3Addons.Items)
            {
                if (!Util.arma3AddonsList.Contains(addon))
                {
                    Util.arma3AddonsList.Add(addon);
                }
            }
            checkedListBox_arma3Addons.Items.Clear();

            foreach (String addon in Util.arma3AddonsList)
            {
                int index = checkedListBox_arma3Addons.Items.Add(addon);
                if (Util.arma3Addons.ContainsKey(addon))
                {
                    checkedListBox_arma3Addons.SetItemChecked(index, Util.tryParse(Util.arma3Addons[addon], false));
                }
            }
        }

        //Server parameters
        private void button_coopA3_Click(object sender, EventArgs e)
        {
            this.textBox_arma3server.Text = "11thmeu.es";
            this.textBox_arma3port.Text = "2302";
        }
        private void button_academiaA3_Click(object sender, EventArgs e)
        {
            this.textBox_arma3server.Text = "11thmeu.es";
            this.textBox_arma3port.Text = "2322";
        }
        private void button_aliveA3_Click(object sender, EventArgs e)
        {
            this.textBox_arma3server.Text = "11thmeu.es";
            this.textBox_arma3port.Text = "2332";
        }

        //Profile edition
        private void combo_profiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Read profile
            Util.readProfile(this.combo_profiles.SelectedItem.ToString());

            //Load parameters state
            this.check_noPatching.Checked = Util.tryParse("noFilePatching", false);
            this.check_skipSplash.Checked = Boolean.Parse(Util.parameters["skipSplashScreen"]);
            this.check_winXP.Checked = Boolean.Parse(Util.parameters["windowsXPMode"]);
            this.check_noPause.Checked = Boolean.Parse(Util.parameters["noPause"]);
            this.check_showErrors.Checked = Boolean.Parse(Util.parameters["showScriptErrors"]);
            this.check_emptyWorld.Checked = Boolean.Parse(Util.parameters["emptyWorld"]);
            this.check_skipIntro.Checked = Boolean.Parse(Util.parameters["skipIntro"]);
            this.check_windowMode.Checked = Boolean.Parse(Util.parameters["windowedMode"]);
            this.check_noCB.Checked = Boolean.Parse(Util.parameters["noCB"]);
            this.check_noLogs.Checked = Boolean.Parse(Util.parameters["noLogs"]);

            this.check_HT.Checked = Util.tryParse("hyperthreading", false);
            this.check_maxMemory.Checked = Boolean.Parse(Util.parameters["maxMemory"]);
            this.combo_maxMemory.SelectedIndex = int.Parse(Util.parameters["maxMemoryValue"]);
            this.check_videoMemory.Checked = Util.tryParse("maxVMemory", false);
            this.combo_maxVMemory.SelectedIndex = Util.tryParse("maxVMemoryValue", -1);
            this.check_CPUCount.Checked = Boolean.Parse(Util.parameters["cpuCount"]);
            this.combo_CPUCount.SelectedIndex = int.Parse(Util.parameters["cpuCountValue"]);
            this.check_priority.Checked = Boolean.Parse(Util.parameters["priority"]);
            this.combo_priority.SelectedIndex = int.Parse(Util.parameters["priorityValue"]);
            this.check_exThreads.Checked = Boolean.Parse(Util.parameters["extraThreads"]);
            this.combo_threads.SelectedIndex = int.Parse(Util.parameters["extraThreadsValue"]);
            this.textBox_parameters.Text = Util.parameters["extraParameters"];

            //Load addons order
            this.sortAddons();

            //Load addons state
            if (Util.arma3Addons.Count > 0)
            {
                arma3addons_allowCheck = true;
                foreach (KeyValuePair<String, String> pair in Util.arma3Addons)
                {
                    Boolean enabled = Boolean.Parse(pair.Value);
                    if (enabled)
                    {
                        int index = this.checkedListBox_arma3Addons.Items.IndexOf(pair.Key);
                        if (index != -1)
                            this.checkedListBox_arma3Addons.SetItemCheckState(index, CheckState.Checked);
                    }
                    else
                    {
                        int index = this.checkedListBox_arma3Addons.Items.IndexOf(pair.Key);
                        if (index != -1)
                            this.checkedListBox_arma3Addons.SetItemCheckState(index, CheckState.Unchecked);
                    }
                }
                arma3addons_allowCheck = false;
            }

            //Load server state
            this.textBox_arma3server.Text = Util.arma3ServerInfo["server"];
            this.textBox_arma3port.Text = Util.arma3ServerInfo["port"];
            this.textBox_arma3password.Text = Util.arma3ServerInfo["pass"];

        }
        private void button_createProfile_Click(object sender, EventArgs e)
        {
            this.textBox_profileName.Enabled = true;
            this.textBox_profileName.Text = "Nuevo perfil";
            this.button_saveProfile.Enabled = true;
            this.textBox_profileName.Select(0, textBox_profileName.Text.Length);
            this.textBox_profileName.Focus();
        }
        private void button_deleteProfile_Click(object sender, EventArgs e)
        {
            //Check the profile selected
            if (this.listBox_profiles.SelectedIndex == -1)
            {
                MetroFramework.MetroMessageBox.Show(this, "No has seleccionado ningún perfil", "Error de borrado de perfil", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (this.listBox_profiles.SelectedItem.ToString().Equals("Predeterminado"))
                {
                    MetroFramework.MetroMessageBox.Show(this, "No puedes borrar el perfil predeterminado", "Error de borrado de perfil", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    //Check if deleted profile is default
                    if (this.listBox_profiles.SelectedItem.ToString().Equals(Util.defaultProfile))
                    {
                        Util.defaultProfile = "Predeterminado";
                    }
                    Util.deleteProfile(this.listBox_profiles.SelectedItem.ToString());
                    this.updateDisplay();
                    this.combo_profiles.SelectedIndex = 0;
                }
            }
        }
        private void button_saveProfile_Click(object sender, EventArgs e)
        {
            //Check that name is not repeated
            if(Util.profiles.Contains(this.textBox_profileName.Text.Trim())){
                MetroFramework.MetroMessageBox.Show(this, "Ya existe un perfil con el nombre indicado", "Error de creación de perfil", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else {
                Util.profiles.Add(this.textBox_profileName.Text.Trim());
                Util.writeParameters();
                Util.writeProfile(this.textBox_profileName.Text.Trim());
                this.updateDisplay();
                this.textBox_profileName.Text = "";
                this.textBox_profileName.Enabled = false;
                this.button_saveProfile.Enabled = false;
            }
        }
        private void button_defaultProfile_Click(object sender, EventArgs e)
        {
            //Check the profile selected
            if (this.listBox_profiles.SelectedIndex == -1)
            {
                MetroFramework.MetroMessageBox.Show(this, "No has seleccionado ningún perfil", "Error de selección de perfil", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Util.defaultProfile = this.listBox_profiles.SelectedItem.ToString();
                Util.writeParameters();
            }
        }
        private void button_save_Click(object sender, EventArgs e)
        {
            this.updateValues();
            Util.writeProfile(this.combo_profiles.SelectedItem.ToString());
            Util.writeParameters();
            MetroFramework.MetroMessageBox.Show(this, "Perfil guardado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //Start game
        private void button_arma3SP_Click(object sender, EventArgs e)
        {
            this.updateValues();
            Util.startArmA3(false, true, false);

            if (Util.startClose)
            {
                Application.Exit();
            }
            if (Util.startMinimize)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }
        private void button_arma3MP_Click(object sender, EventArgs e)
        {
            this.updateValues();
            Util.startArmA3(true, true, false);

            if (Util.startClose)
            {
                Application.Exit();
            }
            if (Util.startMinimize)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }
        private void button_arma3HC_Click(object sender, EventArgs e)
        {
            this.updateValues();
            Util.startArmA3(true, true, true);

            if (Util.startClose)
            {
                Application.Exit();
            }
            if (Util.startMinimize)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }
        private void image_arma3SP_Click(object sender, EventArgs e)
        {
            this.updateValues();
            Clipboard.SetText(Util.startArmA3(false, false, false));
        }
        private void image_arma3MP_Click(object sender, EventArgs e)
        {
            this.updateValues();
            Clipboard.SetText(Util.startArmA3(true, false, false));
        }
        private void image_arma3HC_Click(object sender, EventArgs e)
        {
            this.updateValues();
            Clipboard.SetText(Util.startArmA3(true, false, true));
        }

        //Path configuration
        private void button_arma3Path_Click(object sender, EventArgs e)
        {
            String selectedPath = Util.pathDialog("arma3.exe");
            if (!selectedPath.Equals(""))
            {
                Util.arma3Path = selectedPath;
                Util.readAddons(this.checkedListBox_arma3Addons);
                Util.writeParameters();
                this.updateDisplay();
            }
        }

        //Tray icon
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.radioButton_minimizeNotificationBar.Checked)
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    ShowInTaskbar = false;
                    notifyIcon1.Visible = true;
                }
            }
        }
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.radioButton_minimizeNotificationBar.Checked)
            {
                ShowInTaskbar = true;
                notifyIcon1.Visible = false;
                WindowState = FormWindowState.Normal;
            }
        }

        //Other
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
        }
        private void button_clear_Click(object sender, EventArgs e)
        {
            DialogResult result = MetroFramework.MetroMessageBox.Show(this, "¿Estás seguro de que quieres borrar toda la información y perfiles guardados?", "Confirmar borrado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Directory.Delete(Util.configPath, true);
                Application.Restart();
            }
        }
        private void label_selectAll_Click(object sender, EventArgs e)
        {
            arma3addons_allowCheck = true;
            for (int i = 0; i < checkedListBox_arma3Addons.Items.Count; i++)
            {
                checkedListBox_arma3Addons.SetItemCheckState(i, CheckState.Checked);
            }
            arma3addons_allowCheck = false;
        }
        private void label_selectNone_Click(object sender, EventArgs e)
        {
            arma3addons_allowCheck = true;
            foreach (int i in checkedListBox_arma3Addons.CheckedIndices)
            {
                checkedListBox_arma3Addons.SetItemCheckState(i, CheckState.Unchecked);
            }
            arma3addons_allowCheck = false;
        }
        private void button_serverStatus_Click(object sender, EventArgs e)
        {
            int selectedIndex = this.combo_serversStatus.SelectedIndex;

            if (selectedIndex == -1) return;

            this.textBox_serverName.Text = "";
            this.textBox_serverDesc.Text = "";
            this.textBox_serverPing.Text = "";
            this.textBox_serverMap.Text = "";
            this.textBox_serverPlayers.Text = "";
            this.textBox_serverVersion.Text = "";
            this.listBox_serverPlayers.Items.Clear();
            this.textBox_serverMods.Text = "";
            this.picture_serverQuery.Image = global::_11thLauncher.Properties.Resources.image_856487;

            Net.queryException = false;
            new Thread(new ParameterizedThreadStart(Net.queryServerInfo)).Start(selectedIndex);
        }
        private void image_logViewer_Click(object sender, EventArgs e)
        {
            if (Program.viewer == null)
            {
                LogViewerForm logViewer = new LogViewerForm();
                logViewer.Show();
            }
        }
    }
}
