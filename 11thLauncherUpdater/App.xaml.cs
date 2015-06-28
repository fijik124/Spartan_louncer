using System.Windows;

namespace _11thLauncherUpdater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 2) {
                MainWindow mainWindow = new MainWindow(e.Args[0], e.Args[1]);
                mainWindow.Show();
            } else
            {
                Current.Shutdown();
            }
        }
    }
}
