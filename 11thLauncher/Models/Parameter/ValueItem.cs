namespace _11thLauncher.Models
{
    public class ValueItem
    {
        public string Value { get; set; }
        public string DisplayName { get; set; }

        public ValueItem(string value, string displayValue)
        {
            Value = value;
            DisplayName = displayValue;
        }
    }
}
