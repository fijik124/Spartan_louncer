using Caliburn.Micro;

namespace _11thLauncher.Model.Parameter
{
    public class LaunchParameter
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Tooltip { get; set; }
        public ParameterType Type { get; set; }
        public ParameterPlatform Platform { get; set; }
        public bool IsEnabled { get; set; }
        public BindableCollection<ParameterValueItem> Values { get; set; }

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
    }
}
