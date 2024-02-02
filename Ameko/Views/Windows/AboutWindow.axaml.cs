using Ameko.ViewModels;
using Avalonia.Controls;

namespace Ameko.Views.Windows
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            this.DataContext = new AboutWindowViewModel();
        }
    }
}
