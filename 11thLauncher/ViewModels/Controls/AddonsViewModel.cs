using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.ViewModels.Controls
{
    public class AddonsViewModel : PropertyChangedBase, IHandle<AddonsLoadedMessage>, IHandle<ProfileLoadedMessage>
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;
        private readonly IAddonService _addonService;

        private BindableCollection<Preset> _presets = ApplicationConfig.AddonPresets; 
        private Preset _selectedPreset;
        private Addon _selectedAddon;

        #endregion

        public AddonsViewModel(IEventAggregator eventAggregator, IAddonService addonService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _addonService = addonService;
        }

        #region Properties

        public BindableCollection<Preset> Presets
        {
            get => _presets;
            set
            {
                _presets = value;
                NotifyOfPropertyChange();
            }
        }

        public Preset SelectedPreset
        {
            get => _selectedPreset;
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
            get => _addonService.Addons;
            set
            {
                _addonService.Addons = value;
                NotifyOfPropertyChange();
            }
        }

        public Addon SelectedAddon
        {
            get => _selectedAddon;
            set
            {
                if (!Equals(_selectedAddon, value))
                {
                    _selectedAddon = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        #endregion

        #region Message handling

        public void Handle(AddonsLoadedMessage message)
        {
            foreach (Addon addon in Addons)
            {
                addon.PropertyChanged += Addon_StatusChanged;
            }
        }

        public void Handle(ProfileLoadedMessage message)
        {
            SelectedPreset = null;

            //Reset all addons before loading
            foreach (Addon addon in Addons)
            {
                addon.SetStatus(false);
            }

            foreach (var addon in Addons)
            {
                var profileAddon = message.Addons.FirstOrDefault(addon.Equals);
                if (profileAddon != null)
                {
                    addon.SetStatus(profileAddon.IsEnabled);
                }
            }

            Addons = new BindableCollection<Addon>(Addons.OrderBy(a => message.Addons.IndexOf(a)));
            CollectionViewSource.GetDefaultView(Addons).Refresh();
        }

        #endregion

        #region UI Actions

        public void Presets_SelectionChanged()
        {
            if (SelectedPreset == null || Addons.Count == 0) return;

            foreach (var addon in Addons)
            {
                addon.SetStatus(SelectedPreset.Addons.Contains(addon.Name));
            }

            CollectionViewSource.GetDefaultView(Addons).Refresh();
            _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
        }

        private void Addon_StatusChanged(object sender, PropertyChangedEventArgs e)
        {
            _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
        }

        public void ContextToggleAddon(Addon addon)
        {
            if (addon != null)
            {
                addon.IsEnabled = !addon.IsEnabled;
            }
        }

        public void ContextBrowseAddon(Addon addon)
        {
            if (addon != null)
            {
                _addonService.BrowseAddonFolder(addon);
            }
        }

        public void ContextAddonLink(Addon addon)
        {
            if (addon != null)
            {
                _addonService.BrowseAddonWebsite(addon);
            }
        }

        public void ButtonMoveUp()
        {
            if (SelectedAddon == null) return;

            var index = Addons.IndexOf(SelectedAddon);
            if (index == 0) return;

            Addons.Move(index, index - 1);
            _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
        }

        public void ButtonMoveDown()
        {
            if (SelectedAddon == null) return;

            int index = Addons.IndexOf(SelectedAddon);
            if (index == Addons.Count - 1) return;

            Addons.Move(index, index + 1);
            _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
        }

        public void ButtonSelectAll()
        {
            foreach (var addon in Addons)
            {
                addon.SetStatus(true);
            }

            CollectionViewSource.GetDefaultView(Addons).Refresh();
            _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
        }

        public void ButtonSelectNone()
        {
            foreach (var addon in Addons)
            {
                addon.SetStatus(false);
            }

            CollectionViewSource.GetDefaultView(Addons).Refresh();
            _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
        }

        #endregion
    }
}