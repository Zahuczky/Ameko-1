using Ameko.DataModels;
using Avalonia.Data;
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
    public class PasteOverFieldConverter : IValueConverter
    {
        private PasteOverField target;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (parameter == null || value == null) return null;

            var mask = (PasteOverField)parameter;
            target = (PasteOverField)value;
            return (mask & target) != 0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (parameter == null) return null;
            target ^= (PasteOverField)parameter;
            return target;
        }
    }
}
