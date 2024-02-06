using Ameko.ViewModels;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using System.Diagnostics;

namespace Ameko.Views;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();
    }

    private void ListBoxItem_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (ViewModel == null) return;
        if (sender == null) return;
        var lbi = (ListBoxItem)sender;
        if (lbi.DataContext == null) return;
        var link = (Holo.Workspace.Link)lbi.DataContext;

        ViewModel.TryLoadReferenced(link.Id);
    }
}
