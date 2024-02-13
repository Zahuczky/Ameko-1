using Ameko.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Ameko.Views
{
    public partial class SearchWindow : ReactiveWindow<SearchWindowViewModel>
    {
        public SearchWindow()
        {
            InitializeComponent();
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                queryBox.Focus();
            });
        }
    }
}
