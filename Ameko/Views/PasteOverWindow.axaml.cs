using Ameko.ViewModels;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace Ameko.Views
{
    public partial class PasteOverWindow : ReactiveWindow<PasteOverWindowViewModel>
    {
        public PasteOverWindow()
        {
            InitializeComponent();

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                if (ViewModel != null)
                {
                    ViewModel.SelectFieldsCommand.Subscribe(x => Close());
                }

                Disposable.Create(() => { }).DisposeWith(disposables);
            });
        }

        // These are absolutely cursed and not MVVM, but RaiseAndSetIfChanged was
        // causing a StackOverflowException, and this doesn't, so......
        // TODO: Use the commands!!
        private void AllButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            commentBox.IsChecked = true;
            layerBox.IsChecked = true;
            startBox.IsChecked = true;
            endBox.IsChecked = true;
            styleBox.IsChecked = true;
            actorBox.IsChecked = true;
            leftBox.IsChecked = true;
            rightBox.IsChecked = true;
            verticalBox.IsChecked = true;
            effectBox.IsChecked = true;
            textBox.IsChecked = true;
        }

        private void NoneButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            commentBox.IsChecked = false;
            layerBox.IsChecked = false;
            startBox.IsChecked = false;
            endBox.IsChecked = false;
            styleBox.IsChecked = false;
            actorBox.IsChecked = false;
            leftBox.IsChecked = false;
            rightBox.IsChecked = false;
            verticalBox.IsChecked = false;
            effectBox.IsChecked = false;
            textBox.IsChecked = false;
        }

        private void TimeButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            commentBox.IsChecked = false;
            layerBox.IsChecked = false;
            startBox.IsChecked = true;
            endBox.IsChecked = true;
            styleBox.IsChecked = false;
            actorBox.IsChecked = false;
            leftBox.IsChecked = false;
            rightBox.IsChecked = false;
            verticalBox.IsChecked = false;
            effectBox.IsChecked = false;
            textBox.IsChecked = false;
        }

        private void TextButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            commentBox.IsChecked = false;
            layerBox.IsChecked = false;
            startBox.IsChecked = false;
            endBox.IsChecked = false;
            styleBox.IsChecked = false;
            actorBox.IsChecked = false;
            leftBox.IsChecked = false;
            rightBox.IsChecked = false;
            verticalBox.IsChecked = false;
            effectBox.IsChecked = false;
            textBox.IsChecked = true;
        }

    }
}
