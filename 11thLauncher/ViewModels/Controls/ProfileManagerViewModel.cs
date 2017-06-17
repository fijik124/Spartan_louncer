using System;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Model;
using _11thLauncher.Models;

namespace _11thLauncher.ViewModels.Controls
{
    public class ProfileManagerViewModel : PropertyChangedBase, IHandle<ProfilesMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private BindableCollection<UserProfile> _profiles = new BindableCollection<UserProfile>();
        private UserProfile _selectedProfile;
        private UserProfile _managedProfile;

        public ProfileManagerViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        #region Message handling

        public void Handle(ProfilesMessage message)
        {
            switch (message.Action)
            {
                case ProfilesAction.Added:
                    Profiles.AddRange(message.Profiles);
                    break;

                case ProfilesAction.Deleted:
                    Profiles.RemoveRange(message.Profiles);

                    break;

                default:
                    _eventAggregator.PublishOnUIThreadAsync(new ExceptionMessage(new ArgumentOutOfRangeException(nameof(message.Action)), GetType().Name));
                    break;
            }
        }

        #endregion

        #region UI Actions

        public void ButtonCreateProfile()
        {
            ManagedProfile = new UserProfile(Resources.Strings.ResourceManager.GetString("S_NEW_PROFILE_NAME"));
        }

        public void ButtonEditProfile()
        {
            if (SelectedProfile == null) return;
            ManagedProfile = SelectedProfile;
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

            //TODO save, set favorite in settings
        }

        public void ButtonDeleteProfile()
        {
            if (SelectedProfile == null) return;
            if (SelectedProfile.IsDefault) return;

            Profiles.Remove(SelectedProfile);
            //TODO update
        }


        public void ButtonSaveProfile()
        {
            if (ManagedProfile == null) return;
            if (!Profiles.Contains(ManagedProfile))
            {
                Profiles.Add(ManagedProfile);
            }
            //TODO save

            ManagedProfile = null;
        }

        #endregion

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
                NotifyOfPropertyChange(() => AllowEditProfile);
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
                NotifyOfPropertyChange(() => AllowEditProfile);
                NotifyOfPropertyChange(() => AllowSaveProfile);
            }
        }

        public bool AllowCreateProfile => ManagedProfile == null;

        public bool AllowEditProfile => ManagedProfile == null && SelectedProfile != null;

        public bool AllowSaveProfile => ManagedProfile != null;

        public bool AllowFavoriteProfile => AllowEditProfile  && !SelectedProfile.IsDefault;

        public bool AllowDeleteProfile => AllowFavoriteProfile;
    }
}
