using Ameko.ViewModels;
using Avalonia.Controls;

namespace Ameko.Views.Windows
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            this.DataContext = new AboutWindowViewModel();
            InitializeComponent();
        }
    }
}
