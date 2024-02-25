using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ameko.Services
{
    public class KeybindService
    {
        public static void TrySetKeybind(Control control, Dictionary<string, string> map, string key, ICommand command, object? param = null)
        {
            try
            {
                if (!map.TryGetValue(key, out string? value)) return;
                var gesture = KeyGesture.Parse(value);

                var binding = new KeyBinding { Gesture = gesture, Command = command };
                if (param != null) binding.CommandParameter = param;

                control.KeyBindings.Add(binding);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}
