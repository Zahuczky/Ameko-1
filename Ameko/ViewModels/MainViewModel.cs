using Ameko.DataModels;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Ameko.ViewModels;

public class MainViewModel : ViewModelBase
{
    public Interaction<AboutWindowViewModel, AboutWindowViewModel?> ShowAboutDialog { get; }
    public ICommand ShowAboutDialogCommand { get; }

    public ObservableCollection<TabItemViewModel> Tabs { get; set; }

    public MainViewModel()
    {
        ShowAboutDialog = new Interaction<AboutWindowViewModel, AboutWindowViewModel?>();
        ShowAboutDialogCommand = ReactiveCommand.Create(async () =>
        {
            var about = new AboutWindowViewModel();
            await ShowAboutDialog.Handle(about);
        });

        Tabs = [new TabItemViewModel("File1", "Content1"), new TabItemViewModel("File2", "Content2")];
    }
}
