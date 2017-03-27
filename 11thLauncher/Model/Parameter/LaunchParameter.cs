using Caliburn.Micro;

namespace _11thLauncher.Model.Parameter
{
    public class LaunchParameter
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Tooltip { get; set; }
        public ParameterType ParameterType { get; set; }
        public bool IsEnabled { get; set; }
        public BindableCollection<ParameterValueItem> Values { get; set; }

        public LaunchParameter() { }

        public LaunchParameter(string name, string displayName, string tooltip, ParameterType parameterType)
        {
            Name = name;
            DisplayName = displayName;
            Tooltip = tooltip;
            ParameterType = parameterType;
        }
        public LaunchParameter(string name, string displayName, string tooltip, ParameterType parameterType, BindableCollection<ParameterValueItem> parameterValueItems)
        {
            Name = name;
            DisplayName = displayName;
            Tooltip = tooltip;
            ParameterType = parameterType;
            Values = parameterValueItems;
        }
    }
}
