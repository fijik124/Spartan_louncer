﻿using Caliburn.Micro;
using Newtonsoft.Json;

namespace _11thLauncher.Models
{
    /// <summary>
    /// An game addon managed by the launcher. 
    /// </summary>
    public class Addon : PropertyChangedBase
    {
        private string _path;
        private bool _isEnabled;
        private string _name;

        /// <summary>
        /// Creates a new instance of the <see cref="Addon"/> class, with the specified name and initial status.
        /// </summary>
        public Addon(string path, string name, bool enabled = false)
        {
            Path = path;
            Name = name;
            IsEnabled = enabled;
        }

        /// <summary>
        /// Location of the addon in the file system.
        /// </summary>
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Current status of the addon.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Name of the addon folder.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Display name of the addon.
        /// </summary>
        [JsonIgnore]
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
