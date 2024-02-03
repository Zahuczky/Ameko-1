using Holo;
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
        private FileWrapper _wrapper;
        private int _id;
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

        public TabItemViewModel(string title, FileWrapper wrapper)
        {
            _title = title;
            _wrapper = wrapper;
            _id = wrapper.ID;
        }
    }
}
