using System.Windows;

namespace _11thLauncher.Updater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 3) {
                MainWindow mainWindow = new MainWindow(e.Args[0], e.Args[1], e.Args[2]);
                mainWindow.Show();
            } else
            {
                Current.Shutdown();
            }
        }
    }
}
