using System;
using System.ComponentModel;
using System.Linq;

namespace _11thLauncher.Model
{
    public static class ExtensionMethods
    {
        public static string GetDescription(this Enum value)
        {
            var descriptionAttribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault() as DescriptionAttribute;


            return descriptionAttribute != null
                ? Resources.Strings.ResourceManager.GetString(descriptionAttribute.Description)
                : value.ToString();
        }
    }
}
