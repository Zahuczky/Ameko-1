using Ameko.Services;
using Ameko.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace Ameko.Views
{
    public partial class ShiftTimesWindow : ReactiveWindow<ShiftTimesWindowViewModel>
    {
        public ShiftTimesWindow()
        {
            InitializeComponent();

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                if (ViewModel != null)
                {
                    ViewModel.ShiftTimesCommand.Subscribe(x => Close());
                }

                Disposable.Create(() => { }).DisposeWith(disposables);
            });

            timeBox.AddHandler(InputElement.KeyDownEvent, Helpers.TimeBox_PreKeyDown, Avalonia.Interactivity.RoutingStrategies.Tunnel);
        }
    }
}
