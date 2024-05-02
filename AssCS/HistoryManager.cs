using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AssCS
{
    /// <summary>
    /// Manages the commit history for a file
    /// </summary>
    public class HistoryManager : INotifyPropertyChanged
    {
        private readonly Stack<Commit<Event>> _eventHistory;
        private readonly Stack<Commit<Event>> _eventFuture;

        public bool EventCanGoBack => _eventHistory.Count > 0;
        public bool EventCanGoForward => _eventFuture.Count > 0;

        public Commit<Event>? EventGoBack()
        {
            if (_eventHistory.Count == 0) return null;
            var result = _eventHistory.Pop();
            _eventFuture.Push(result);
            Notify();
            return result;
        }

        public Commit<Event>? EventGoForward()
        {
            if (_eventFuture.Count == 0) return null;
            var result = _eventFuture.Pop();
            _eventHistory.Push(result);
            Notify();
            return result;
        }

        public void Commit(Commit<Event> commit)
        {
            _eventHistory.Push(commit);
            _eventFuture.Clear();
            Notify();
            System.Diagnostics.Debug.WriteLine("[COMMIT]");
        }


        public HistoryManager()
        {
            _eventHistory = new Stack<Commit<Event>>();
            _eventFuture = new Stack<Commit<Event>>();
        }

        private void Notify()
        {
            OnPropertyChanged(nameof(EventCanGoBack));
            OnPropertyChanged(nameof(EventCanGoForward));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
