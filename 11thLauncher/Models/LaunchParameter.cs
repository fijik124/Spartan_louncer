using System.Runtime.Serialization;
using Caliburn.Micro;

namespace _11thLauncher.Models
{
    [DataContract]
    public class LaunchParameter : PropertyChangedBase //TODO complete class
    {
        private string _name;
        private ParameterPlatform _platform;

        public LaunchParameter() { }

        public LaunchParameter(string name, string displayName, string tooltip, ParameterType parameterType, ParameterPlatform platform = ParameterPlatform.Any)
        {
            Name = name;
            DisplayName = displayName;
            Tooltip = tooltip;
            Type = parameterType;
            Platform = platform;
        }

        public LaunchParameter(string name, string displayName, string tooltip, ParameterType parameterType, BindableCollection<ParameterValueItem> parameterValueItems, ParameterPlatform platform = ParameterPlatform.Any)
        {
            Name = name;
            DisplayName = displayName;
            Tooltip = tooltip;
            Type = parameterType;
            Values = parameterValueItems;
            Platform = platform;
        }

        /// <summary>
        /// Name of the launch parameter.
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

        public string LegacyName { get; set; }

        public string DisplayName { get; set; }

        public string Tooltip { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        public ParameterType Type { get; set; }

        /// <summary>
        /// Platform of the launch parameter.
        /// </summary>
        [DataMember]
        public ParameterPlatform Platform
        {
            get => _platform;
            set
            {
                _platform = value;
                NotifyOfPropertyChange();
            }
        }

        public BindableCollection<ParameterValueItem> Values { get; set; }

        [DataMember]
        public ParameterValueItem SelectedValue { get; set; } //TODO

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
