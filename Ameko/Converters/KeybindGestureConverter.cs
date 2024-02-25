using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Media;
using Holo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.Converters
{
    public class KeybindGestureConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string? key = parameter as string;
            if (key == null) return null;
            var map = HoloContext.Instance.ConfigurationManager.KeybindsMap;
            if (map.ContainsKey(key)) return KeyGesture.Parse(map[key]);
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
