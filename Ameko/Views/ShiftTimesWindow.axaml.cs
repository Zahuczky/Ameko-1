using Ameko.Services;
using Avalonia.Controls;
using Avalonia.Input;

namespace Ameko.Views
{
    public partial class ShiftTimesWindow : Window
    {
        public ShiftTimesWindow()
        {
            InitializeComponent();

            timeBox.AddHandler(InputElement.KeyDownEvent, Helpers.TimeBox_PreKeyDown, Avalonia.Interactivity.RoutingStrategies.Tunnel);
        }
    }
}
