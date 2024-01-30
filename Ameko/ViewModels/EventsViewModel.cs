using Ameko.Services;
using AssCS;
using Avalonia.Controls;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.ViewModels
{
    public class EventsViewModel : ViewModelBase
    {
        private Event? _selectedEvent;

        public ObservableCollection<Event> Events { get; private set; }
        public List<Event> SelectedEvents { get; private set; }
        public Event? SelectedEvent
        {
            get => _selectedEvent;
            private set => this.RaiseAndSetIfChanged(ref _selectedEvent, value);
        }

        private void UpdateEvents(object? sender, EventArgs e)
        {
            Events.Clear();
            Events.AddRange(HoloService.HoloInstance.Workspace.WorkingFile.EventManager.Ordered);
        }

        public void UpdateSelection(List<Event> selectedEvents, Event selectedEvent)
        {
            SelectedEvents = selectedEvents;
            SelectedEvent = selectedEvent;
        }

        public EventsViewModel()
        {
            Events = new ObservableCollection<Event>(HoloService.HoloInstance.Workspace.WorkingFile.EventManager.Ordered);
            SelectedEvents = new List<Event>();

            // Set up the Events collection to update when changes are made
            HoloService.HoloInstance.Workspace.WorkingFile.EventManager.CurrentEvents.CollectionChanged += UpdateEvents;
            HoloService.HoloInstance.Workspace.PropertyChanged += UpdateEvents;
        }
    }
}
