﻿using Ameko.DataModels;
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
        private int _selectionStart;
        private int _selectionEnd;
        private int _focusLostSelectionEnd;

        private static readonly Event FALLBACK_EVENT = new Event(-1) { Text="<No Event Selected>" };

        public ObservableCollection<string> Actors { get; private set; }
        public ObservableCollection<string> Effects { get; private set; }
        public bool DisplayActorsColumn
        {
            get => Actors.Count > 0;
        }
        public bool DisplayEffectsColumn
        {
            get => Effects.Count > 0;
        }

        public Interaction<TabItemViewModel, string?> CopySelectedEvents { get; }
        public Interaction<TabItemViewModel, string?> CutSelectedEvents { get; }
        public Interaction<TabItemViewModel, string[]?> Paste { get; }
        public Interaction<Event, Unit> ScrollIntoViewInteraction { get; }

        public Interaction<PasteOverWindowViewModel, PasteOverField> ShowPasteOverFieldDialog { get; }

        public ICommand DeleteSelectedCommand { get; }
        public ICommand CutSelectedEventsCommand { get; }
        public ICommand CopySelectedEventsCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand PasteOverCommand { get; }
        public ICommand DuplicateSelectedEventsCommand { get; }
        public ICommand NextOrAddEventCommand { get; }
        public ICommand InsertBeforeCommand { get; }
        public ICommand InsertAfterCommand { get; }
        public ICommand SplitEventCommand { get; }
        public ICommand MergeEventsCommand { get; }
        public ICommand ActivateScriptCommand { get; }
        public ICommand ToggleTagCommand { get; }

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

        public int SelectionStart
        {
            get => _selectionStart;
            set => this.RaiseAndSetIfChanged(ref _selectionStart, value);
        }

        public int SelectionEnd
        {
            get => _selectionEnd;
            set => this.RaiseAndSetIfChanged(ref _selectionEnd, value);
        }

        public int FocusLostSelectionEnd
        {
            get => _focusLostSelectionEnd;
            set => this.RaiseAndSetIfChanged(ref _focusLostSelectionEnd, value);
        }

        public int ID => _id;
        public ObservableCollection<Event> Events { get; private set; }
        public int SelectedIndex { get; set; }

        public Event? SelectedEvent
        {
            get
            {
                if (_selectedEvent != null) return _selectedEvent;
                else return FALLBACK_EVENT;
            }
            private set => this.RaiseAndSetIfChanged(ref _selectedEvent, value);
        }

        public void UpdateEventSelection(List<Event> selectedEvents, Event selectedEvent)
        {
            Wrapper.Select(selectedEvents, selectedEvent);
            Actors.Clear();
            Actors.AddRange(Wrapper.File.EventManager.Actors);
            Effects.Clear();
            Effects.AddRange(Wrapper.File.EventManager.Effects);
            this.RaisePropertyChanged(nameof(DisplayActorsColumn));
            this.RaisePropertyChanged(nameof(DisplayEffectsColumn));
        }

        private void UpdateEventsCallback(object? sender, EventArgs e)
        {
            Events.Clear();
            Events.AddRange(Wrapper.File.EventManager.Ordered);
        }

        public void UpdateSelectionsOutsideCallback()
        {
            SelectedEvent = Wrapper.SelectedEvent;
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

            Actors = new ObservableCollection<string>(Wrapper.File.EventManager.Actors);
            Effects = new ObservableCollection<string>(Wrapper.File.EventManager.Effects);

            CopySelectedEvents = new Interaction<TabItemViewModel, string?>();
            CutSelectedEvents = new Interaction<TabItemViewModel, string?>();
            Paste = new Interaction<TabItemViewModel, string[]?>();
            ScrollIntoViewInteraction = new Interaction<Event, Unit>();
            ShowPasteOverFieldDialog = new Interaction<PasteOverWindowViewModel, PasteOverField>();

            DeleteSelectedCommand = ReactiveCommand.Create(() =>
            {
                // TODO: Add checking!
                if (Wrapper.SelectedEvent == null || Wrapper.SelectedEventCollection == null) return;
                Wrapper.Remove(Wrapper.SelectedEventCollection, Wrapper.SelectedEvent);
            });

            CopySelectedEventsCommand = ReactiveCommand.Create(async () => { 
                await CopySelectedEvents.Handle(this); 
            });
            CutSelectedEventsCommand = ReactiveCommand.Create(async () => { await CutSelectedEvents.Handle(this); });
            PasteCommand = ReactiveCommand.Create(() => IOCommandService.PasteLines(Paste, this));
            PasteOverCommand = ReactiveCommand.Create(() => IOCommandService.PasteOverLines(Paste, ShowPasteOverFieldDialog, this));
            DuplicateSelectedEventsCommand = ReactiveCommand.Create(Wrapper.DuplicateSelected);
            InsertBeforeCommand = ReactiveCommand.Create(Wrapper.InsertBeforeSelected);
            InsertAfterCommand = ReactiveCommand.Create(Wrapper.InsertAfterSelected);
            SplitEventCommand = ReactiveCommand.Create(Wrapper.SplitSelected);
            MergeEventsCommand = ReactiveCommand.Create(Wrapper.MergeSelectedAdj);
            NextOrAddEventCommand = ReactiveCommand.Create(Wrapper.NextOrAdd);
            ToggleTagCommand = ReactiveCommand.Create(
                (string tag) =>
                {
                    (SelectionStart, SelectionEnd) = Wrapper.ToggleTag(tag, SelectionStart, FocusLostSelectionEnd);
                    FocusLostSelectionEnd = SelectionEnd;
                }
            );

            ActivateScriptCommand = ReactiveCommand.Create<string>(async (string scriptName) =>
            {
                await ScriptService.Instance.Execute(scriptName);
            });

            // TODO: Maybe not do this this way
            Wrapper.PropertyChanged += (o, e) => { this.RaisePropertyChanged(nameof(Display)); };
            Wrapper.Select(new List<Event>() { Wrapper.File.EventManager.Head }, Wrapper.File.EventManager.Head);
            SelectedEvent = Wrapper.File.EventManager.Head;
        }
    }
}
