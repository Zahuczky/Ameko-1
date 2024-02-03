using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.ViewModels
{
    public class TabItemViewModel : ViewModelBase
    {
        private string _title;
        private string _description;

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        public TabItemViewModel(string title, string description)
        {
            _title = title;
            _description = description;
        }
    }
}
