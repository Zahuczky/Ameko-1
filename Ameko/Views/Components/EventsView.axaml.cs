using Ameko.Services;
using Ameko.ViewModels;
using AssCS;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ameko.Views.Components
{
    public partial class EventsView : UserControl
    {
        public EventsView()
        {
            this.DataContext = new EventsViewModel();
            InitializeComponent();

            eventsGrid.SelectionChanged += (o, e) => {
                List<Event> list = eventsGrid.SelectedItems.Cast<Event>().ToList();
                Event recent = (Event)eventsGrid.SelectedItem;
                ((EventsViewModel)DataContext).UpdateSelection(list, recent);
            };
        }
    }
}
