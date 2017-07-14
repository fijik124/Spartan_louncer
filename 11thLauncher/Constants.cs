using System;
using System.IO;
using System.Reflection;
using Caliburn.Micro;
using _11thLauncher.Models;

namespace _11thLauncher
{
    public static class Constants
    {
        //
        // Build info
        //
        public const string Author = "Javier 'Thrax' Rico";
        public const string BuildCodeName = "Echo";
        public static DateTime BuildDate = new DateTime(2017, 06, 01);
        public static readonly string AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        //
        // Program Settings
        //
        public static readonly string ProfilesPath = ConfigPath + "\\profiles";
        public const string LogoLight = "pack://application:,,,/Resources/a3logo.png";
        public const string LogoDark = "pack://application:,,,/Resources/a3logo_inverted.png";

        //
        // Settings Service
        //
        public static readonly string[] Languages = { "en-US", "es-ES" };
        public static readonly string ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "11th Launcher");
        public const string ConfigFileName = "config.json";
        public static readonly string[] Arma3RegPath32 = { @"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive\ArmA 3", "MAIN", null };
        public static readonly string[] Arma3RegPath64 = { @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive\ArmA 3", "MAIN", null };
        public static readonly string[] SteamRegPath32 = { @"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", "" };
        public static readonly string[] SteamRegPath64 = { @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", "" };
        public const string DefaultArma3SteamPath = @"SteamApps\common\ArmA 3";
        public const string GameExecutable32 = "arma3.exe";
        public const string GameExecutable64 = "arma3_x64.exe";

        //
        // Addon Service
        //
        public static readonly string[] VanillaAddons = { "arma 3", "expansion", "curator", "kart", "heli", "mark", "jets", "orange", "argo", "tacops", "tanks", "dlcbundle" };
        public const string AddonSubfolderName = "addons";
        public const string AddonFilePattern = "*.?bo";
        public const string AddonMetaDataFile = "mod.cpp";

        //
        // Updater Service
        //
        public const string VersionUrl = "http://raw.githubusercontent.com/11thmeu/launcher/master/bin/version";
        public const string DownloadBaseUrl = "https://raw.githubusercontent.com/11thmeu/launcher/master/bin/";
        public static readonly string UpdaterPath = Path.Combine(Path.GetTempPath(), "11thLauncherUpdater.exe");

        //
        // Arma3Sync Service
        //
        public const string JavaExecutable = "java.exe";
        public const string JavaRuntimeBinaryFolder = "bin";
        public const string Arma3SyncBaseRegistryPath32 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        public const string Arma3SyncBaseRegistryPath64 = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
        public const string Arma3SyncRegDisplayNameEntry = "DisplayName";
        public const string Arma3SyncRegDisplayNameValue = "ArmA3Sync";
        public const string Arma3SyncRegLocationEntry = "InstallLocation";
        public const string RepositoryConfigFolder = @"\resources\ftp\";

        // 
        // 11th MEU addon presets
        //
        public static readonly BindableCollection<Preset> AddonPresets = new BindableCollection<Preset>
        {
            new Preset
            {
                Name = "Guerra Moderna",
                Addons = new[] { "@cba_a3", "@ace", "@acre2", "@cup_terrains", "@meu", "@meu_maps", "@meu_rhs", "@rhsafrf", "@rhsgref", "@rhsusaf" }
            },
            new Preset
            {
                Name = "Guerra Moderna [ALiVE]",
                Addons = new[] { "@cba_a3", "@ace", "@acre2", "@cup_terrains", "@meu", "@meu_maps", "@meu_rhs", "@rhsafrf", "@rhsgref", "@rhsusaf", "@alive" }
            },
            new Preset
            {
                Name = "Vietnam [Unsung]",
                Addons = new[] { "@cba_a3", "@ace", "@acre2", "@meu", "@unsung" }
            }
        };

        //
        // Repository Settings
        //
        public static readonly string A3SdsPath = Path.Combine(Path.GetTempPath(), "A3SDS.jar");
    }
}
