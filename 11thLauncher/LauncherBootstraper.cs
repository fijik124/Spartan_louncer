using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using _11thLauncher.Model.Addon;
using _11thLauncher.Model.Profile;
using _11thLauncher.Model.Server;
using _11thLauncher.Model.Settings;
using _11thLauncher.ViewModels;

namespace _11thLauncher
{
    public class LauncherBootstraper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new SimpleContainer();
   
        public LauncherBootstraper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();
            _container.Singleton<IDialogCoordinator, DialogCoordinator>();

            _container.PerRequest<ShellViewModel>();
            _container.Singleton<SettingsManager>();
            _container.Singleton<ProfileManager>();
            _container.Singleton<AddonManager>();

            _container.Singleton<ServerManager>();
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = _container.GetInstance(service, key);
            if (instance != null)
                return instance;
            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
