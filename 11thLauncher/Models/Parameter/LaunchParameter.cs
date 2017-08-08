using System.Runtime.Serialization;
using Caliburn.Micro;

namespace _11thLauncher.Models
{
    [DataContract]
    public class LaunchParameter : PropertyChangedBase //TODO complete class
    {
        private string _name;
        private ParameterPlatform _platform;
        private bool _isEnabled;

        public LaunchParameter()
        {
            Platform = ParameterPlatform.Any;
        }

        /// <summary>
        /// Name of the launch parameter.
        /// </summary>
        [DataMember(Order = 0)]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyOfPropertyChange();
            }
        }

        public string LegacyName { get; set; }

        public string DisplayName { get; set; }

        public string Tooltip { get; set; }

        [DataMember(Order = 1)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange();
            }
        }

        public ParameterType Type { get; set; }

        /// <summary>
        /// Platform of the launch parameter.
        /// </summary>
        [DataMember(Order = 2)]
        public ParameterPlatform Platform
        {
            get => _platform;
            set
            {
                _platform = value;
                NotifyOfPropertyChange();
            }
        }
        
        public void CopyStatus(LaunchParameter parameter)
        {
            IsEnabled = parameter?.IsEnabled ?? false;
        }

        public override bool Equals(object obj)
        {
            var item = obj as LaunchParameter;

            return item != null && Name.Equals(item.Name) && Platform.Equals(item.Platform);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() * Platform.GetHashCode();
        }
    }
}
