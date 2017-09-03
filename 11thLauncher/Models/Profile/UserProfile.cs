using System;
using System.Runtime.Serialization;
using Caliburn.Micro;

namespace _11thLauncher.Models
{
    [DataContract]
    public class UserProfile : PropertyChangedBase
    {
        #region Fields

        private string _name;
        private bool _isDefault;

        #endregion

        public UserProfile(string name, bool isDefault = false)
        {
            Id = Guid.NewGuid();
            Name = name;
            IsDefault = isDefault;
        }

        public UserProfile(Guid guid, string name, bool isDefault = false)
        {
            Id = guid;
            Name = name;
            IsDefault = isDefault;
        }

        #region Properties

        [DataMember]
        public Guid Id { get; }

        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => DisplayName);
            }
        }

        public string DisplayName => IsDefault ? "★ " + Name : Name;

        public bool IsDefault
        {
            get => _isDefault;
            set
            {
                _isDefault = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => DisplayName);
            }
        }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            var item = obj as UserProfile;

            return item != null && Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion
    }
}
