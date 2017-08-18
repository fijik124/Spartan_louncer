using System;
using System.Windows;
using System.Windows.Controls;
using _11thLauncher.Models;
using _11thLauncher.Models.Parameter;

namespace _11thLauncher.Util
{
    public class ParameterTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (element == null || !(item is LaunchParameter)) return null;

            LaunchParameter parameter = (LaunchParameter) item;

            switch (parameter)
            {
                case BooleanParameter b:
                    return element.FindResource("ParameterBooleanTemplate") as DataTemplate;
                case SelectionParameter s:
                    return element.FindResource("ParameterSelectionTemplate") as DataTemplate;
                case TextParameter t:
                    return element.FindResource("ParameterTextTemplate") as DataTemplate;
                case NumericalParameter n:
                    return element.FindResource("ParameterNumericalTemplate") as DataTemplate;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
