using System;
using System.IO;
using Caliburn.Micro;
using Newtonsoft.Json;
using _11thLauncher.Models;

namespace _11thLauncher
{
    public static class Constants
    {
        //
        // Addon Service
        //
        public static readonly string[] VanillaAddons = { "arma 3", "expansion", "curator", "kart", "heli", "mark", "jets", "orange", "argo", "tacops", "tanks", "dlcbundle" };
        public const string AddonSubfolderName = "addons";
        public const string AddonFilePattern = "*.?bo";
        public const string AddonMetaDataFile = "mod.cpp";
        public static readonly BindableCollection<Preset> AddonPresets = new BindableCollection<Preset>
        {
            new Preset
            {
                Name = "Guerra Moderna",
                Addons = new[] { "@cba_a3", "@ace", "@acre2", "@cup_terrains", "@meu", "@meu_maps", "@meu_rhs", "@meu_fleet", "@rhsafrf", "@rhsgref", "@rhsusaf" }
            },
            new Preset
            {
                Name = "Guerra Moderna [ALiVE]",
                Addons = new[] { "@cba_a3", "@ace", "@acre2", "@cup_terrains", "@meu", "@meu_maps", "@meu_rhs", "@meu_fleet", "@rhsafrf", "@rhsgref", "@rhsusaf", "@alive" }
            },
            new Preset
            {
                Name = "Vietnam [Unsung]",
                Addons = new[] { "@cba_a3", "@ace", "@acre2", "@meu", "@unsung" }
            }
        };

        //
        // Parameter Service
        //
        public const string AllocatorsFolder = "Dll";
        public const string AllocatorsPattern32 = ".dll";
        public const string AllocatorsPattern64 = "_x64.dll";

        //
        // Server Status Service
        //
        public static readonly Server[] DefaultServers = 
        {
            new Server
            {
                Name = "Coop",
                Address = "11thmeu.es",
                Port = 2302,
                IsDefault = true,
                IsEnabled = true
            },
            new Server
            {
                Name = "Academia",
                Address = "11thmeu.es",
                Port = 2322,
                IsDefault = true,
                IsEnabled = true
            },
            new Server
            {
                Name = "ALiVE",
                Address = "11thmeu.es",
                Port = 2332,
                IsDefault = true,
                IsEnabled = true
            },
            new Server
            {
                Name = "Test",
                Address = "11thmeu.es",
                Port = 2342,
                IsDefault = true,
                IsEnabled = true
            },
            new Server
            {
                Name = "Vietnam",
                Address = "11thmeu.es",
                Port = 2352,
                IsDefault = true,
                IsEnabled = true
            }
        };

        //
        // Arma3Sync Service
        //
        public static readonly string A3SdsPath = Path.Combine(Path.GetTempPath(), "A3SDS.jar");
        public const string JavaExecutable = "java.exe";
        public const string JavaPathCommand = "java";
        public const string JavaRuntimeBinaryFolder = "bin";
        public const string Arma3SyncBaseRegistryPath32 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        public const string Arma3SyncBaseRegistryPath64 = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
        public const string Arma3SyncRegDisplayNameEntry = "DisplayName";
        public const string Arma3SyncRegDisplayNameValue = "ArmA3Sync";
        public const string Arma3SyncRegLocationEntry = "InstallLocation";
        public const string Arma3SyncExecutable = "ArmA3Sync.exe";
        public const string Arma3SyncConfigFolder = @"resources\ftp\";
        public const string Arma3SyncRepositoryExtension = @".a3s.repository";
        public const string Arma3SyncRemoteServerInfo = @"ftp://{0}/.a3s/serverinfo";

        //
        // Updater Service
        //
        public const string UpdaterServiceUserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
        public const string GithubApiReleaseEndpoint = "https://api.github.com/repos/11thmeu/launcher/releases/latest";
        public const string GithubApiCurrentVersion = "application/vnd.github.v3+json";
        public const string GithubVersionTagFormat = "v{0}";
        public static readonly string UpdaterPath = Path.Combine(Path.GetTempPath(), "11thLauncher.Updater.exe");

        //
        // Settings Service
        //
        public static readonly string[] Languages = { "en-US", "es-ES" };
        public static readonly string ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "11th Launcher");
        public const string ConfigFileName = "config.json";
        public const string LegacyConfigFileName = "config.xml";
        public static readonly string[] Arma3RegPath32 = { @"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive\ArmA 3", "MAIN", null };
        public static readonly string[] Arma3RegPath64 = { @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive\ArmA 3", "MAIN", null };
        public static readonly string[] SteamRegPath32 = { @"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", "" };
        public static readonly string[] SteamRegPath64 = { @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", "" };
        public const string DefaultArma3SteamPath = @"SteamApps\common\ArmA 3";
        public const string GameExecutable32 = "arma3.exe";
        public const string GameExecutable64 = "arma3_x64.exe";
#if DEBUG 
        public const Formatting JsonFormatting = Formatting.Indented;
#else
        public const Formatting JsonFormatting = Formatting.None;
#endif

        //
        // Profile Service
        //
        public const string ProfilesFolder = "profiles";
        public const string ProfileNameFormat = "{0}.json";
        public const string LegacyProfileNameFormat = "{0}.xml";

        //
        // Logging
        //
#if DEBUG
        public const LogLevel MaxLogLevel = LogLevel.DEBUG;
#else
        public const LogLevel MaxLogLevel = LogLevel.INFO;
#endif
        public const int LogRolledFiles = 2;
        public const long LogSizeLimit = 5242880; //5MB

        //
        // Program Settings
        //
        public const string LogoLight = "pack://application:,,,/Resources/a3logo.png";
        public const string LogoDark = "pack://application:,,,/Resources/a3logo_inverted.png";
        public const string Arma3SyncIconEnabled = "pack://application:,,,/Resources/a3sEnabled.png";
        public const string Arma3SyncIconDisabled = "pack://application:,,,/Resources/a3sDisabled.png";

        //
        // Build info
        //
        public const string Author = "Javier 'Thrax' Rico";
        public const string BuildCodeName = "Echo";
        public static DateTime BuildDate = new DateTime(2017, 06, 01);
    }
}