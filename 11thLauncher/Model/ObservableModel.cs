using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _11thLauncher.Model
{
    public class ObservableEntity : INotifyPropertyChanged
    {
        /// <summary>
        /// Property changed event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise a change in a property of the object.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
