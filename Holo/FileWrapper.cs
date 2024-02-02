﻿using AssCS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Holo
{
    public class FileWrapper : INotifyPropertyChanged
    {
        private readonly File file;
        private Event? selectedEvent;
        private List<Event>? selectedEvents;

        public File File => file;

        public List<Event>? SelectedEvents
        {
            get => selectedEvents;
            private set => selectedEvents = value;
        }

        public Event? SelectedEvent
        {
            get => selectedEvent;
            private set { selectedEvent = value; OnPropertyChanged(nameof(SelectedEvent)); }
        }
        
        public void Select(List<Event> selectedEvents, Event selectedEvent)
        {
            if (SelectedEvents == null)
            {
                file.HistoryManager.Commit(new Commit<Event>(selectedEvents, AssCS.Action.EDIT));
                SelectedEvents = selectedEvents;
                SelectedEvent = selectedEvent;
                return;
            }

            if (selectedEvents.Count == 0) return;
            
            if ((SelectedEvent != null 
                && SelectedEvent.Equals(file.EventManager.Get(SelectedEvent.Id)))
                || SelectedEvent == null)
            {
                // No change, continue
                SelectedEvents = selectedEvents;
                SelectedEvent = selectedEvent;
                return;
            }
            else
            {
                // Change happened, commit the lot
                file.HistoryManager.Commit(new Commit<Event>(selectedEvents, AssCS.Action.EDIT));
                SelectedEvents = selectedEvents;
                SelectedEvent = selectedEvent;
                return;
            }
        }

        public void Remove(List<Event> selectedEvents, Event selectedEvent)
        {
            if (selectedEvents.Count > 1)
            {
                // Remove all but the "currently selected" one
                foreach (var evnt in selectedEvents.Where(e => e.Id != selectedEvent.Id))
                {
                    file.EventManager.Remove(evnt.Id);
                }
            }
            // Commit the list, remove the final one, and select an adjacent event
            var want = SelectAdjOrDefault(selectedEvent);
            file.HistoryManager.Commit(new Commit<Event>(selectedEvents, AssCS.Action.DELETE));
            file.EventManager.Remove(selectedEvent.Id);
            SelectedEvents = new List<Event> { want };
            SelectedEvent = want;
            return;
        }

        public void Add(List<Event> selectedEvents, Event selectedEvent, bool select)
        {
            file.HistoryManager.Commit(new Commit<Event>(selectedEvents, AssCS.Action.INSERT));
            if (select)
            {
                SelectedEvent = selectedEvent;
                SelectedEvents = selectedEvents;
            }
        }

        // TODO: Undo and redo!

        /// <summary>
        /// Select the next, or previous, or new event
        /// </summary>
        /// <param name="e">Current event</param>
        /// <returns>Event found</returns>
        private Event SelectAdjOrDefault(Event e)
        {
            var want = file.EventManager.GetAfter(e.Id);
            if (want == null) want = file.EventManager.GetBefore(e.Id);
            if (want == null)
            {
                var id = file.EventManager.AddFirst(new Event(file.EventManager.NextId));
                want = file.EventManager.Get(id);
            }
            return want;
        }

        public FileWrapper(File file)
        {
            this.file = file;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
