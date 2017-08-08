using System.Runtime.Serialization;
using Caliburn.Micro;

namespace _11thLauncher.Models
{
    public class SelectionParameter : LaunchParameter
    {
        public SelectionParameter()
        {
            Type = ParameterType.Selection;
        }

        public BindableCollection<ValueItem> Values { get; set; }

        [DataMember(Order = 3)]
        public ValueItem SelectedValue { get; set; } //TODO

        public void CopyStatus(SelectionParameter parameter)
        {
            IsEnabled = parameter?.IsEnabled ?? false;
            SelectedValue = parameter?.SelectedValue;
        }
    }
}
