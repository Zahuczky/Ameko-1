using Ameko.DataModels;
using Ameko.Services;
using AssCS;
using AssCS.IO;
using DynamicData;
using Holo;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Ameko.ViewModels
{
    public class TabItemViewModel : ViewModelBase
    {
        private string _title;
        private FileWrapper _wrapper;
        private readonly int _id;
        private Event? _selectedEvent;

        public Interaction<TabItemViewModel, string?> CopySelectedEvents { get; }
        public Interaction<TabItemViewModel, string?> CutSelectedEvents { get; }
        public Interaction<TabItemViewModel, string?> Paste { get; }

        public ICommand DeleteSelectedCommand { get; }
        public ICommand CutSelectedEventsCommand { get; }
        public ICommand CopySelectedEventsCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand DuplicateSelectedEventsCommand { get; }
        public ICommand NextOrAddEventCommand { get; }
        public ICommand InsertBeforeCommand { get; }
        public ICommand InsertAfterCommand { get; }

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public FileWrapper Wrapper
        {
            get => _wrapper;
            set => this.RaiseAndSetIfChanged(ref _wrapper, value);
        }

        public string Display
        {
            get => $"{Title}{(!Wrapper.UpToDate ? "*" : "")}";
        }

        public int ID => _id;
        public ObservableCollection<Event> Events { get; private set; }
        public int SelectedIndex { get; set; }

        public Event? SelectedEvent
        {
            get => _selectedEvent;
            private set => this.RaiseAndSetIfChanged(ref _selectedEvent, value);
        }

        public void UpdateEventSelection(List<Event> selectedEvents, Event selectedEvent)
        {
            Wrapper.Select(selectedEvents, selectedEvent);
        }

        private void UpdateEventsCallback(object? sender, EventArgs e)
        {
            Events.Clear();
            Events.AddRange(Wrapper.File.EventManager.Ordered);
        }

        private void UpdateSelectionsCallback(object? sender, EventArgs e)
        {
            SelectedEvent = Wrapper.SelectedEvent;
        }

        public TabItemViewModel(string title, FileWrapper wrapper)
        {
            _title = title;
            _wrapper = wrapper;
            _id = wrapper.ID;

            Events = new ObservableCollection<Event>(Wrapper.File.EventManager.Ordered);
            Wrapper.File.EventManager.CurrentEvents.CollectionChanged += UpdateEventsCallback;
            Wrapper.PropertyChanged += UpdateSelectionsCallback;

            CopySelectedEvents = new Interaction<TabItemViewModel, string?>();
            CutSelectedEvents = new Interaction<TabItemViewModel, string?>();
            Paste = new Interaction<TabItemViewModel, string?>();

            DeleteSelectedCommand = ReactiveCommand.Create(() =>
            {
                // TODO: Add checking!
                if (Wrapper.SelectedEvent == null || Wrapper.SelectedEvents == null) return;
                Wrapper.Remove(Wrapper.SelectedEvents, Wrapper.SelectedEvent);
            });

            CopySelectedEventsCommand = ReactiveCommand.Create(async () =>
            {
                await CopySelectedEvents.Handle(this);
            });

            CutSelectedEventsCommand = ReactiveCommand.Create(async () =>
            {
                await CutSelectedEvents.Handle(this);
            });

            PasteCommand = ReactiveCommand.Create(async () =>
            {
                await Paste.Handle(this);
            });

            DuplicateSelectedEventsCommand = ReactiveCommand.Create(() =>
            {
                if (Wrapper.SelectedEvents == null) return;
                foreach (var evnt in Wrapper.SelectedEvents)
                {
                    var newEvnt = new Event(Wrapper.File.EventManager.NextId, evnt);
                    Wrapper.File.EventManager.AddAfter(evnt.Id, newEvnt);
                }
            });

            InsertBeforeCommand = ReactiveCommand.Create(() =>
            {
                if (Wrapper.SelectedEvent == null) return;
                var newEvnt = new Event(Wrapper.File.EventManager.NextId);
                var before = Wrapper.File.EventManager.GetBefore(Wrapper.SelectedEvent.Id);
                if (before != null)
                {
                    if ((Wrapper.SelectedEvent.Start - before.End).TotalSeconds < 5)
                        newEvnt.Start = before.End;
                    else
                        newEvnt.Start = Wrapper.SelectedEvent.Start - Time.FromSeconds(5);
                }
                newEvnt.End = Wrapper.SelectedEvent.Start;
                newEvnt.Style = Wrapper.SelectedEvent.Style;

                Wrapper.File.EventManager.AddBefore(Wrapper.SelectedEvent.Id, newEvnt);
            });

            InsertAfterCommand = ReactiveCommand.Create(() =>
            {
                if (Wrapper.SelectedEvent == null) return;
                var newEvnt = new Event(Wrapper.File.EventManager.NextId);
                var after = Wrapper.File.EventManager.GetAfter(Wrapper.SelectedEvent.Id);
                if (after != null)
                {
                    if ((after.Start - Wrapper.SelectedEvent.End).TotalSeconds < 5)
                        newEvnt.End = after.Start;
                    else
                        newEvnt.End = Wrapper.SelectedEvent.End + Time.FromSeconds(5);
                }
                newEvnt.Start = Wrapper.SelectedEvent.End;
                newEvnt.Style = Wrapper.SelectedEvent.Style;

                Wrapper.File.EventManager.AddAfter(Wrapper.SelectedEvent.Id, newEvnt);
            });

            NextOrAddEventCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedEvent == null) return;
                var next = Wrapper.File.EventManager.GetAfter(SelectedEvent.Id);
                if (next != null)
                {
                    UpdateEventSelection([next], next);
                }
                else
                {
                    next = new Event(Wrapper.File.EventManager.NextId)
                    {
                        Style = SelectedEvent.Style,
                        Start = new Time(SelectedEvent.End),
                        End = new Time(SelectedEvent.End + Time.FromSeconds(5))
                    };
                    Wrapper.File.EventManager.AddLast(next);
                    UpdateEventSelection([next], next);
                }
            });

            // TODO: Maybe not do this this way
            Wrapper.PropertyChanged += (o, e) => { this.RaisePropertyChanged(nameof(Display)); };
            SelectedIndex = 0;
            Wrapper.Select([Wrapper.File.EventManager.Head], Wrapper.File.EventManager.Head);
        }
    }
}
