using Ameko.Services;
using AssCS;
using DynamicData;
using Holo;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.ViewModels
{
    public class TabItemViewModel : ViewModelBase
    {
        private string _title;
        private FileWrapper _wrapper;
        private readonly int _id;
        private Event? _selectedEvent;
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
            Events = new ObservableCollection<Event>(Wrapper.File.EventManager.Ordered);
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
        }
    }
}
