using Ameko.Services;
using AssCS;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.ViewModels
{
    public class EditingViewModel : ViewModelBase
    {
        private Event? _selectedEvent;
        public Event? SelectedEvent
        {
            get => _selectedEvent;
            private set => this.RaiseAndSetIfChanged(ref _selectedEvent, value);
        }

        private void UpdateSelections(object? sender, EventArgs e)
        {
            SelectedEvent = HoloService.HoloInstance.Workspace.WorkingFile.SelectedEvent;
        }

        public EditingViewModel()
        {
            HoloService.HoloInstance.Workspace.WorkingFile.PropertyChanged += UpdateSelections; // won't work
        }
    }
}
