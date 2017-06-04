using System;
using System.IO;
using Caliburn.Micro;
using _11thLauncher.Model.Addons;

namespace _11thLauncher
{
    public static class Constants
    {
        //
        // Program Settings
        //
        public static readonly string ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\11th Launcher";
        public static readonly string ProfilesPath = ConfigPath + "\\profiles";
        public static readonly string ConfigFileName = "config.json";
        public static readonly string LogoLight = "pack://application:,,,/Resources/a3logo.png";
        public static readonly string LogoDark = "pack://application:,,,/Resources/a3logo_inverted.png";

        //
        // Game settings
        //
        public static readonly string SteamProtocol = "steam";
        public static readonly int Arma3SteamAppId = 107410;
        public static readonly string[] Arma3RegPath32 = { "HKEY_LOCAL_MACHINE\\SOFTWARE\\Bohemia Interactive\\ArmA 3", "MAIN", null };
        public static readonly string[] Arma3RegPath64 = { "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Bohemia Interactive\\ArmA 3", "MAIN", null };
        public static readonly string[] SteamRegPath32 = { "HKEY_LOCAL_MACHINE\\SOFTWARE\\Valve\\Steam", "InstallPath", "" };
        public static readonly string[] SteamRegPath64 = { "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Valve\\Steam", "InstallPath", ""};
        public static readonly string DefaultArma3SteamPath = "SteamApps\\common\\ArmA 3";
        public static readonly string[] VanillaAddons = { "arma 3", "expansion", "curator", "kart", "heli", "mark", "jets", "orange", "argo", "tacops", "tanks", "dlcbundle" };
        public static readonly string AddonSubfolderName = "addons";

        // 
        // 11th MEU addon presets
        //
        public static readonly BindableCollection<Preset> AddonPresets = new BindableCollection<Preset>
        {
            new Preset
            {
                Name = Resources.Strings.S_PRESET1,
                Addons = new[] { "@cba_a3", "@ace", "@acre2", "@cup_terrains", "@meu", "@meu_maps", "@meu_rhs", "@rhsafrf", "@rhsgref", "@rhsusaf" }
            },
            new Preset
            {
                Name = Resources.Strings.S_PRESET2,
                Addons = new[] { "@cba_a3", "@ace", "@acre2", "@cup_terrains", "@meu", "@meu_maps", "@meu_rhs", "@rhsafrf", "@rhsgref", "@rhsusaf", "@alive" }
            },
            new Preset
            {
                Name = Resources.Strings.S_PRESET3,
                Addons = new[] { "@cba_a3", "@ace", "@acre2", "@meu", "@unsung" }
            }
        };

        //
        // Repository Settings
        //
        public static readonly string A3SdsPath = Path.Combine(Path.GetTempPath(), "A3SDS.jar");

        //
        // Server Settings
        //
        public static readonly string ServerUrl = "www.11thmeu.es"; //TODO deprecate
        public static readonly ushort[] ServerPorts = { 2303, 2323, 2333 }; //Query port = server port + 1

        //
        // Updater Settings
        //
        public static readonly string VersionUrl = "http://raw.githubusercontent.com/11thmeu/launcher/master/bin/version";
        public static readonly string DownloadBaseUrl = "https://raw.githubusercontent.com/11thmeu/launcher/master/bin/";
        public static readonly string UpdaterPath = Path.Combine(Path.GetTempPath(), "11thLauncherUpdater.exe");

        public static readonly string CurrentVersion = "300";

        //
        // About info
        //
        public static string Author = "Javier 'Thrax' Rico";
        public static string BuildCodeName = "Echo";
        public static DateTime BuildDate = new DateTime(2017, 06, 01);
    }
}
