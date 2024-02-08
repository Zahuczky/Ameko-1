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
        public ObservableCollection<string> GlobalStyles { get; private set; }
        public ObservableCollection<string> WorkspaceStyles { get; private set; }
        public ObservableCollection<string> FileStyles { get; private set; }
        
        public StylesWindowViewModel()
        {
            GlobalStyles = new ObservableCollection<string>();
            WorkspaceStyles = new ObservableCollection<string>();
            FileStyles = new ObservableCollection<string>();

            GlobalStyles.Add("Asdf");
            GlobalStyles.Add("qwert");
            GlobalStyles.Add("zxcv");
            WorkspaceStyles.Add("Asdf");
            WorkspaceStyles.Add("qwert");
            WorkspaceStyles.Add("zxcv");
            FileStyles.Add("Asdf");
            FileStyles.Add("qwert");
            FileStyles.Add("zxcv");
        }
    }
}
