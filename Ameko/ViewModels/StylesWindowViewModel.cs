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
    public class StylesWindowViewModel : ViewModelBase
    {
        private Workspace _workspace;
        public Workspace Workspace
        {
            get => _workspace;
            set => this.RaiseAndSetIfChanged(ref _workspace, value);
        }

        public ObservableCollection<string> GlobalStyles { get; private set; }
        
        public StylesWindowViewModel(Workspace workspace)
        {
            _workspace = workspace;

            GlobalStyles = new ObservableCollection<string>(); // TODO
        }
    }
}
