using System;
using System.Linq;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Model;
using _11thLauncher.Model.Addon;
using _11thLauncher.Model.Profile;

namespace _11thLauncher.ViewModels.Controls
{
    public class ProfileSelectorViewModel : PropertyChangedBase, IHandle<ProfilesMessage>, IHandle<ProfileMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly AddonManager _addonManager;

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

        public ProfileSelectorViewModel(IEventAggregator eventAggregator, AddonManager addonManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _addonManager = addonManager;
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
                    _eventAggregator.PublishOnUIThreadAsync(new ExceptionMessage(new ArgumentOutOfRangeException()));
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
                    SelectedProfile.Write();
                    break;
                default:
                    _eventAggregator.PublishOnUIThreadAsync(new ExceptionMessage(new ArgumentOutOfRangeException()));
                    break;
            }
        }

        #endregion

        #region UI Actions

        public void Profiles_SelectionChanged()
        {
            //TODO
            if (SelectedProfile == null) return;

            SelectedProfile.Read(_addonManager.Addons, null, null); //TODO params,servinfo

            _eventAggregator.PublishOnCurrentThread(new ProfileMessage(ProfileAction.Loaded, SelectedProfile));

            ////Update the display with the profile
            //UpdateForProfile(); TODO
        }

        #endregion
    }
}
