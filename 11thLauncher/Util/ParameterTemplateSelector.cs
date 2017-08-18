using System;
using System.Windows;
using System.Windows.Controls;
using _11thLauncher.Models;

namespace _11thLauncher.Util
{
    public class ParameterTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (element == null || !(item is LaunchParameter)) return null;

            LaunchParameter parameter = (LaunchParameter) item;

            switch (parameter.Type)
            {
                case ParameterType.Boolean:
                    return element.FindResource("ParameterBooleanTemplate") as DataTemplate;
                case ParameterType.Selection:
                    return element.FindResource("ParameterSelectionTemplate") as DataTemplate;
                case ParameterType.Text:
                    return element.FindResource("ParameterTextTemplate") as DataTemplate;
                case ParameterType.Numerical:
                    return element.FindResource("ParameterNumericalTemplate") as DataTemplate;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
