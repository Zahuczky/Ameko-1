using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.Services
{
    public static class Helpers
    {
        /// <summary>
        /// Enumerate over the set flags in a [Flags] enum
        /// </summary>
        /// <param name="e">Enum to evaluate</param>
        /// <returns>Enumerable</returns>
        public static IEnumerable<Enum> GetFlags(this Enum e)
        {
            return Enum.GetValues(e.GetType()).Cast<Enum>().Where(e.HasFlag);
        }

        /// <summary>
        /// Pre-KeyDown event handler for Time text boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void TimeBox_PreKeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (sender == null) return;
            var box = (TextBox)sender;
            if (box.Text == null) return;

            // TODO: cut/copy/insert handling

            // Movement Keys
            switch (e.Key)
            {
                case Avalonia.Input.Key.Back:
                case Avalonia.Input.Key.Left:
                case Avalonia.Input.Key.Down:
                    box.CaretIndex -= 1;
                    e.Handled = true;
                    return;
                case Avalonia.Input.Key.Home:
                    box.CaretIndex = 0;
                    e.Handled = true;
                    return;
                case Avalonia.Input.Key.End:
                    box.CaretIndex = box.Text.Length;
                    e.Handled = true;
                    return;
                case Avalonia.Input.Key.Right:
                case Avalonia.Input.Key.Up:
                    box.CaretIndex += 1;
                    e.Handled = true;
                    return;
                default:
                    break;
            }

            // Discard if at the end
            if (box.CaretIndex >= box.Text.Length)
            {
                e.Handled = true;
                return;
            }

            // Discard if invalid character
            var key = Convert.ToChar(e.KeySymbol ?? "a");
            if ((key < '0' || key > '9') && key != ';' && key != '.' && key != ',' && key != ':')
            {
                e.Handled = true;
                return;
            }

            // Move forward if at punctuation
            var next = box.Text?[box.CaretIndex] ?? '-';
            if (next == ':' || next == '.' || next == ',')
            {
                box.CaretIndex += 1;
            }

            // Nothing more needed for punctuation
            if (key == ';' || key == '.' || key == ',' || key == ':')
            {
                e.Handled = true;
                return;
            }

            box.Text = box.Text?.Remove(box.CaretIndex, 1)
                .Insert(box.CaretIndex, Convert.ToString(key));
            box.CaretIndex += 1;
            e.Handled = true;
            return;
        }
    }
}
