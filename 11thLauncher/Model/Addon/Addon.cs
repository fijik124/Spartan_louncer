using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _11thLauncher.Model.Addon
{
    public class Addon : INotifyPropertyChanged
    {
        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string DisplayName => Name.Replace("_", "__");

        public Addon(string name, bool enabled = false)
        {
            Name = name;
            IsEnabled = enabled;
        }

        /// <summary>
        /// Set the status of the addon without triggering an event
        /// </summary>
        /// <param name="status">Status to set</param>
        public void SetStatus(bool status)
        {
            _isEnabled = status;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
