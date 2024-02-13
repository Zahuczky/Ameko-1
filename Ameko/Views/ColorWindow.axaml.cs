using Ameko.ViewModels;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace Ameko.Views
{
    public partial class ColorWindow : ReactiveWindow<ColorWindowViewModel>
    {
        public ColorWindow()
        {
            InitializeComponent();

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                if (ViewModel != null)
                {
                    ViewModel.SelectColorCommand.Subscribe(Close);
                }

                Disposable.Create(() => { }).DisposeWith(disposables);
            });
        }
    }
}
