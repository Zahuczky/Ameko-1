using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
    public class HistoryManager
    {
        public delegate void EventCommitEvent(Commit<Event> current);
        public delegate void StyleCommitEvent(Commit<Style> current);
        public event EventCommitEvent? CommitEvent;
        public event StyleCommitEvent? CommitStyle;

        private Stack<Commit<Event>> eventsHistory;
        private Stack<Commit<Event>> eventsFuture;
        private Stack<Commit<Style>> stylesHistory;
        private Stack<Commit<Style>> stylesFuture;

        public void Commit(Commit<Event> commit)
        {
            eventsHistory.Push(commit);
            CommitEvent?.Invoke(commit);
        }

        public void Commit(Commit<Style> commit)
        {
            stylesHistory.Push(commit);
            CommitStyle?.Invoke(commit);
        }

        public Commit<Event> Back(Commit<Event> current)
        {
            eventsFuture.Push(current);
            var newCurrent = eventsHistory.Pop();
            CommitEvent?.Invoke(newCurrent);
            return newCurrent;
        }

        public Commit<Style> Back(Commit<Style> current)
        {
            stylesFuture.Push(current);
            var newCurrent = stylesHistory.Pop();
            CommitStyle?.Invoke(newCurrent);
            return newCurrent;
        }

        public Commit<Event> Forward(Commit<Event> current)
        {
            eventsHistory.Push(current);
            var newCurrent = eventsFuture.Pop();
            CommitEvent?.Invoke(newCurrent);
            return newCurrent;
        }

        public Commit<Style> Forward(Commit<Style> current)
        {
            stylesHistory.Push(current);
            var newCurrent = stylesFuture.Pop();
            CommitStyle?.Invoke(newCurrent);
            return newCurrent;
        }

        public HistoryManager()
        {
            eventsHistory = new Stack<Commit<Event>>();
            eventsFuture = new Stack<Commit<Event>>();
            stylesHistory = new Stack<Commit<Style>>();
            stylesFuture = new Stack<Commit<Style>>();
        }
    }
}
