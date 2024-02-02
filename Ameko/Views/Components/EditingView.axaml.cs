using Ameko.ViewModels;
using Avalonia.Controls;

namespace Ameko.Views.Components
{
    public partial class EditingView : UserControl
    {
        public EditingView()
        {
            this.DataContext = new EditingViewModel();
            InitializeComponent();
        }
    }
}
