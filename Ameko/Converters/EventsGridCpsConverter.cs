using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.Converters
{
    public class EventsGridCpsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            int cps = System.Convert.ToInt32(value);
            // TODO: Load threshold from workspace / config
            if (cps > 18)
            {
                return new SolidColorBrush(Colors.Red, 1.0);
            }
            return Brushes.White;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
