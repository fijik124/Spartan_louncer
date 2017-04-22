using System.IO;
using Caliburn.Micro;
using _11thLauncher.Model.Game;
using _11thLauncher.Properties;

namespace _11thLauncher.Model.Parameter
{
    public class ParameterManager
    {
        public LaunchParameter SkipIntro = new LaunchParameter("-skipIntro", Resources.S_PARAMETER_SKIP_INTRO, Resources.S_PARAMETER_SKIP_INTRO_DESC, ParameterType.Boolean);
        public LaunchParameter NoSplash = new LaunchParameter("-noSplash", Resources.S_PARAMETER_NO_SPLASH, Resources.S_PARAMETER_NO_SPLASH_DESC, ParameterType.Boolean);
        public LaunchParameter EmptyWorld = new LaunchParameter("-world=empty", Resources.S_PARAMETER_EMPTY_WORLD, Resources.S_PARAMETER_EMPTY_WORLD_DESC, ParameterType.Boolean);
        public LaunchParameter Window = new LaunchParameter("-window", Resources.S_PARAMETER_WINDOW, Resources.S_PARAMETER_WINDOW_DESC, ParameterType.Boolean);
        public LaunchParameter FilePatching = new LaunchParameter("-filePatching", Resources.S_PARAMETER_FILE_PATCHING, Resources.S_PARAMETER_FILE_PATCHING_DESC, ParameterType.Boolean);
        public LaunchParameter NoPause = new LaunchParameter("-noPause", Resources.S_PARAMETER_NO_PAUSE, Resources.S_PARAMETER_NO_PAUSE_DESC, ParameterType.Boolean);
        public LaunchParameter NoCb = new LaunchParameter("-noCB", Resources.S_PARAMETER_NO_MULTICORE, Resources.S_PARAMETER_NO_MULTICORE_DESC, ParameterType.Boolean);
        public LaunchParameter ShowScriptErrors = new LaunchParameter("-showScriptErrors", Resources.S_PARAMETER_SHOW_ERRORS, Resources.S_PARAMETER_SHOW_ERRORS_DESC, ParameterType.Boolean);
        public LaunchParameter NoLogs = new LaunchParameter("-noLogs", Resources.S_PARAMETER_NO_LOGS, Resources.S_PARAMETER_NO_LOGS_DESC, ParameterType.Boolean);
        public LaunchParameter EnableHt = new LaunchParameter("-enableHT", Resources.S_PARAMETER_HYPERTHREADING, Resources.S_PARAMETER_HYPERTHREADING_DESC, ParameterType.Boolean);
        public LaunchParameter HugePages = new LaunchParameter("-hugePages", Resources.S_PARAMETER_HUGE_PAGES, Resources.S_PARAMETER_HUGE_PAGES_DESC, ParameterType.Boolean);
        public LaunchParameter Malloc32 = new LaunchParameter("-malloc=", Resources.S_PARAMETER_MALLOC_32, Resources.S_PARAMETER_MALLOC_32_DESC,
            ParameterType.Selection, Platform.X86);
        public LaunchParameter Malloc64 = new LaunchParameter("-malloc=", Resources.S_PARAMETER_MALLOC_64, Resources.S_PARAMETER_MALLOC_64_DESC,
            ParameterType.Selection, Platform.X64);
        public LaunchParameter Additional = new LaunchParameter("", Resources.S_PARAMETER_ADDITIONAL, Resources.S_PARAMETER_ADDITIONAL_DESC, ParameterType.Text);

        public BindableCollection<LaunchParameter> Parameters;

        public ParameterManager()
        {
            Parameters = new BindableCollection<LaunchParameter> {
                SkipIntro,
                NoSplash,
                EmptyWorld,
                Window,
                FilePatching,
                NoPause,
                NoCb,
                ShowScriptErrors,
                NoLogs,
                EnableHt,
                HugePages,
                Malloc32,
                Malloc64,
                Additional
            };
        }

        /// <summary>
        /// Read the memory allocators available in the ArmA 3 Dll folder
        /// </summary>
        public void ReadAllocators(string arma3Path)
        {
            BindableCollection<ParameterValueItem> allocators32 = new BindableCollection<ParameterValueItem>();
            BindableCollection<ParameterValueItem> allocators64 = new BindableCollection<ParameterValueItem>();

            allocators32.Add(new ParameterValueItem("system", Resources.S_PARAMETER_MALLOC_SYSTEM));
            allocators64.Add(new ParameterValueItem("system", Resources.S_PARAMETER_MALLOC_SYSTEM));

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

            Malloc32.Values = allocators32;
            Malloc64.Values = allocators64;
        }
    }
}
