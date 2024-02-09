using AssCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.ViewModels
{
    public class StyleEditorViewModel : ViewModelBase
    {
        public Style Style { get; set; }

        public StyleEditorViewModel(Style style)
        {
            this.Style = style;
        }
    }
}
