using Caliburn.Micro;
using _11thLauncher.Model.Game;

namespace _11thLauncher.Model.Parameter
{
    public class LaunchParameter
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Tooltip { get; set; }
        public ParameterType Type { get; set; }
        public Platform Platform { get; set; }
        public bool IsEnabled { get; set; }
        public BindableCollection<ParameterValueItem> Values { get; set; }
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
    }
}
