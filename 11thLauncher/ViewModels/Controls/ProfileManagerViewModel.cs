using System.Linq;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.ViewModels.Controls
{
    public class ProfileManagerViewModel : PropertyChangedBase, IHandle<ProfileAddedMessage>
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;
        private readonly ISettingsService _settingsService;
        private readonly IProfileService _profileService;
        private BindableCollection<UserProfile> _profiles = new BindableCollection<UserProfile>();
        private UserProfile _selectedProfile;
        private UserProfile _managedProfile;

        #endregion

        public ProfileManagerViewModel(IEventAggregator eventAggregator, ISettingsService settingsService, IProfileService profileService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _settingsService = settingsService;
            _profileService = profileService;
        }

        #region Properties

        public BindableCollection<UserProfile> Profiles
        {
            get => _profiles;
            set
            {
                _profiles = value;
                NotifyOfPropertyChange();
            }
        }

        public UserProfile SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                _selectedProfile = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => AllowFavoriteProfile);
                NotifyOfPropertyChange(() => AllowDeleteProfile);
            }
        }

        public UserProfile ManagedProfile
        {
            get => _managedProfile;
            set
            {
                _managedProfile = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => AllowCreateProfile);
                NotifyOfPropertyChange(() => AllowSaveProfile);
            }
        }

        public bool AllowCreateProfile => ManagedProfile == null;

        public bool AllowSaveProfile => ManagedProfile != null;

        public bool AllowFavoriteProfile => ManagedProfile == null && SelectedProfile != null && !SelectedProfile.IsDefault;

        public bool AllowDeleteProfile => AllowFavoriteProfile;

        #endregion

        #region Message handling

        public void Handle(ProfileAddedMessage message)
        {
            Profiles.AddRange(message.Profiles.Where(p => !Profiles.Contains(p)));
        }

        #endregion

        #region UI Actions

        public void ButtonCreateProfile()
        {
            ManagedProfile = new UserProfile(Resources.Strings.ResourceManager.GetString("S_NEW_PROFILE_NAME"));
        }

        public void ButtonMarkFavorite()
        {
            if (SelectedProfile == null) return;
            foreach (UserProfile userProfile in Profiles)
            {
                userProfile.IsDefault = false;
            }
            SelectedProfile.IsDefault = true;
            NotifyOfPropertyChange(() => AllowFavoriteProfile);
            NotifyOfPropertyChange(() => AllowDeleteProfile);

            _settingsService.DefaultProfileId = SelectedProfile.Id;
            _settingsService.Write();
        }

        public void ButtonDeleteProfile()
        {
            if (SelectedProfile == null) return;
            if (SelectedProfile.IsDefault) return;

            _profileService.DeleteProfile(SelectedProfile);

            _settingsService.UserProfiles.Remove(SelectedProfile);
            _settingsService.Write();

            _eventAggregator.PublishOnCurrentThread(new ProfileDeletedMessage(SelectedProfile));
            Profiles.Remove(SelectedProfile);
        }

        public void ButtonSaveProfile()
        {
            if (ManagedProfile == null) return;
            Profiles.Add(ManagedProfile);

            _settingsService.UserProfiles.Add(ManagedProfile);
            _settingsService.Write();
            _profileService.Write(ManagedProfile, null, null, null);

            _eventAggregator.PublishOnCurrentThread(new ProfileAddedMessage(new BindableCollection<UserProfile> { ManagedProfile }));

            ManagedProfile = null;
        }

        #endregion
    }
}