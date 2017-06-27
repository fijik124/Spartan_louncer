using System;
using System.Globalization;
using System.Windows.Data;

namespace _11thLauncher.Views.Util
{
    public class CultureDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? "" : string.IsNullOrWhiteSpace((string) value) ? "" : CultureInfo.GetCultureInfo((string) value).NativeName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
