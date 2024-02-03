using Ameko.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Ameko.ViewModels;

public class MainViewModel : ViewModelBase
{
    private int selectedTabIndex;
    public Interaction<AboutWindowViewModel, AboutWindowViewModel?> ShowAboutDialog { get; }
    public Interaction<MainViewModel, string?> ShowOpenFileDialog { get; }
    public ICommand ShowAboutDialogCommand { get; }
    public ICommand ShowOpenFileDialogCommand { get; }

    public ObservableCollection<TabItemViewModel> Tabs { get; set; }
    public int SelectedTabIndex
    {
        get => selectedTabIndex;
        set => this.RaiseAndSetIfChanged(ref selectedTabIndex, value);
    }

    public MainViewModel()
    {
        ShowAboutDialog = new Interaction<AboutWindowViewModel, AboutWindowViewModel?>();
        ShowOpenFileDialog = new Interaction<MainViewModel, string?>();
        
        ShowAboutDialogCommand = ReactiveCommand.Create(async () =>
        {
            var about = new AboutWindowViewModel();
            await ShowAboutDialog.Handle(about);
        });
        ShowOpenFileDialogCommand = ReactiveCommand.Create(async () =>
        {
            string filepath = await ShowOpenFileDialog.Handle(this);
            if (filepath == null) return;

            int id = HoloService.HoloInstance.Workspace.AddFileToWorkspace(filepath);
            // TODO: Real 
            Tabs?.Add(new TabItemViewModel(Path.GetFileNameWithoutExtension(filepath), 
                                            HoloService.HoloInstance.Workspace.GetFile(id).File.EventManager.Head.Text
                                            ));
            SelectedTabIndex = (Tabs?.Count ?? 1) - 1; // lol
        });

        Tabs = [new TabItemViewModel("File1", "Content1"), new TabItemViewModel("File2", "Content2")];
    }
}
