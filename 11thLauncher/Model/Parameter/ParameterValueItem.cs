namespace _11thLauncher.Model.Parameter
{
    public class ParameterValueItem
    {
        public string DisplayName { get; set; }
        public string Value { get; set; }

        public ParameterValueItem(string displayValue, string value)
        {
            DisplayName = displayValue;
            Value = value;
        }
    }
}
