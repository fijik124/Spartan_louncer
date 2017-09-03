using System.Runtime.Serialization;

namespace _11thLauncher.Models
{
    public class NumericalParameter : LaunchParameter
    {
        #region Fields

        private int? _minValue;
        private int? _maxValue;
        private int _value;

        #endregion

        public NumericalParameter()
        {
            Type = ParameterType.Numerical;
        }

        #region Properties

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

        [DataMember(Order = 4)]
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

        #endregion

        public override void CopyStatus(LaunchParameter parameter)
        {
            base.CopyStatus(parameter);
            _value = ((NumericalParameter) parameter)?.Value ?? MinValue;
        }
    }
}