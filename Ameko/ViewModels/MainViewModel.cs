using Ameko.Services;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Ameko.ViewModels;

public class MainViewModel : ViewModelBase
{
    private int selectedTabIndex;

    public string WindowTitle { get; } = $"Ameko {AmekoService.VERSION_BUG}";
    public Interaction<AboutWindowViewModel, AboutWindowViewModel?> ShowAboutDialog { get; }
    public Interaction<MainViewModel, Uri?> ShowOpenFileDialog { get; }
    public ICommand ShowAboutDialogCommand { get; }
    public ICommand ShowOpenFileDialogCommand { get; }

    public ICommand CloseTabCommand { get; }

    public ObservableCollection<TabItemViewModel> Tabs { get; set; }
    public int SelectedTabIndex
    {
        get => selectedTabIndex;
        set => this.RaiseAndSetIfChanged(ref selectedTabIndex, value);
    }

    public MainViewModel()
    {
        ShowAboutDialog = new Interaction<AboutWindowViewModel, AboutWindowViewModel?>();
        ShowOpenFileDialog = new Interaction<MainViewModel, Uri?>();
        
        ShowAboutDialogCommand = ReactiveCommand.Create(async () =>
        {
            var about = new AboutWindowViewModel();
            await ShowAboutDialog.Handle(about);
        });
        ShowOpenFileDialogCommand = ReactiveCommand.Create(async () =>
        {
            var uri = await ShowOpenFileDialog.Handle(this);
            if (uri == null) return;

            int id = HoloService.HoloInstance.Workspace.AddFileToWorkspace(uri.LocalPath);
            // TODO: Real 
            Tabs?.Add(new TabItemViewModel(
                Path.GetFileNameWithoutExtension(uri.LocalPath), 
                HoloService.HoloInstance.Workspace.GetFile(id)
            ));
            SelectedTabIndex = (Tabs?.Count ?? 1) - 1; // lol
        });

        CloseTabCommand = ReactiveCommand.Create<int>((int fileId) =>
        {
            // TODO: Saving and stuff
            var closed = HoloService.HoloInstance.Workspace.CloseFileInWorkspace(fileId);
            if (Tabs != null) {
                var tab = Tabs.Where(t => t.ID == fileId).Single();
                Tabs.Remove(tab);
            }
        });

        Tabs = new ObservableCollection<TabItemViewModel>();
    }
}
