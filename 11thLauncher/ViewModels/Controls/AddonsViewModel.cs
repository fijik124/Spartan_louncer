using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Model;
using _11thLauncher.Model.Addon;

namespace _11thLauncher.ViewModels.Controls
{
    public class AddonsViewModel : PropertyChangedBase, IHandle<AddonsMessage>, IHandle<ProfileMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private BindableCollection<Preset> _presets;
        private Preset _selectedPreset;
        private BindableCollection<Addon> _addons = new BindableCollection<Addon>();
        private Addon _selectedAddon;

        public AddonsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            Presets = Constants.AddonPresets;
        }

        #region Message handling

        public void Handle(AddonsMessage message)
        {
            switch (message.Action)
            {
                case AddonsAction.Added:
                    _addons.AddRange(message.Addons);
                    foreach (Addon addon in Addons)
                    {
                        addon.PropertyChanged += Addon_StatusChanged;
                    }
                    break;

                default:
                    _eventAggregator.PublishOnUIThreadAsync(new ExceptionMessage(new ArgumentOutOfRangeException(nameof(message)), GetType().Name));
                    break;
            }
        }

        public void Handle(ProfileMessage message)
        {
            if (message.Action != ProfileAction.Loaded) return;

            SelectedPreset = null;

            var profile = message.Profile;
            if (profile != null)
            {
                Addons = profile.ProfileAddons;
            }
        }

        #endregion

        #region UI Actions

        public void Presets_SelectionChanged()
        {
            if (SelectedPreset == null) return;

            foreach (var addon in Addons)
            {
                addon.SetStatus(SelectedPreset.Addons.Contains(addon.Name));
            }

            CollectionViewSource.GetDefaultView(Addons).Refresh();
            _eventAggregator.PublishOnCurrentThread(new ProfileMessage(ProfileAction.Updated));
        }

        private void Addon_StatusChanged(object sender, PropertyChangedEventArgs e)
        {
            _eventAggregator.PublishOnCurrentThread(new ProfileMessage(ProfileAction.Updated));
        }

        public void ButtonMoveUp()
        {
            if (SelectedAddon != null)
            {
                var index = Addons.IndexOf(SelectedAddon);
                if (index != 0)
                {
                    Addons.Move(index, index - 1);
                }
                _eventAggregator.PublishOnCurrentThread(new ProfileMessage(ProfileAction.Updated));
            }
        }

        public void ButtonMoveDown()
        {
            if (SelectedAddon != null)
            {
                int index = Addons.IndexOf(SelectedAddon);
                if (index != Addons.Count - 1)
                {
                    Addons.Move(index, index + 1);
                }
            }
            _eventAggregator.PublishOnCurrentThread(new ProfileMessage(ProfileAction.Updated));
        }

        public void ButtonSelectAll()
        {
            foreach (var addon in Addons)
            {
                addon.SetStatus(true);
            }

            CollectionViewSource.GetDefaultView(Addons).Refresh();
            _eventAggregator.PublishOnCurrentThread(new ProfileMessage(ProfileAction.Updated));
        }

        public void ButtonSelectNone()
        {
            foreach (var addon in Addons)
            {
                addon.SetStatus(false);
            }

            CollectionViewSource.GetDefaultView(Addons).Refresh();
            _eventAggregator.PublishOnCurrentThread(new ProfileMessage(ProfileAction.Updated));
        }

        #endregion

        public BindableCollection<Preset> Presets
        {
            get { return _presets; }
            set
            {
                _presets = value;
                NotifyOfPropertyChange();
            }
        }

        public Preset SelectedPreset
        {
            get { return _selectedPreset; }
            set
            {
                if (!Equals(_selectedPreset, value))
                {
                    _selectedPreset = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public BindableCollection<Addon> Addons
        {
            get { return _addons; }
            set
            {
                _addons = value;
                NotifyOfPropertyChange();
            }
        }

        public Addon SelectedAddon
        {
            get { return _selectedAddon; }
            set
            {
                if (!Equals(_selectedAddon, value))
                {
                    _selectedAddon = value;
                    NotifyOfPropertyChange();
                }
            }
        }
    }
}