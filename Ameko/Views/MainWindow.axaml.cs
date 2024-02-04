using Ameko.ViewModels;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Holo;
using ReactiveUI;
using System;
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
        interaction.SetOutput(null);
    }

    private async Task DoShowOpenFileDialogAsync(InteractionContext<MainViewModel, Uri?> interaction)
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
            interaction.SetOutput(files[0].Path);
            return;
        }
        interaction.SetOutput(null);
    }

    private async Task DoShowSaveAsFileDialogAsync(InteractionContext<FileWrapper, Uri?> interaction)
    {
        if (interaction.Input == null) return;

        var file = await this.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Subtitle File As...",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("ASS Files") { Patterns = new[] { "*.ass" } }
            },
            SuggestedFileName = $"{interaction.Input.Title} (Copy)"
        });
        if (file != null)
        {
            interaction.SetOutput(file.Path);
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
                ViewModel.ShowSaveAsFileDialog.RegisterHandler(DoShowSaveAsFileDialogAsync);
            }

            Disposable.Create(() => { }).DisposeWith(disposables);
        });
    }
}
