using System;
using System.Linq;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.ViewModels.Controls
{
    public class ProfileSelectorViewModel : PropertyChangedBase, IHandle<ProfilesMessage>, IHandle<ProfileMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IAddonService _addonService;
        private readonly IProfileService _profileService;
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
            IProfileService profileService, ParameterManager parameterManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _addonService = addonService;
            _profileService = profileService;
            _parameterManager = parameterManager;
        }

        #region Message handling

        public void Handle(ProfilesMessage message)
        {
            switch (message.Action)
            {
                case ProfilesAction.Added:
                    bool initialLoad = Profiles.Count == 0;
                    Profiles.AddRange(message.Profiles);
                    if (initialLoad)
                    {
                        SelectedProfile = Profiles.First(p => p.IsDefault);
                        Profiles_SelectionChanged();
                    }
                    break;

                case ProfilesAction.Deleted:
                    Profiles.RemoveRange(message.Profiles);
                    break;

                default:
                    _eventAggregator.PublishOnUIThreadAsync(new ExceptionMessage(new ArgumentOutOfRangeException(nameof(message.Action)), GetType().Name));
                    break;
            }
        }

        public void Handle(ProfileMessage message)
        {
            switch (message.Action)
            {
                case ProfileAction.Loaded:
                    break;
                case ProfileAction.Updated:
                    //_profileManager.WriteProfile(SelectedProfile, _addonService.GetAddons(), _parameterManager.Parameters, _launchManager.GameConfig); TODO
                    break;
                default:
                    _eventAggregator.PublishOnUIThreadAsync(new ExceptionMessage(new ArgumentOutOfRangeException(nameof(message.Action)), GetType().Name));
                    break;
            }
        }

        #endregion

        #region UI Actions

        public void Profiles_SelectionChanged()
        {
            //TODO
            if (SelectedProfile == null) return;

            BindableCollection<Addon> addons;
            BindableCollection<LaunchParameter> parameters;
            GameConfig gameConfig;
            //_profileManager.ReadProfile(SelectedProfile, out addons, out parameters, out gameConfig); TODO

            //_eventAggregator.PublishOnCurrentThread(new LoadProfileMessage(SelectedProfile, addons, parameters, gameConfig)); TODO

            ////Update the display with the profile
            //UpdateForProfile(); TODO
        }

        #endregion
    }
}
