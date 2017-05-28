using System.Runtime.Serialization;
using Caliburn.Micro;
using _11thLauncher.Model.Game;

namespace _11thLauncher.Model.Parameter
{
    [DataContract]
    public class LaunchParameter : PropertyChangedBase //TODO complete class
    {
        private string _name;
        public string DisplayName { get; set; }
        public string Tooltip { get; set; }
        public ParameterType Type { get; set; }
        private Platform _platform;
        [DataMember]
        public bool IsEnabled { get; set; }
        public BindableCollection<ParameterValueItem> Values { get; set; }
        [DataMember]
        public ParameterValueItem SelectedValue { get; set; } //TODO

        public LaunchParameter() { }

        public LaunchParameter(string name, string displayName, string tooltip, ParameterType parameterType, Platform platform = Platform.Any)
        {
            Name = name;
            DisplayName = displayName;
            Tooltip = tooltip;
            Type = parameterType;
            Platform = platform;
        }
        public LaunchParameter(string name, string displayName, string tooltip, ParameterType parameterType, BindableCollection<ParameterValueItem> parameterValueItems, Platform platform = Platform.Any)
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
            get { return _name; }
            set
            {
                _name = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Platform of the launch parameter.
        /// </summary>
        [DataMember]
        public Platform Platform
        {
            get { return _platform; }
            set
            {
                _platform = value;
                NotifyOfPropertyChange();
            }
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
