using Ameko.ViewModels;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive.Disposables;
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

    private async Task DoShowOpenFileDialogAsync(InteractionContext<MainViewModel, string?> interaction)
    {
        var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Subtitle File",
            AllowMultiple = false,
            FileTypeFilter = new[] {
                new FilePickerFileType("ASS Files") { Patterns = new[] { "*.ass" } }
            }
        });
        if (files.Count > 0)
        {
            var filepath = files[0].Path.AbsolutePath.ToString();
            interaction.SetOutput(filepath);
            return;
        }
        interaction.SetOutput(null);
    }

    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated((CompositeDisposable disposables) =>
        {
            if (ViewModel != null)
            {
                ViewModel.ShowAboutDialog.RegisterHandler(DoShowAboutDialogAsync);
                ViewModel.ShowOpenFileDialog.RegisterHandler(DoShowOpenFileDialogAsync);
            }

            Disposable.Create(() => { }).DisposeWith(disposables);
        });
    }
}
