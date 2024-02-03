using Ameko.ViewModels;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Threading.Tasks;

namespace Ameko.Views;

public partial class MainWindow : ReactiveWindow<MainViewModel>
{
    private async Task DoShowAboutDialogAsync(InteractionContext<AboutWindowViewModel, AboutWindowViewModel?> interaction)
    {
        var dialog = new AboutWindow();
        dialog.DataContext = interaction.Input;
        await dialog.ShowDialog(this);
    }

    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(action => action(ViewModel!.ShowAboutDialog.RegisterHandler(DoShowAboutDialogAsync)));
    }
}
