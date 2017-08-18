using System.IO;
using Caliburn.Micro;
using _11thLauncher.Accessors.Contracts;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.Services
{
    class ParameterService : IParameterService
    {
        #region Fields

        private readonly IFileAccessor _fileAccessor;

        private readonly BooleanParameter _skipIntro = new BooleanParameter
        {
            Name = "-skipIntro",
            LegacyName = "skipIntro",
            DisplayName = Resources.Strings.S_PARAMETER_SKIP_INTRO,
            Tooltip = Resources.Strings.S_PARAMETER_SKIP_INTRO_DESC
        };
        private readonly BooleanParameter _noSplash = new BooleanParameter
        {
            Name = "-noSplash",
            LegacyName = "skipSplashScreen",
            DisplayName = Resources.Strings.S_PARAMETER_NO_SPLASH,
            Tooltip = Resources.Strings.S_PARAMETER_NO_SPLASH_DESC
        };
        private readonly BooleanParameter _emptyWorld = new BooleanParameter
        {
            Name = "-world=empty",
            LegacyName = "emptyWorld",
            DisplayName = Resources.Strings.S_PARAMETER_EMPTY_WORLD,
            Tooltip = Resources.Strings.S_PARAMETER_EMPTY_WORLD_DESC
        };
        private readonly BooleanParameter _window = new BooleanParameter
        {
            Name = "-window",
            LegacyName = "windowedMode",
            DisplayName = Resources.Strings.S_PARAMETER_WINDOW,
            Tooltip = Resources.Strings.S_PARAMETER_WINDOW_DESC
        };
        private readonly BooleanParameter _filePatching = new BooleanParameter
        {
            Name = "-filePatching",
            LegacyName = "noFilePatching",
            DisplayName = Resources.Strings.S_PARAMETER_FILE_PATCHING,
            Tooltip = Resources.Strings.S_PARAMETER_FILE_PATCHING_DESC
        };
        private readonly BooleanParameter _noPause = new BooleanParameter
        {
            Name = "-noPause",
            LegacyName = "noPause",
            DisplayName = Resources.Strings.S_PARAMETER_NO_PAUSE,
            Tooltip = Resources.Strings.S_PARAMETER_NO_PAUSE_DESC
        };
        private readonly BooleanParameter _noCb = new BooleanParameter
        {
            Name = "-noCB",
            LegacyName = "noCB",
            DisplayName = Resources.Strings.S_PARAMETER_NO_MULTICORE,
            Tooltip = Resources.Strings.S_PARAMETER_NO_MULTICORE_DESC
        };
        private readonly BooleanParameter _showScriptErrors = new BooleanParameter
        {
            Name = "-showScriptErrors",
            LegacyName = "showScriptErrors",
            DisplayName = Resources.Strings.S_PARAMETER_SHOW_ERRORS,
            Tooltip = Resources.Strings.S_PARAMETER_SHOW_ERRORS_DESC
        };
        private readonly BooleanParameter _noLogs = new BooleanParameter
        {
            Name = "-noLogs",
            LegacyName = "noLogs",
            DisplayName = Resources.Strings.S_PARAMETER_NO_LOGS,
            Tooltip = Resources.Strings.S_PARAMETER_NO_LOGS_DESC
        };
        private readonly BooleanParameter _enableHt = new BooleanParameter
        {
            Name = "-enableHT",
            LegacyName = "hyperthreading",
            DisplayName = Resources.Strings.S_PARAMETER_HYPERTHREADING,
            Tooltip = Resources.Strings.S_PARAMETER_HYPERTHREADING_DESC
        };
        private readonly BooleanParameter _hugePages = new BooleanParameter
        {
            Name = "-hugePages",
            LegacyName = string.Empty,
            DisplayName = Resources.Strings.S_PARAMETER_HUGE_PAGES,
            Tooltip = Resources.Strings.S_PARAMETER_HUGE_PAGES_DESC
        };
        private readonly SelectionParameter _malloc32 = new SelectionParameter
        {
            Name = "-malloc=",
            LegacyName = string.Empty,
            DisplayName = Resources.Strings.S_PARAMETER_MALLOC_32,
            Tooltip = Resources.Strings.S_PARAMETER_MALLOC_32_DESC,
            Platform = ParameterPlatform.X86
        };
        private readonly SelectionParameter _malloc64 = new SelectionParameter
        {
            Name = "-malloc=",
            LegacyName = string.Empty,
            DisplayName = Resources.Strings.S_PARAMETER_MALLOC_64,
            Tooltip = Resources.Strings.S_PARAMETER_MALLOC_64_DESC,
            Platform = ParameterPlatform.X64
        };
        private readonly SelectionParameter _memory = new SelectionParameter
        {
            Name = "-maxMem=",
            LegacyName = string.Empty,
            DisplayName = Resources.Strings.S_PARAMETER_MAX_MEMORY,
            Tooltip = Resources.Strings.S_PARAMETER_MAX_MEMORY_DESC, 
            Values = new BindableCollection<ValueItem>
            {
                new ValueItem("1024", "1024 MiB"),
                new ValueItem("2048", "2048 MiB"),
                new ValueItem("4096", "4096 MiB"),
                new ValueItem("8192", "8192 MiB"),
            }
        };
        private readonly SelectionParameter _videoMemory = new SelectionParameter
        {
            Name = "-maxVRAM=",
            LegacyName = string.Empty,
            DisplayName = Resources.Strings.S_PARAMETER_MAX_VMEMORY,
            Tooltip = Resources.Strings.S_PARAMETER_MAX_VMEMORY_DESC,
            Values = new BindableCollection<ValueItem>
            {
                new ValueItem("512", "512 MiB"),
                new ValueItem("1024", "1024 MiB"),
                new ValueItem("2048", "2048 MiB"),
                new ValueItem("4096", "4096 MiB"),
                new ValueItem("8192", "8192 MiB"),
            }
        };
        private readonly NumericalParameter _cpuCount = new NumericalParameter
        {
            Name = "-cpuCount=",
            LegacyName = string.Empty,
            DisplayName = Resources.Strings.S_PARAMETER_CPU_COUNT,
            Tooltip = Resources.Strings.S_PARAMETER_CPU_COUNT_DESC,
            MinValue = 1,
            MaxValue = 32
        };
        private readonly NumericalParameter _exThreads = new NumericalParameter
        {
            Name = "-exThreads=",
            LegacyName = string.Empty,
            DisplayName = Resources.Strings.S_PARAMETER_EXTHREADS,
            Tooltip = Resources.Strings.S_PARAMETER_EX_THREADS_DESC,
            MinValue = 0,
            MaxValue = 7
        };
        private readonly TextParameter _additional = new TextParameter
        {
            Name = "Additional parameters",
            LegacyName = "extraParameters",
            DisplayName = Resources.Strings.S_PARAMETER_ADDITIONAL,
            Tooltip = Resources.Strings.S_PARAMETER_ADDITIONAL_DESC
        };

        #endregion

        public ParameterService(IFileAccessor fileAccessor)
        {
            _fileAccessor = fileAccessor;

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
                _videoMemory,
                _cpuCount,
                _exThreads,
                _additional
            };
        }

        public BindableCollection<LaunchParameter> Parameters { get; }

        public void ReadAllocators(string arma3Path)
        {
            BindableCollection<ValueItem> allocators32 = new BindableCollection<ValueItem>();
            BindableCollection<ValueItem> allocators64 = new BindableCollection<ValueItem>();

            allocators32.Add(new ValueItem("system", Resources.Strings.S_PARAMETER_MALLOC_SYSTEM));
            allocators64.Add(new ValueItem("system", Resources.Strings.S_PARAMETER_MALLOC_SYSTEM));

            if (arma3Path == "") return;

            string[] files = _fileAccessor.GetFiles(arma3Path + "\\Dll", "*.dll");
            foreach (string file in files)
            {
                if (file.EndsWith("_x64.dll")) continue;
                var name = Path.GetFileNameWithoutExtension(file);
                allocators32.Add(new ValueItem(name, name + " (x32)"));
            }

            string[] filesX64 = _fileAccessor.GetFiles(arma3Path + "\\Dll", "*_x64.dll");
            foreach (string file in filesX64)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                allocators64.Add(new ValueItem(name, name + " (x64)"));
            }

            _malloc32.Values = allocators32;
            _malloc64.Values = allocators64;
        }
    }
}
