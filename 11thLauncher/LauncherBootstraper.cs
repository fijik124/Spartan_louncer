using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using _11thLauncher.Model.Profile;
using _11thLauncher.Models;
using _11thLauncher.Services;
using _11thLauncher.Services.Contracts;
using _11thLauncher.ViewModels;
using _11thLauncher.ViewModels.Controls;

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
            //Application managers
            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();
            _container.Singleton<IDialogCoordinator, DialogCoordinator>();

            //ViewModels
            _container.Singleton<ShellViewModel>();
            _container.Singleton<StatusbarViewModel>();
            _container.Singleton<ProfileSelectorViewModel>();
            _container.Singleton<AddonsViewModel>();
            _container.Singleton<GameViewModel>();
            _container.Singleton<ServerStatusViewModel>();
            _container.Singleton<RepositoryStatusViewModel>();
            _container.Singleton<ParametersViewModel>();
            _container.Singleton<ServerQueryViewModel>();
            _container.Singleton<ProfileManagerViewModel>();
            _container.Singleton<SettingsViewModel>();
            _container.Singleton<AboutViewModel>();

            //TODO
            _container.Singleton<ProfileManager>();
            _container.Singleton<ParameterManager>();

            //Services
            _container.Singleton<ISettingsService, SettingsService>();
            _container.Singleton<IServerQueryService, ServerQueryService>();
            _container.Singleton<IAddonService, AddonService>();
            _container.Singleton<IAddonSyncService, Arma3SyncService>();
            _container.Singleton<IGameLauncherService, GameLauncherService>();
            _container.Singleton<IUpdaterService, UpdaterService>();
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
