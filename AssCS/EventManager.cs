using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
    public class EventManager
    {
        private readonly Dictionary<int, Event> events;

        private int _id = 0;
        public int NextId => _id++;

        public int Set(Event e)
        {
            events[e.Id] = e;
            return e.Id;
        }

        public Event? Get(int id)
        {
            if (events.ContainsKey(id)) return events[id];
            return null;
        }

        public bool Remove(int id)
        {
            return events.Remove(id);
        }

        public void Clear()
        {
            events.Clear();
        }

        public void LoadDefault()
        {
            Clear();
            _id = 0;
            Set(new Event(NextId));
        }

        public EventManager(EventManager source)
        {
            events = new Dictionary<int, Event>(source.events);
        }

        public EventManager(File source)
        {
            events = new Dictionary<int, Event>(source.EventManager.events);
        }

        public EventManager()
        {
            events = new Dictionary<int, Event>();
        }
    }
}
