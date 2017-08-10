namespace _11thLauncher.Models
{
    public class TextParameter : LaunchParameter
    {
        private string _value;

        public TextParameter()
        {
            Type = ParameterType.Text;
        }

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
    }
}
