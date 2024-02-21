using Avalonia.Data.Converters;
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
    public class EventsGridCpsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            int cps = System.Convert.ToInt32(value);

            // Workspace > Globals > Default
            int threshold;
            if (HoloContext.Instance.Workspace.Cps != 0)
                threshold = HoloContext.Instance.Workspace.Cps;
            else if (HoloContext.Instance.GlobalsManager.Cps != 0)
                threshold = HoloContext.Instance.GlobalsManager.Cps;
            else
                threshold = 18; // Default

            if (cps > threshold)
            {
                var maxSteps = 5;
                var steps = Math.Min(maxSteps, Math.Max(0, Math.Ceiling((cps - threshold) / 2.0)) + 1);
                var red = (100 - (20 * maxSteps) + (steps * 20)) / 100;
                //return new SolidColorBrush(new Color(255, red, 0, 0), 1.0);
                return new SolidColorBrush(Colors.Red, red);
            }
            return Brushes.Transparent;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
