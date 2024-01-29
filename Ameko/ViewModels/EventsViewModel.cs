using Ameko.Services;
using AssCS;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.ViewModels
{
    public class EventsViewModel : ViewModelBase
    {
        public string Ye => "Ye Binding Here";
        public ObservableCollection<Event> Events { get; private set; }

        private void UpdateEvents(object? sender, EventArgs e)
        {
            Events.Clear();
            Events.AddRange(HoloService.HoloInstance.Workspace.WorkingFile.EventManager.Ordered);
        }

        public EventsViewModel()
        {
            Events = new ObservableCollection<Event>(HoloService.HoloInstance.Workspace.WorkingFile.EventManager.Ordered);

            // Set up the Events collection to update when changes are made
            HoloService.HoloInstance.Workspace.WorkingFile.EventManager.CurrentEvents.CollectionChanged += UpdateEvents;
            HoloService.HoloInstance.Workspace.PropertyChanged += UpdateEvents;
        }
    }
}
