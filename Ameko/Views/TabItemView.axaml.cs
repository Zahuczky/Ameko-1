using Ameko.ViewModels;
using AssCS;
using Avalonia.Controls;
using System.Collections.Generic;
using System.Linq;

namespace Ameko.Views
{
    public partial class TabView : UserControl
    {
        public TabView()
        {
            InitializeComponent();

            eventsGrid.SelectionChanged += (o, e) =>
            {
                List<Event> list = eventsGrid.SelectedItems.Cast<Event>().ToList();
                Event recent = (Event)eventsGrid.SelectedItem;
                if (DataContext != null)
                {
                    ((TabItemViewModel)DataContext).UpdateEventSelection(list, recent);
                }
            };
        }
    }
}
