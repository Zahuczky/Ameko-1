using Ameko.Services;
using Ameko.ViewModels;
using Avalonia.Controls;

namespace Ameko.Views
{
    public partial class EventsView : UserControl
    {
        public EventsView()
        {
            this.DataContext = new EventsViewModel();
            InitializeComponent();
        }
    }
}
