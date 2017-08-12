using System.Runtime.Serialization;

namespace _11thLauncher.Models
{
    public class TextParameter : LaunchParameter
    {
        private string _value;

        public TextParameter()
        {
            Type = ParameterType.Text;
        }

        [DataMember(Order = 4)]
        public string Value
        {
            get => _value ?? string.Empty;
            set
            {
                _value = value;
                NotifyOfPropertyChange();
            }
        }

        public override string LaunchString => Value.Trim();

        public void SetStatus(bool enabled, string value)
        {
            base.SetStatus(enabled);
            _value = value;
        }
    }
}
