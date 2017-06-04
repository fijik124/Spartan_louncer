using System;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Model;
using _11thLauncher.Model.Profile;

namespace _11thLauncher.ViewModels.Controls
{
    public class ProfilesViewModel : PropertyChangedBase, IHandle<ProfilesMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        private BindableCollection<UserProfile> _profiles = new BindableCollection<UserProfile>();
        public BindableCollection<UserProfile> Profiles
        {
            get => _profiles;
            set
            {
                _profiles = value;
                NotifyOfPropertyChange(() => Profiles);
            }
        }

        public ProfilesViewModel(IEventAggregator eventAggregator)
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
    }
}
