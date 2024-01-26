using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
    /// <summary>
    /// Manages the commit history for a file
    /// </summary>
    public class HistoryManager
    {
        public delegate void EventCommitEvent(Commit<Event> current);
        public delegate void StyleCommitEvent(Commit<Style> current);
        public event EventCommitEvent? CommitEvent;
        public event StyleCommitEvent? CommitStyle;

        private readonly Stack<Commit<Event>> eventsHistory;
        private readonly Stack<Commit<Event>> eventsFuture;
        private readonly Stack<Commit<Style>> stylesHistory;
        private readonly Stack<Commit<Style>> stylesFuture;

        /// <summary>
        /// Commit an event change to history
        /// </summary>
        /// <param name="commit"></param>
        public void Commit(Commit<Event> commit)
        {
            eventsHistory.Push(commit);
            CommitEvent?.Invoke(commit);
        }

        /// <summary>
        /// Commit a style change to history
        /// </summary>
        /// <param name="commit"></param>
        public void Commit(Commit<Style> commit)
        {
            stylesHistory.Push(commit);
            CommitStyle?.Invoke(commit);
        }

        /// <summary>
        /// Go back in history.
        /// </summary>
        /// <param name="current">Commit to push to the future</param>
        /// <returns>The commit 1 level up in history</returns>
        public Commit<Event> Back(Commit<Event> current)
        {
            eventsFuture.Push(current);
            var newCurrent = eventsHistory.Pop();
            CommitEvent?.Invoke(newCurrent);
            return newCurrent;
        }

        /// <summary>
        /// Go back in history.
        /// </summary>
        /// <param name="current">Commit to push to the future</param>
        /// <returns>The commit 1 level up in history</returns>
        public Commit<Style> Back(Commit<Style> current)
        {
            stylesFuture.Push(current);
            var newCurrent = stylesHistory.Pop();
            CommitStyle?.Invoke(newCurrent);
            return newCurrent;
        }

        /// <summary>
        /// Go forward in history.
        /// </summary>
        /// <param name="current">Commit to push to the past</param>
        /// <returns>The commit 1 level up in the future</returns>
        public Commit<Event> Forward(Commit<Event> current)
        {
            eventsHistory.Push(current);
            var newCurrent = eventsFuture.Pop();
            CommitEvent?.Invoke(newCurrent);
            return newCurrent;
        }

        /// <summary>
        /// Go forward in history.
        /// </summary>
        /// <param name="current">Commit to push to the past</param>
        /// <returns>The commit 1 level up in the future</returns>
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
