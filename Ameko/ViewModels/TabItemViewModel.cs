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

        private void UpdateEvents(object? sender, EventArgs e)
        {
            Events.Clear();
            Events.AddRange(Wrapper.File.EventManager.Ordered);
        }

        private void UpdateSelections(object? sender, EventArgs e)
        {
            SelectedEvent = HoloService.HoloInstance.Workspace.WorkingFile.SelectedEvent;
        }

        public TabItemViewModel(string title, FileWrapper wrapper)
        {
            _title = title;
            _wrapper = wrapper;
            _id = wrapper.ID;

            Events = new ObservableCollection<Event>(Wrapper.File.EventManager.Ordered);
            Wrapper.File.EventManager.CurrentEvents.CollectionChanged += UpdateEvents;
            Wrapper.PropertyChanged += UpdateSelections;

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

            // TODO: Maybe not do this this way
            Wrapper.PropertyChanged += (o, e) => { this.RaisePropertyChanged(nameof(Display)); };
        }
    }
}
