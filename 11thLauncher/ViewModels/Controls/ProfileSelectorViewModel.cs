using System;
using System.Linq;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.ViewModels.Controls
{
    public class ProfileSelectorViewModel : PropertyChangedBase, IHandle<ProfileAddedMessage>, IHandle<ProfileDeletedMessage>, 
        IHandle<LoadProfileMessage>, IHandle<SaveProfileMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IAddonService _addonService;
        private readonly IProfileService _profileService;
        private readonly ILauncherService _launcherService;
        private readonly ParameterManager _parameterManager;

        private BindableCollection<UserProfile> _profiles = new BindableCollection<UserProfile>();
        public BindableCollection<UserProfile> Profiles
        {
            get => _profiles;
            set
            {
                _profiles = value;
                NotifyOfPropertyChange();
            }
        }

        private UserProfile _selectedProfile;
        public UserProfile SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                if (!Equals(_selectedProfile, value))
                {
                    _selectedProfile = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public ProfileSelectorViewModel(IEventAggregator eventAggregator, IAddonService addonService,
            IProfileService profileService, ILauncherService launcherService, ParameterManager parameterManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _addonService = addonService;
            _profileService = profileService;
            _launcherService = launcherService;
            _parameterManager = parameterManager;
        }

        #region Message handling

        public void Handle(ProfileAddedMessage message)
        {
            bool initialLoad = Profiles.Count == 0;
            Profiles.AddRange(message.Profiles);
            if (!initialLoad) return;

            SelectedProfile = Profiles.First(p => p.IsDefault);
            Profiles_SelectionChanged();
        }

        public void Handle(ProfileDeletedMessage message)
        {
            if (SelectedProfile.Equals(message.Profile))
            {
                SelectedProfile = Profiles.FirstOrDefault(p => !p.Equals(message.Profile));
            }

            Profiles.Remove(message.Profile);
        }

        public void Handle(SaveProfileMessage message)
        {
            _profileService.Write(SelectedProfile, _addonService.GetAddons(), _parameterManager.Parameters, _launcherService.LaunchSettings);
        }

        public void Handle(LoadProfileMessage message)
        {
            _profileService.Read(message.Profile, out BindableCollection<Addon> addons, 
                out BindableCollection<LaunchParameter> parameters, out LaunchSettings launchSettings);

            _eventAggregator.PublishOnCurrentThread(new ProfileLoadedMessage(message.Profile, addons, parameters, launchSettings));
        }

        #endregion

        #region UI Actions

        public void Profiles_SelectionChanged()
        {
            if (SelectedProfile == null) return;

            _profileService.Read(SelectedProfile, out BindableCollection<Addon> addons, out BindableCollection<LaunchParameter> parameters, 
                out LaunchSettings launchSettings);

            _eventAggregator.PublishOnCurrentThread(new ProfileLoadedMessage(SelectedProfile, addons, parameters, launchSettings));
        }

        #endregion
    }
}
