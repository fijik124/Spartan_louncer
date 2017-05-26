using System;
using System.Linq;
using Caliburn.Micro;
using _11thLauncher.Game;
using _11thLauncher.Messages;
using _11thLauncher.Model;
using _11thLauncher.Model.Addons;
using _11thLauncher.Model.Game;
using _11thLauncher.Model.Parameter;
using _11thLauncher.Model.Profile;

namespace _11thLauncher.ViewModels.Controls
{
    public class ProfileSelectorViewModel : PropertyChangedBase, IHandle<ProfilesMessage>, IHandle<ProfileMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly AddonManager _addonManager;
        private readonly ProfileManager _profileManager;
        private readonly ParameterManager _parameterManager;
        private readonly LaunchManager _launchManager;

        private BindableCollection<UserProfile> _profiles = new BindableCollection<UserProfile>();
        public BindableCollection<UserProfile> Profiles
        {
            get { return _profiles; }
            set
            {
                _profiles = value;
                NotifyOfPropertyChange();
            }
        }

        private UserProfile _selectedProfile;
        public UserProfile SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                if (!Equals(_selectedProfile, value))
                {
                    _selectedProfile = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public ProfileSelectorViewModel(IEventAggregator eventAggregator, AddonManager addonManager, 
            ProfileManager profileManager, ParameterManager parameterManager, LaunchManager launchManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _addonManager = addonManager;
            _profileManager = profileManager;
            _parameterManager = parameterManager;
            _launchManager = launchManager;
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
                    _profileManager.WriteProfile(SelectedProfile, _addonManager.Addons, _parameterManager.Parameters, _launchManager.GameConfig);
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
            _profileManager.ReadProfile(SelectedProfile, out addons, out parameters, out gameConfig);

            _eventAggregator.PublishOnCurrentThread(new LoadProfileMessage(SelectedProfile, addons, parameters, gameConfig));

            ////Update the display with the profile
            //UpdateForProfile(); TODO
        }

        #endregion
    }
}
