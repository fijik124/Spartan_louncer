using System.Linq;
using System.Runtime.Serialization;
using Caliburn.Micro;

namespace _11thLauncher.Models
{
    public class SelectionParameter : LaunchParameter
    {
        #region Fields

        private BindableCollection<ValueItem> _values;
        private ValueItem _selectValueItem;

        #endregion

        public SelectionParameter()
        {
            Type = ParameterType.Selection;
        }

        #region Properties

        public BindableCollection<ValueItem> Values
        {
            get => _values;
            set
            {
                _values = value;
                NotifyOfPropertyChange();
            }
        }

        [DataMember(Order = 4)]
        public ValueItem SelectedValue
        {
            get => _selectValueItem;
            set
            {
                _selectValueItem = value;
                NotifyOfPropertyChange();
            }
        }

        public override string LaunchString => SelectedValue != null ? Name + SelectedValue.Value : string.Empty;

        #endregion

        public override void CopyStatus(LaunchParameter parameter)
        {
            base.CopyStatus(parameter);
            if (Values != null && Values.Count != 0)
            {
                _selectValueItem = Values.FirstOrDefault(p => p.Value.Equals(((SelectionParameter)parameter)?.SelectedValue?.Value)) ?? Values.FirstOrDefault();
            }
        }
    }
}
