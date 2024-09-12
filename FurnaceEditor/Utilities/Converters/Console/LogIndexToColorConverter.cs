using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FurnaceEditor.Utilities.Converters.Console
{

    public class LogIndexToColorConverter : IValueConverter
    {
        public Brush EvenColor { get; set; } = Brushes.LightGray;
        public Brush OddColor { get; set; } = Brushes.White;

        private int _index;


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _index++;
            if(_index % 2 == 0)
                return EvenColor;
            else
                return OddColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("This is a one-way conversion.");
        }
    }
}
