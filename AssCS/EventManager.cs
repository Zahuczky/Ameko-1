using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssCS
{
    public class EventManager
    {
        private readonly LinkedList<Event> chain;
        private readonly Dictionary<int, LinkedListNode<Event>> events;
        private int _id = 0;

        public int NextId => _id++;

        public Event Head => chain.First.Value;
        public Event Tail => chain.Last.Value;
        public List<Event> Ordered => chain.ToList();

        public int AddAfter(int id, Event e)
        {
            if (!events.ContainsKey(id)) throw new ArgumentException($"Cannot add event after id={id} because that id cannot be found.");

            var reference = events[id];
            var link = chain.AddAfter(reference, e);
            events[e.Id] = link;
            return e.Id;
        }

        public int AddBefore(int id, Event e)
        {
            if (!events.ContainsKey(id)) throw new ArgumentException($"Cannot add event before id={id} because that id cannot be found.");

            var reference = events[id];
            var link = chain.AddBefore(reference, e);
            events[e.Id] = link;
            return e.Id;
        }

        public int AddLast(Event e)
        {
            var link = chain.AddLast(e);
            events[e.Id] = link;
            return e.Id;
        }

        public int AddFirst(Event e)
        {
            var link = chain.AddFirst(e);
            events[e.Id] = link;
            return e.Id;
        }

        public Event Swap(Event e)
        {
            var original = events[e.Id].Value;
            events[e.Id].Value = e;
            return original;
        }

        public bool Remove(Event e)
        {
            if (events.ContainsKey(e.Id))
            {
                events.Remove(e.Id);
                return chain.Remove(e);
            }
            throw new ArgumentException($"Cannot remove event id={e.Id} because that id cannot be found.");
        }

        public bool Remove(int id)
        {
            if (events.ContainsKey(id))
            {
                var e = events[id].Value;
                events.Remove(id);
                return chain.Remove(e);
            }
            throw new ArgumentException($"Cannot remove event id={id} because that id cannot be found.");
        }

        public void Clear()
        {
            events.Clear();
            chain.Clear();
        }

        public void LoadDefault()
        {
            Clear();
            _id = 0;
            AddFirst(new Event(NextId));
        }

        public EventManager(EventManager source)
        {
            chain = new LinkedList<Event>(source.chain);
            events = new Dictionary<int, LinkedListNode<Event>>(source.events);
        }

        public EventManager(File source)
        {
            chain = new LinkedList<Event>(source.EventManager.chain);
            events = new Dictionary<int, LinkedListNode<Event>>(source.EventManager.events);
        }

        public EventManager()
        {
            chain = new LinkedList<Event>();
            events = new Dictionary<int, LinkedListNode<Event>>();
        }
    }
}
