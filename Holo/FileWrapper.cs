using AssCS;
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
        private Event? selectedEventCopy;
        private List<Event>? selectedEvents;
        private Uri? filePath;
        private bool upToDate;
        private string title;

        public File File => file;
        public int ID { get; }

        public List<Event>? SelectedEventCollection
        {
            get => selectedEvents;
            private set => selectedEvents = value;
        }

        public Event? SelectedEvent
        {
            get => selectedEvent;
            private set { selectedEvent = value; OnPropertyChanged(nameof(SelectedEvent)); }
        }

        public Uri? FilePath
        {
            get => filePath;
            set { filePath = value; OnPropertyChanged(nameof(FilePath)); }
        }

        public bool UpToDate
        {
            get => upToDate;
            set { upToDate = value; OnPropertyChanged(nameof(UpToDate)); }
        }

        public string Title
        {
            get => title;
            set { title = value; OnPropertyChanged(nameof(Title)); }
        }
        
        public void Select(List<Event> newSelectedEvents, Event newSelectedEvent)
        {
            if (SelectedEventCollection == null && newSelectedEvent != null)
            {
                var snapshot = new Snapshot<Event>(
                    newSelectedEvents.Select(e => 
                        new SnapPosition<Event>(e.Clone(), file.EventManager.GetBefore(e.Id)?.Id)).ToList(),
                    AssCS.Action.EDIT);
                file.HistoryManager.Commit(new Commit<Event>(snapshot));
                SelectedEventCollection = new List<Event>(newSelectedEvents);
                SelectedEvent = newSelectedEvent;
                selectedEventCopy = new Event(newSelectedEvent.Id, newSelectedEvent);
                return;
            }

            if (newSelectedEvents.Count == 0) return;
            
            if ((SelectedEvent != null && selectedEventCopy != null
                && file.EventManager.Has(selectedEventCopy.Id)
                && selectedEventCopy.Equals(file.EventManager.Get(selectedEventCopy.Id)))
                || SelectedEvent == null)
            {
                // No change, continue
                SelectedEventCollection = new List<Event>(newSelectedEvents);
                SelectedEvent = newSelectedEvent;
                selectedEventCopy = new Event(newSelectedEvent!.Id, newSelectedEvent);
                return;
            }
            else
            {
                // Change happened, commit the lot
                var snapshot = new Snapshot<Event>(
                    SelectedEventCollection.Select(e =>
                        new SnapPosition<Event>(e.Clone(), file.EventManager.GetBefore(e.Id)?.Id)).ToList(),
                    AssCS.Action.EDIT);
                file.HistoryManager.Commit(new Commit<Event>(snapshot));
                SelectedEventCollection = new List<Event>(newSelectedEvents);
                SelectedEvent = newSelectedEvent;
                selectedEventCopy = new Event(newSelectedEvent!.Id, newSelectedEvent);
                UpToDate = false;
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
            var snapshot = new Snapshot<Event>(
                selectedEvents.Select(e =>
                    new SnapPosition<Event>(e.Clone(), file.EventManager.GetBefore(e.Id)?.Id)).ToList(),
                AssCS.Action.DELETE);
            file.HistoryManager.Commit(new Commit<Event>(snapshot));
            file.EventManager.Remove(selectedEvent.Id);
            SelectedEventCollection = new List<Event> { want };
            SelectedEvent = want;
            return;
        }

        public void Add(List<Event> selectedEvents, Event selectedEvent, bool select)
        {
            var snapshot = new Snapshot<Event>(
                selectedEvents.Select(e =>
                    new SnapPosition<Event>(e.Clone(), file.EventManager.GetBefore(e.Id)?.Id)).ToList(),
                AssCS.Action.INSERT);
            file.HistoryManager.Commit(new Commit<Event>(snapshot));
            if (select)
            {
                SelectedEvent = selectedEvent;
                SelectedEventCollection = selectedEvents;
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

        #region Commands

        /// <summary>
        /// Duplicate the selected events.
        /// Each event will be duplicated; ABC → AABBCC
        /// </summary>
        public void DuplicateSelected()
        {
            if (SelectedEventCollection == null) return;
            foreach (var evnt in SelectedEventCollection)
            {
                File.EventManager.Duplicate(evnt);
            }
        }

        /// <summary>
        /// Insert a new event before the selected event
        /// </summary>
        public void InsertBeforeSelected()
        {
            if (SelectedEvent == null) return;
            File.EventManager.InsertBefore(SelectedEvent);
        }

        /// <summary>
        /// Insert a new event after the selected event
        /// </summary>
        public void InsertAfterSelected()
        {
            if (SelectedEvent == null) return;
            File.EventManager.InsertAfter(SelectedEvent);
        }

        /// <summary>
        /// Split an event on \N, CPS-adjusted
        /// </summary>
        public void SplitSelected()
        {
            var original = SelectedEvent;
            if (original == null) return;
            var segments = original.Text.Split("\\N");
            if (segments.Length == 0) return;

            var rollingTime = original.Start;
            var goalTime = original.End;
            foreach (var segment in segments)
            {
                var newEvent = new Event(File.EventManager.NextId, original);
                var ratio = segment.Length / (double)original.Text.Replace("\\N", string.Empty).Length;
                newEvent.Text = segment;
                newEvent.Start = Time.FromTime(rollingTime);
                newEvent.End = rollingTime + Time.FromMillis(Convert.ToInt64((goalTime.TotalMilliseconds - original.Start.TotalMilliseconds) * ratio));
                if (newEvent.End > goalTime) newEvent.End = goalTime;

                File.EventManager.AddAfter(SelectedEvent?.Id ?? original.Id, newEvent);
                Select(new List<Event>() { newEvent }, newEvent);
                rollingTime = newEvent.End;
            }
            Remove(new List<Event>() { original }, original);
        }

        public void MergeSelectedAdj()
        {
            if (SelectedEvent == null || SelectedEventCollection == null) return;
            if (SelectedEventCollection.Count != 2) return;

            var one = SelectedEventCollection[0];
            var two = SelectedEventCollection[1];

            var afterOne = File.EventManager.GetAfter(one.Id);
            var beforeOne = File.EventManager.GetBefore(one.Id);

            if (afterOne != null && afterOne.Equals(two))
            {
                var result = new Event(File.EventManager.NextId, one)
                {
                    Start = one.Start,
                    End = two.End,
                    Text = $"{one.Text}\\N{two.Text}"
                };
                File.EventManager.AddAfter(two.Id, result);
                Remove(SelectedEventCollection, SelectedEvent);
                SelectedEvent = result;
                SelectedEventCollection.Clear();
                SelectedEventCollection.Add(result);
            }
            else if (beforeOne != null && beforeOne.Equals(two))
            {
                var result = new Event(File.EventManager.NextId, one)
                {
                    Start = two.Start,
                    End = one.End,
                    Text = $"{two.Text}\\N{one.Text}"
                };
                File.EventManager.AddAfter(one.Id, result);
                Remove(SelectedEventCollection, SelectedEvent);
                SelectedEvent = result;
                SelectedEventCollection.Clear();
                SelectedEventCollection.Add(result);
            }
            else return;
        }

        /// <summary>
        /// Select the next event, or create a new one if there is no subsequent event
        /// </summary>
        public void NextOrAdd()
        {
            if (SelectedEvent == null) return;
            var next = File.EventManager.GetAfter(SelectedEvent.Id);
            if (next != null)
            {
                Select(new List<Event>() { next }, next);
            }
            else
            {
                next = new Event(File.EventManager.NextId)
                {
                    Style = SelectedEvent.Style,
                    Start = new Time(SelectedEvent.End),
                    End = new Time(SelectedEvent.End + Time.FromSeconds(5))
                };
                File.EventManager.AddLast(next);
                Select(new List<Event>() { next }, next);
            }
        }

        #endregion

        public FileWrapper(File file, int id, Uri? filePath)
        {
            this.file = file;
            ID = id;
            UpToDate = true;
            FilePath = filePath;
            if (filePath != null) title = System.IO.Path.GetFileNameWithoutExtension(filePath.LocalPath);
            else title = $"New {id}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
