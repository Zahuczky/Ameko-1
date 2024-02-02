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

        public EditingViewModel()
        {
            
        }
    }
}
