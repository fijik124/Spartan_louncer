using System.Runtime.Serialization;

namespace _11thLauncher.Models
{
    public class NumericalParameter : LaunchParameter
    {
        private int? _minValue;
        private int? _maxValue;
        private int _value;

        public int MinValue
        {
            get => _minValue ?? 0;
            set => _minValue = value;
        }

        public int MaxValue
        {
            get => _maxValue ?? int.MaxValue;
            set => _maxValue = value;
        }

        [DataMember(Order = 3)]
        public int Value
        {
            get => _value;
            set
            {
                _value = (value > MaxValue) ? MaxValue : value;
                NotifyOfPropertyChange();
            }
        }

        public override string LaunchString => Name + Value;

        public override void CopyStatus(LaunchParameter parameter)
        {
            base.CopyStatus(parameter);
            _value = ((NumericalParameter) parameter)?.Value ?? MinValue;
        }
    }
}