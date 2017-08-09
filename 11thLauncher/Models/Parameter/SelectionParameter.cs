using System.Linq;
using System.Runtime.Serialization;
using Caliburn.Micro;

namespace _11thLauncher.Models
{
    public class SelectionParameter : LaunchParameter
    {
        private ValueItem _selectValueItem;

        public SelectionParameter()
        {
            Type = ParameterType.Selection;
        }

        public BindableCollection<ValueItem> Values { get; set; }

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

        public override void CopyStatus(LaunchParameter parameter)
        {
            base.CopyStatus(parameter);
            _selectValueItem = Values.FirstOrDefault(p => p.Value.Equals(((SelectionParameter)parameter)?.SelectedValue?.Value)) ?? Values.FirstOrDefault();
        }
    }
}
