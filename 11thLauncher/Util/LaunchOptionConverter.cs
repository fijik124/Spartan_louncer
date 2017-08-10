using System;
using System.Globalization;
using System.Windows.Data;
using _11thLauncher.Models;
using EnumerationMember = _11thLauncher.Util.EnumerationExtension.EnumerationMember;

namespace _11thLauncher.Util
{
    public class LaunchOptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var launchOption = (LaunchOption?) value ?? 0;
            return new EnumerationMember
            {
                Value = launchOption,
                Description = launchOption.GetDescription()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.Parse(typeof(LaunchOption), ((EnumerationMember) value)?.Value.ToString() ?? string.Empty);
        }
    }
}
