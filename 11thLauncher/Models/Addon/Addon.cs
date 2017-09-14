using System.Runtime.Serialization;
using Caliburn.Micro;
using Newtonsoft.Json;

namespace _11thLauncher.Models
{
    /// <summary>
    /// An game addon managed by the launcher. 
    /// </summary>
    [DataContract]
    public class Addon : PropertyChangedBase
    {
        #region Fields

        private string _path;
        private string _name;
        private bool _isEnabled;
        private AddonMetaData _metaData;

        #endregion

        public Addon() {}

        /// <summary>
        /// Creates a new instance of the <see cref="Addon"/> class, with the specified name and initial status.
        /// </summary>
        public Addon(string path, string name, bool enabled = false)
        {
            Path = path;
            Name = name;
            IsEnabled = enabled;
        }

        #region Properties

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
        /// Name of the addon folder.
        /// </summary>
        [DataMember]
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
        /// Current status of the addon.
        /// </summary>
        [DataMember]
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
        /// Addon metadata, taken from the mod.cpp file.
        /// </summary>
        [JsonIgnore]
        public AddonMetaData MetaData
        {
            get => _metaData;
            set
            {
                _metaData = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => IsLinkAvailable);
            }
        }

        /// <summary>
        /// Indicates if a link to the addon website is available.
        /// </summary>
        [JsonIgnore]
        public bool IsLinkAvailable => !string.IsNullOrEmpty(MetaData?.Action);


        #endregion

        #region Methods

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

        #endregion
    }
}
