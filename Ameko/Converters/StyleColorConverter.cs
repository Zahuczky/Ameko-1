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
    public class StyleColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return null;
            bool opacity = false;
            if (parameter != null) opacity = System.Convert.ToBoolean(parameter);

            AssCS.Color assColor = (AssCS.Color)value;
            Color color;
            if (opacity)
                color = new Color((byte)(255 - assColor.Alpha), (byte)assColor.Red, (byte)assColor.Green, (byte)assColor.Blue);
            else
                color = new Color(255, (byte)assColor.Red, (byte)assColor.Green, (byte)assColor.Blue);
            return new SolidColorBrush(color);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
