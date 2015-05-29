using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Security.Principal;
using System.Xml;
using System.Windows.Forms;

namespace _11thLauncher
{
    class Util
    {
        // Configuration variables
        public static String configPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\11th Launcher";
        public static String arma3Path = "";
        public static List<String> profiles = new List<String>();
        public static String defaultProfile = "";
        public static Boolean minimizeNotification = false;
        public static Boolean startClose = false;
        public static Boolean startMinimize = false;
        public static Dictionary<String, String> parameters = new Dictionary<String, String>();
        public static List<String> arma3AddonsList = new List<String>();
        public static Dictionary<String, String> arma3Addons = new Dictionary<String, String>();
        public static Dictionary<String, String> arma3ServerInfo = new Dictionary<String, String>();

        //Blacklisted addon folders (not manually activable)
        private static List<String> vanillaAddons = new List<String> { "arma 3", "curator", "kart", "heli", "mark", "dlcbundle" };

        //Reads the addons from the configuration path
        public static void readAddons(CheckedListBox arma3ListBox)
        {
            arma3Addons.Clear();
            arma3AddonsList.Clear();
            arma3ListBox.Items.Clear();

            if (arma3Path != "")
            {
                string[] directories = Directory.GetDirectories(arma3Path, "addons", SearchOption.AllDirectories);
                foreach (String directory in directories)
                {
                    if (!vanillaAddons.Contains(Directory.GetParent(directory).Name.ToLower()))
                    {
                        int pathindex = directory.IndexOf(arma3Path) + arma3Path.Length + 1;
                        String addon = directory.Substring(pathindex, (directory.Length - pathindex) - ("Addons".Length + 1));

                        //Add addon
                        arma3Addons.Add(addon, "False");
                        arma3AddonsList.Add(addon);
                        arma3ListBox.Items.Add(addon);
                    }
                }
            }
        }

        //Checks if the configuration file exists
        public static Boolean configExists()
        {
            return Directory.Exists(configPath);
        }

        //Try to read the game path from Windows registry
        public static void readPath()
        {
            String arma3regPath = "";

            if (Environment.Is64BitOperatingSystem)
            {
                arma3regPath = (String)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Bohemia Interactive\\ArmA 3", "MAIN", "null");
            }
            else
            {
                arma3regPath = (String)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Bohemia Interactive\\ArmA 3", "MAIN", "null");
            }

            if (arma3regPath != null)
            {
                if (!arma3regPath.Equals("null"))
                {
                    arma3Path = arma3regPath;
                }
            }

        }

        //Creates a dialog asking the user to input the executable path
        public static String pathDialog(String filename)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            String path = "";

            dialog.FileName = filename;
            dialog.Filter = "Ejecutables (*.exe) | *.exe";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path = System.IO.Path.GetDirectoryName(dialog.FileName);
            }

            return path;
        } 

        //Writes the XML configuration file from the configuration variables
        public static void writeParameters()
        {
            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            using (XmlWriter writer = XmlWriter.Create(configPath + "\\config.xml", settings))
            { 
                writer.WriteStartDocument();
                writer.WriteStartElement("Configuration");

                // Path
                writer.WriteElementString("ArmA3Path", arma3Path);

                //Profiles
                writer.WriteStartElement("Profiles");
                writer.WriteAttributeString("default", defaultProfile);
                if (!(profiles == null))
                {
                    foreach (String profile in profiles)
                    {
                        writer.WriteStartElement("Profile");
                        writer.WriteString(profile);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();

                //Configuration parameters
                writer.WriteElementString("minimizeNotification", minimizeNotification.ToString());
                writer.WriteElementString("startMinimize", startMinimize.ToString());
                writer.WriteElementString("startClose", startClose.ToString());

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        //Reads the XML configuration file and sets the configuration variables
        public static void readParameters()
        {
            profiles.Clear();
            using (XmlReader reader = XmlReader.Create(configPath + "\\config.xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        String value;
                        String parameter;
                        switch (reader.Name)
                        {
                            case "ArmA3Path":
                                reader.Read();
                                value = reader.Value.Trim();
                                arma3Path = value;
                                break;
                            case "Profiles":
                                parameter = reader["default"];
                                reader.Read();
                                defaultProfile = parameter;
                                break;
                            case "Profile":
                                reader.Read();
                                value = reader.Value.Trim();
                                profiles.Add(value);
                                break;
                            case "minimizeNotification":
                                reader.Read();
                                value = reader.Value.Trim();
                                minimizeNotification = Boolean.Parse(value);
                                break;
                            case "startMinimize":
                                reader.Read();
                                value = reader.Value.Trim();
                                startMinimize = Boolean.Parse(value);
                                break;
                            case "startClose":
                                reader.Read();
                                value = reader.Value.Trim();
                                startClose = Boolean.Parse(value);
                                break;
                        }
                    }
                }
            }
        }

        //Try to parse a parameter, if it doesn't exist return default value
        public static Boolean tryParse(String parameter, Boolean defaultValue)
        {
            Boolean result;

            if (parameters.ContainsKey(parameter))
            {
                result = Boolean.Parse(parameters[parameter]);
            }
            else
            {
                result = defaultValue;
            }

            return result;
        }

        //Try to parse a parameter, if it doesn't exist return default value
        public static int tryParse(String parameter, int defaultValue)
        {
            int result;

            if (parameters.ContainsKey(parameter))
            {
                result = int.Parse(parameters[parameter]);
            }
            else
            {
                result = defaultValue;
            }

            return result;
        }

        //Write profile to XML
        public static void writeProfile(String profile)
        {
            if (!Directory.Exists(configPath+"\\profiles"))
            {
                Directory.CreateDirectory(configPath+"\\profiles");
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            using (XmlWriter writer = XmlWriter.Create(configPath + "\\profiles\\" + profile + ".xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Profile");

                // Parameters
                writer.WriteStartElement("Parameters");
                if (!(parameters == null))
                {
                    foreach (KeyValuePair<String, String> entry in parameters)
                    {
                        writer.WriteStartElement("Parameter");
                        writer.WriteAttributeString("name", entry.Key);
                        writer.WriteString(entry.Value);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();

                // Addons
                writer.WriteStartElement("ArmA3Addons");
                if (!(arma3Addons == null))
                {
                    foreach (String addon in arma3AddonsList)
                    {
                        writer.WriteStartElement("A3Addon");
                        writer.WriteAttributeString("name", addon);
                        writer.WriteString(arma3Addons[addon]);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();

                // ServerInfo
                writer.WriteStartElement("ArmA3Server");
                if (!(arma3ServerInfo == null))
                {
                    foreach (KeyValuePair<String, String> entry in arma3ServerInfo)
                    {
                        writer.WriteStartElement("A3ServerInfo");
                        writer.WriteAttributeString("name", entry.Key);
                        writer.WriteString(entry.Value);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        //Read profile from XML
        public static void readProfile(String profile)
        {
            //Clear enabled addons
            foreach (String key in arma3Addons.Keys.ToList())
            {
                arma3Addons[key] = "False";
            }
            arma3AddonsList.Clear();

            //Read profile
            using (XmlReader reader = XmlReader.Create(configPath + "\\profiles\\" + profile + ".xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        String parameter;
                        String value;
                        switch (reader.Name)
                        {
                            case "ArmA3Path":
                                reader.Read();
                                value = reader.Value.Trim();
                                arma3Path = value;
                                break;
                            case "Parameter":
                                parameter = reader["name"];
                                reader.Read();
                                value = reader.Value.Trim();
                                parameters[parameter] = value;
                                break;
                            case "A3Addon":
                                parameter = reader["name"];
                                reader.Read();
                                value = reader.Value.Trim();
                                //Only set value if addon exists currently (may have been deleted)
                                if (arma3Addons.ContainsKey(parameter))
                                {
                                    arma3Addons[parameter] = value;
                                    arma3AddonsList.Add(parameter);
                                }
                                break;
                            case "A3ServerInfo":
                                parameter = reader["name"];
                                reader.Read();
                                value = reader.Value.Trim();
                                arma3ServerInfo[parameter] = value;
                                break;
                        }
                    }
                }
            }
        }

        //Delete profile
        public static void deleteProfile(String profile)
        {
            profiles.Remove(profile);
            File.Delete(configPath + "\\profiles\\" + profile + ".xml");
            writeParameters();
        }

        //Check if the application is run with administrator privileges
        public static Boolean hasPrivileges()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
        }

        //Start the game with the current parameters
        public static String startArmA3(Boolean multiplayer, Boolean startProcess, Boolean headless)
        {
            String launchParams = "";

            //Parameters
            if (Boolean.Parse(parameters["noFilePatching"]))
                launchParams += " -noFilePatching";
            if (Boolean.Parse(parameters["skipSplashScreen"]))
                launchParams += " -noSplash";
            if (Boolean.Parse(parameters["windowsXPMode"]))
                launchParams += " -winxp";
            if (Boolean.Parse(parameters["noPause"]))
                launchParams += " -noPause";
            if (Boolean.Parse(parameters["showScriptErrors"]))
                launchParams += " -showScriptErrors";
            if (Boolean.Parse(parameters["emptyWorld"]))
                launchParams += " -world=empty";
            if (Boolean.Parse(parameters["skipIntro"]))
                launchParams += " -skipIntro";
            if (Boolean.Parse(parameters["windowedMode"]))
                launchParams += " -window";
            if (Boolean.Parse(parameters["noCB"]))
                launchParams += " -noCB";
            if (Boolean.Parse(parameters["noLogs"]))
                launchParams += " -noLogs";
            if (Boolean.Parse(parameters["hyperthreading"]))
                launchParams += " -enableHT";
            if (Boolean.Parse(parameters["maxMemory"]))
            {
                switch (parameters["maxMemoryValue"])
                {
                    case "0":
                        launchParams += " -maxMem=768";
                        break;
                    case "1":
                        launchParams += " -maxMem=1024";
                        break;
                    case "2":
                        launchParams += " -maxMem=2047";
                        break;
                    default:
                        launchParams += " -maxMem=768";
                        break;
                }
            }
            if (Boolean.Parse(parameters["maxVMemory"]))
            {
                switch (parameters["maxVMemoryValue"])
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
                        launchParams += " -maxVRAM=2047";
                        break;
                    default:
                        launchParams += " -maxVRAM=1024";
                        break;
                }
            }
            if (Boolean.Parse(parameters["cpuCount"]))
            {
                int value = int.Parse(parameters["cpuCountValue"]) + 1;
                launchParams += " -cpuCount=" + value.ToString();
            }
            if (Boolean.Parse(parameters["extraThreads"]))
            {
                switch (parameters["extraThreadsValue"])
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
            if (parameters["extraParameters"].Length > 0)
                launchParams += " " + parameters["extraParameters"];

            //Addons
            String addonParams = "";
            if (arma3Addons.Count > 0)
            {
                foreach (String addon in arma3AddonsList)
                {
                    if (Boolean.Parse(arma3Addons[addon]))
                    {
                        addonParams += addon + ";";
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
                if (arma3ServerInfo["server"].Length > 0)
                    launchParams += " -connect=" + arma3ServerInfo["server"].Trim();
                if (arma3ServerInfo["port"].Length > 0)
                    launchParams += " -port=" + arma3ServerInfo["port"].Trim();
                if (arma3ServerInfo["pass"].Length > 0)
                    launchParams += " -password=" + arma3ServerInfo["pass"].Trim();
            }

            Process process = new Process();
            String path;
            //No steam found, try to launch normally
            path = arma3Path + "\\arma3.exe";
            process.StartInfo.FileName = path;
            if (Util.hasPrivileges())
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
            if (Boolean.Parse(parameters["priority"]) && startProcess)
            {
                switch (int.Parse(parameters["priorityValue"]))
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

            return path + " " + launchParams;
        }

        //Handle a unhandled exceptions
        public static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MetroFramework.MetroMessageBox.Show(Program.form, e.ExceptionObject.ToString(), "Error inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void ThreadUnhandledException(object sender, ThreadExceptionEventArgs e)
        {
            MetroFramework.MetroMessageBox.Show(Program.form, e.Exception.ToString(), "Error inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}