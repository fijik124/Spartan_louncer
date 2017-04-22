namespace _11thLauncher.Model.Addon
{
    /// <summary>
    /// An game addon managed by the launcher. 
    /// </summary>
    public class Addon : ObservableEntity
    {
        private bool _isEnabled;
        private string _name;

        /// <summary>
        /// Creates a new instance of the <see cref="Addon"/> class, with the specified name and initial status.
        /// </summary>
        public Addon(string name, bool enabled = false)
        {
            Name = name;
            IsEnabled = enabled;
        }

        /// <summary>
        /// Current status of the addon.
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Name of the addon folder.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Display name of the addon.
        /// </summary>
        public string DisplayName => Name.Replace("_", "__");

        /// <summary>
        /// Set the status of the addon without triggering an event.
        /// </summary>
        /// <param name="status">Status to set.</param>
        public void SetStatus(bool status)
        {
            _isEnabled = status;
        }

        public override bool Equals(object obj)
        {
            var item = obj as Addon;

            return item != null && Name.Equals(item.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
