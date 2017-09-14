using System.Runtime.Serialization;

namespace _11thLauncher.Models
{
    [DataContract]
    public class ValueItem
    {
        public ValueItem(string value, string displayValue)
        {
            Value = value;
            DisplayName = displayValue;
        }

        [DataMember]
        public string Value { get; set; }
        public string DisplayName { get; }
    }
}
