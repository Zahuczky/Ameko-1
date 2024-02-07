using Ameko.ViewModels;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

    private void TabControl_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        if (e.Source is TabControl)
        {
            if (e.AddedItems?.Count > 0)
            {
                var vm = e.AddedItems.Cast<TabItemViewModel>().ElementAt(0);
                vm.UpdateSelectionsOutsideCallback();
            }
        }
    }
}
