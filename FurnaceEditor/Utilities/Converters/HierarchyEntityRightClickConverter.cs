using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using FurnaceEditor.ViewModels.Components;

namespace FurnaceEditor.Utilities.Converters
{
    public class HierarchyEntityRightClickConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                return (values[0] as Entity, values[1] as StackPanel);
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
