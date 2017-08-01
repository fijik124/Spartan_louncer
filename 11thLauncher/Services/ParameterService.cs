using System.IO;
using Caliburn.Micro;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.Services
{
    class ParameterService : IParameterService
    {
        #region Fields

        private readonly LaunchParameter _skipIntro = new LaunchParameter
        {
            Name = "-skipIntro",
            LegacyName = "skipIntro",
            DisplayName = Resources.Strings.S_PARAMETER_SKIP_INTRO,
            Tooltip = Resources.Strings.S_PARAMETER_SKIP_INTRO_DESC,
            Type = ParameterType.Boolean
        };
        private readonly LaunchParameter _noSplash = new LaunchParameter
        {
            Name = "-noSplash",
            LegacyName = "skipSplashScreen",
            DisplayName = Resources.Strings.S_PARAMETER_NO_SPLASH,
            Tooltip = Resources.Strings.S_PARAMETER_NO_SPLASH_DESC,
            Type = ParameterType.Boolean
        };
        private readonly LaunchParameter _emptyWorld = new LaunchParameter
        {
            Name = "-world=empty",
            LegacyName = "emptyWorld",
            DisplayName = Resources.Strings.S_PARAMETER_EMPTY_WORLD,
            Tooltip = Resources.Strings.S_PARAMETER_EMPTY_WORLD_DESC,
            Type = ParameterType.Boolean
        };
        private readonly LaunchParameter _window = new LaunchParameter
        {
            Name = "-window",
            LegacyName = "windowedMode",
            DisplayName = Resources.Strings.S_PARAMETER_WINDOW,
            Tooltip = Resources.Strings.S_PARAMETER_WINDOW_DESC,
            Type = ParameterType.Boolean
        };
        private readonly LaunchParameter _filePatching = new LaunchParameter
        {
            Name = "-filePatching",
            LegacyName = "noFilePatching",
            DisplayName = Resources.Strings.S_PARAMETER_FILE_PATCHING,
            Tooltip = Resources.Strings.S_PARAMETER_FILE_PATCHING_DESC,
            Type = ParameterType.Boolean
        };
        private readonly LaunchParameter _noPause = new LaunchParameter
        {
            Name = "-noPause",
            LegacyName = "noPause",
            DisplayName = Resources.Strings.S_PARAMETER_NO_PAUSE,
            Tooltip = Resources.Strings.S_PARAMETER_NO_PAUSE_DESC,
            Type = ParameterType.Boolean
        };
        private readonly LaunchParameter _noCb = new LaunchParameter
        {
            Name = "-noCB",
            LegacyName = "noCB",
            DisplayName = Resources.Strings.S_PARAMETER_NO_MULTICORE,
            Tooltip = Resources.Strings.S_PARAMETER_NO_MULTICORE_DESC,
            Type = ParameterType.Boolean
        };
        private readonly LaunchParameter _showScriptErrors = new LaunchParameter
        {
            Name = "-showScriptErrors",
            LegacyName = "showScriptErrors",
            DisplayName = Resources.Strings.S_PARAMETER_SHOW_ERRORS,
            Tooltip = Resources.Strings.S_PARAMETER_SHOW_ERRORS_DESC,
            Type = ParameterType.Boolean
        };
        private readonly LaunchParameter _noLogs = new LaunchParameter
        {
            Name = "-noLogs",
            LegacyName = "noLogs",
            DisplayName = Resources.Strings.S_PARAMETER_NO_LOGS,
            Tooltip = Resources.Strings.S_PARAMETER_NO_LOGS_DESC,
            Type = ParameterType.Boolean
        };
        private readonly LaunchParameter _enableHt = new LaunchParameter
        {
            Name = "-enableHT",
            LegacyName = "hyperthreading",
            DisplayName = Resources.Strings.S_PARAMETER_HYPERTHREADING,
            Tooltip = Resources.Strings.S_PARAMETER_HYPERTHREADING_DESC,
            Type = ParameterType.Boolean
        };
        private readonly LaunchParameter _hugePages = new LaunchParameter
        {
            Name = "-hugePages",
            LegacyName = string.Empty,
            DisplayName = Resources.Strings.S_PARAMETER_HUGE_PAGES,
            Tooltip = Resources.Strings.S_PARAMETER_HUGE_PAGES_DESC,
            Type = ParameterType.Boolean
        };
        private readonly SelectionParameter _malloc32 = new SelectionParameter
        {
            Name = "-malloc=",
            LegacyName = "memoryAllocatorValue",
            DisplayName = Resources.Strings.S_PARAMETER_MALLOC_32,
            Tooltip = Resources.Strings.S_PARAMETER_MALLOC_32_DESC,
            Type = ParameterType.Selection,
            Platform = ParameterPlatform.X86
        };
        private readonly SelectionParameter _malloc64 = new SelectionParameter
        {
            Name = "-malloc=",
            LegacyName = "",
            DisplayName = Resources.Strings.S_PARAMETER_MALLOC_64,
            Tooltip = Resources.Strings.S_PARAMETER_MALLOC_64_DESC,
            Type = ParameterType.Selection,
            Platform = ParameterPlatform.X64
        };
        private readonly SelectionParameter _memory = new SelectionParameter
        {
            Name = "-maxMem=",
            LegacyName = "maxVMemoryValue",
            DisplayName = "TODO",
            Tooltip = "TODO", 
            Values = new BindableCollection<ParameterValueItem>
            {
                new ParameterValueItem("768", "TODO768"),
                new ParameterValueItem("1024", "TODO1024"),
                new ParameterValueItem("2048", "TODO2048"),
                new ParameterValueItem("4096", "TODO4096"),
                new ParameterValueItem("8192", "TODO8192"),
            }
        };
        private readonly LaunchParameter _additional = new LaunchParameter
        {
            Name = "",
            LegacyName = "extraParameters",
            DisplayName = Resources.Strings.S_PARAMETER_ADDITIONAL,
            Tooltip = Resources.Strings.S_PARAMETER_ADDITIONAL_DESC,
            Type = ParameterType.Text
        };

        #endregion

        public ParameterService()
        {
            Parameters = new BindableCollection<LaunchParameter> {
                _skipIntro,
                _noSplash,
                _emptyWorld,
                _window,
                _filePatching,
                _noPause,
                _noCb,
                _showScriptErrors,
                _noLogs,
                _enableHt,
                _hugePages,
                _malloc32,
                _malloc64,
                _memory,
                _additional
            };
        }

        public BindableCollection<LaunchParameter> Parameters { get; }

        public void ReadAllocators(string arma3Path)
        {
            BindableCollection<ParameterValueItem> allocators32 = new BindableCollection<ParameterValueItem>();
            BindableCollection<ParameterValueItem> allocators64 = new BindableCollection<ParameterValueItem>();

            allocators32.Add(new ParameterValueItem("system", Resources.Strings.S_PARAMETER_MALLOC_SYSTEM));
            allocators64.Add(new ParameterValueItem("system", Resources.Strings.S_PARAMETER_MALLOC_SYSTEM));

            if (arma3Path == "") return;

            string[] files = Directory.GetFiles(arma3Path + "\\Dll", "*.dll");
            foreach (string file in files)
            {
                if (file.EndsWith("_x64.dll")) continue;
                var name = Path.GetFileNameWithoutExtension(file);
                allocators32.Add(new ParameterValueItem(name, name + " (x32)"));
            }

            string[] filesX64 = Directory.GetFiles(arma3Path + "\\Dll", "*_x64.dll");
            foreach (string file in filesX64)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                allocators64.Add(new ParameterValueItem(name, name + " (x64)"));
            }

            _malloc32.Values = allocators32;
            _malloc64.Values = allocators64;
        }
    }
}
