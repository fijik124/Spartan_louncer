using System;
using System.Collections.Generic;
using System.IO;
using Caliburn.Micro;
using _11thLauncher.Model.Addon;
using _11thLauncher.Model.Parameter;
using _11thLauncher.Properties;

namespace _11thLauncher.Model
{
    public static class Constants
    {
        //
        // Program Settings
        //
        public static readonly string ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\11th Launcher";
        public static readonly string ProfilesPath = ConfigPath + "\\profiles";
        public static readonly List<string> Accents = new List<string>
        {
            "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet",
            "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna"
        };

        //
        // Game settings
        //
        public static readonly string Arma3Filename32 = "arma3.exe";
        public static readonly string Arma3Filename64 = "arma3_x64.exe";
        public static readonly string[] Arma3RegPath32 = { "HKEY_LOCAL_MACHINE\\SOFTWARE\\Bohemia Interactive\\ArmA 3", "MAIN", null };
        public static readonly string[] Arma3RegPath64 = { "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Bohemia Interactive\\ArmA 3", "MAIN", null };
        public static readonly string[] SteamRegPath32 = { "HKEY_LOCAL_MACHINE\\SOFTWARE\\Valve\\Steam", "InstallPath", "" };
        public static readonly string[] SteamRegPath64 = { "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Valve\\Steam", "InstallPath", ""};
        public static readonly string DefaultArma3SteamPath = "SteamApps\\common\\ArmA 3";
        public static readonly string[] VanillaAddons = { "arma 3", "expansion", "curator", "kart", "heli", "mark", "dlcbundle" };
        public static readonly string AddonSubfolderName = "addons";

        //
        // Game parameters
        //
        public static readonly BindableCollection<LaunchParameter> Parameters = new BindableCollection<LaunchParameter>
        {
            new LaunchParameter("Name1", "DisplayName1", "tooltip1", ParameterType.Boolean),
            new LaunchParameter("Name2", "DisplayName2aaaaaaaaaaaaaaaa", "tooltip2", ParameterType.Selection, new BindableCollection<ParameterValueItem>
            {
                new ParameterValueItem("DisplayValue1", "Value1"),
                new ParameterValueItem("DisplayValue2", "Value2")
            })
        };

        // 
        // 11th MEU addon presets
        //
        public static readonly BindableCollection<Preset> AddonPresets = new BindableCollection<Preset>
        {
            new Preset
            {
                Name = Resources.S_PRESET1,
                Addons = new[] { "@cba_a3", "@ace", "@acre2", "@cup_terrains", "@meu", "@meu_maps", "@meu_rhs", "@rhsafrf", "@rhsgref", "@rhsusaf" }
            },
            new Preset
            {
                Name = Resources.S_PRESET2,
                Addons = new[] { "@cba_a3", "@ace", "@acre2", "@cup_terrains", "@meu", "@meu_maps", "@meu_rhs", "@rhsafrf", "@rhsgref", "@rhsusaf", "@alive" }
            },
            new Preset
            {
                Name = Resources.S_PRESET3,
                Addons = new[] { "@cba_a3", "@ace", "@acre2", "@meu", "@unsung" }
            }
        };

        //
        // Profile settings
        //
        public static readonly string DefaultProfileName = "Predeterminado";

        //
        // Repository Settings
        //
        public static readonly string A3SdsPath = Path.Combine(Path.GetTempPath(), "A3SDS.jar");

        //
        // Server Settings
        //
        public static readonly ushort[] ServerPorts = { 2303, 2323, 2333 }; //Query port = server port + 1

        //
        // Updater Settings
        //
        public static readonly string VersionUrl = "http://raw.githubusercontent.com/11thmeu/launcher/master/bin/version";
        public static readonly string DownloadBaseUrl = "https://raw.githubusercontent.com/11thmeu/launcher/master/bin/";
        public static readonly string UpdaterPath = Path.Combine(Path.GetTempPath(), "11thLauncherUpdater.exe");

        public static readonly string CurrentVersion = "210";
    }
}
