using System;
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
        private readonly ILogger _logger;

        #endregion

        public ParameterService(IFileAccessor fileAccessor, ILogger logger)
        {
            _fileAccessor = fileAccessor;
            _logger = logger;
        }

        public BindableCollection<LaunchParameter> Parameters { get; private set; }

        public void InitializeParameters(string arma3Path)
        {
            _logger.LogDebug("ParameterService", "Starting parameter initialization");

            BooleanParameter skipIntro = new BooleanParameter
            {
                Name = "-skipIntro",
                LegacyName = "skipIntro",
                DisplayName = Resources.Strings.S_PARAMETER_SKIP_INTRO,
                Tooltip = Resources.Strings.S_PARAMETER_SKIP_INTRO_DESC
            };
            BooleanParameter noSplash = new BooleanParameter
            {
                Name = "-noSplash",
                LegacyName = "skipSplashScreen",
                DisplayName = Resources.Strings.S_PARAMETER_NO_SPLASH,
                Tooltip = Resources.Strings.S_PARAMETER_NO_SPLASH_DESC
            };
            BooleanParameter emptyWorld = new BooleanParameter
            {
                Name = "-world=empty",
                LegacyName = "emptyWorld",
                DisplayName = Resources.Strings.S_PARAMETER_EMPTY_WORLD,
                Tooltip = Resources.Strings.S_PARAMETER_EMPTY_WORLD_DESC
            };
            BooleanParameter window = new BooleanParameter
            {
                Name = "-window",
                LegacyName = "windowedMode",
                DisplayName = Resources.Strings.S_PARAMETER_WINDOW,
                Tooltip = Resources.Strings.S_PARAMETER_WINDOW_DESC
            };
            BooleanParameter filePatching = new BooleanParameter
            {
                Name = "-filePatching",
                LegacyName = "noFilePatching",
                DisplayName = Resources.Strings.S_PARAMETER_FILE_PATCHING,
                Tooltip = Resources.Strings.S_PARAMETER_FILE_PATCHING_DESC
            };
            BooleanParameter noPause = new BooleanParameter
            {
                Name = "-noPause",
                LegacyName = "noPause",
                DisplayName = Resources.Strings.S_PARAMETER_NO_PAUSE,
                Tooltip = Resources.Strings.S_PARAMETER_NO_PAUSE_DESC
            };
            BooleanParameter noCb = new BooleanParameter
            {
                Name = "-noCB",
                LegacyName = "noCB",
                DisplayName = Resources.Strings.S_PARAMETER_NO_MULTICORE,
                Tooltip = Resources.Strings.S_PARAMETER_NO_MULTICORE_DESC
            };
            BooleanParameter showScriptErrors = new BooleanParameter
            {
                Name = "-showScriptErrors",
                LegacyName = "showScriptErrors",
                DisplayName = Resources.Strings.S_PARAMETER_SHOW_ERRORS,
                Tooltip = Resources.Strings.S_PARAMETER_SHOW_ERRORS_DESC
            };
            BooleanParameter noLogs = new BooleanParameter
            {
                Name = "-noLogs",
                LegacyName = "noLogs",
                DisplayName = Resources.Strings.S_PARAMETER_NO_LOGS,
                Tooltip = Resources.Strings.S_PARAMETER_NO_LOGS_DESC
            };
            BooleanParameter enableHt = new BooleanParameter
            {
                Name = "-enableHT",
                LegacyName = "hyperthreading",
                DisplayName = Resources.Strings.S_PARAMETER_HYPERTHREADING,
                Tooltip = Resources.Strings.S_PARAMETER_HYPERTHREADING_DESC
            };
            BooleanParameter hugePages = new BooleanParameter
            {
                Name = "-hugePages",
                LegacyName = string.Empty,
                DisplayName = Resources.Strings.S_PARAMETER_HUGE_PAGES,
                Tooltip = Resources.Strings.S_PARAMETER_HUGE_PAGES_DESC
            };
            SelectionParameter malloc32 = new SelectionParameter
            {
                Name = "-malloc=",
                LegacyName = string.Empty,
                DisplayName = Resources.Strings.S_PARAMETER_MALLOC_32,
                Tooltip = Resources.Strings.S_PARAMETER_MALLOC_32_DESC,
                Platform = ParameterPlatform.X86
            };
            SelectionParameter malloc64 = new SelectionParameter
            {
                Name = "-malloc=",
                LegacyName = string.Empty,
                DisplayName = Resources.Strings.S_PARAMETER_MALLOC_64,
                Tooltip = Resources.Strings.S_PARAMETER_MALLOC_64_DESC,
                Platform = ParameterPlatform.X64
            };
            SelectionParameter memory = new SelectionParameter
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
            SelectionParameter videoMemory = new SelectionParameter
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
            NumericalParameter cpuCount = new NumericalParameter
            {
                Name = "-cpuCount=",
                LegacyName = string.Empty,
                DisplayName = Resources.Strings.S_PARAMETER_CPU_COUNT,
                Tooltip = Resources.Strings.S_PARAMETER_CPU_COUNT_DESC,
                MinValue = 1,
                MaxValue = 32
            };
            NumericalParameter exThreads = new NumericalParameter
            {
                Name = "-exThreads=",
                LegacyName = string.Empty,
                DisplayName = Resources.Strings.S_PARAMETER_EXTHREADS,
                Tooltip = Resources.Strings.S_PARAMETER_EX_THREADS_DESC,
                MinValue = 0,
                MaxValue = 7
            };
            TextParameter additional = new TextParameter
            {
                Name = "Additional parameters",
                LegacyName = "extraParameters",
                DisplayName = Resources.Strings.S_PARAMETER_ADDITIONAL,
                Tooltip = Resources.Strings.S_PARAMETER_ADDITIONAL_DESC
            };

            Parameters = new BindableCollection<LaunchParameter> {
                skipIntro,
                noSplash,
                emptyWorld,
                window,
                filePatching,
                noPause,
                noCb,
                showScriptErrors,
                noLogs,
                enableHt,
                hugePages,
                malloc32,
                malloc64,
                memory,
                videoMemory,
                cpuCount,
                exThreads,
                additional
            };

            _logger.LogDebug("ParameterService", "Finished parameter initialization");

            //Read allocators
            BindableCollection<ValueItem> allocators32 = new BindableCollection<ValueItem>();
            BindableCollection<ValueItem> allocators64 = new BindableCollection<ValueItem>();

            allocators32.Add(new ValueItem("system", Resources.Strings.S_PARAMETER_MALLOC_SYSTEM));
            allocators64.Add(new ValueItem("system", Resources.Strings.S_PARAMETER_MALLOC_SYSTEM));

            if (arma3Path == "") return;

            try
            {
                _logger.LogDebug("ParameterService", "Starting reading memory allocators");

                string[] files = _fileAccessor.GetFiles(Path.Combine(arma3Path, Constants.AllocatorsFolder), $"*{Constants.AllocatorsPattern32}");
                foreach (string file in files)
                {
                    if (file.EndsWith(Constants.AllocatorsPattern64)) continue;
                    var name = Path.GetFileNameWithoutExtension(file);
                    allocators32.Add(new ValueItem(name, name + " (x32)"));
                }

                string[] filesX64 = _fileAccessor.GetFiles(Path.Combine(arma3Path, Constants.AllocatorsFolder), $"*{Constants.AllocatorsPattern64}");
                foreach (string file in filesX64)
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    allocators64.Add(new ValueItem(name, name + " (x64)"));
                }

                malloc32.Values = allocators32;
                malloc64.Values = allocators64;

                _logger.LogDebug("ParameterService", "Finished reading memory allocators");
            }
            catch (Exception e)
            {
                _logger.LogException("ParameterService", "Error reading memory allocators", e);
            }
        } 
    }
}
