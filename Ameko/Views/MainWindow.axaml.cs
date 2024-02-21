using Ameko.Services;
using Ameko.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Holo;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Ameko.Views;

public partial class MainWindow : ReactiveWindow<MainViewModel>
{
    private SearchWindow _searchWindow;
    private bool _isSearching = false;
    private static string[] DragDropExtensions = { ".ass" };

    private bool _canClose = false;

    public async void DoShowDependencyControlWindow(InteractionContext<DependencyControlWindowViewModel, Unit> interaction)
    {
        var dialog = new DependencyControlWindow();
        dialog.DataContext = interaction.Input;
        await dialog.ShowDialog(this);
        interaction.SetOutput(Unit.Default);
    }

    public void DoShowSearchWindow(InteractionContext<SearchWindowViewModel, string?> interaction)
    {
        if (_searchWindow == null) return;
        _searchWindow.DataContext ??= interaction.Input;

        if (_isSearching)
            _searchWindow?.Activate();
        else
        {
            _isSearching = true;
            _searchWindow?.Show();
        }
    }

    private async Task DoShowShiftTimesDialog(InteractionContext<ShiftTimesWindowViewModel, Unit> interaction)
    {
        var dialog = new ShiftTimesWindow();
        dialog.DataContext = interaction.Input;
        await dialog.ShowDialog(this);
        interaction.SetOutput(Unit.Default);
    }

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
            SuggestedFileName = $"{interaction.Input.Title}"
        });
        if (file != null)
        {
            interaction.SetOutput(file.Path);
            return;
        }
        interaction.SetOutput(null);
    }

    private async Task DoShowSaveAsWorkspaceDialogAsync(InteractionContext<Workspace, Uri?> interaction)
    {
        if (interaction.Input == null) return;

        var file = await this.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Workspace As...",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("Ameko Workspace Files") { Patterns = new[] { "*.amk" } }
            },
            SuggestedFileName = $"Workspace"
        });
        if (file != null)
        {
            interaction.SetOutput(file.Path);
            return;
        }
        interaction.SetOutput(null);
    }

    private async Task DoShowOpenWorkspaceDialogAsync(InteractionContext<MainViewModel, Uri?> interaction)
    {
        var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Workspace File",
            AllowMultiple = false,
            FileTypeFilter = new[] {
                new FilePickerFileType("Ameko Workspace Files") { Patterns = new[] { "*.amk" } }
            }
        });
        if (files.Count > 0)
        {
            interaction.SetOutput(files[0].Path);
            return;
        }
        interaction.SetOutput(null);
    }

    private void DoShowStylesManager(InteractionContext<StylesManagerViewModel, StylesManagerViewModel?> interaction)
    {
        var manager = new StylesManagerWindow();
        manager.DataContext = interaction.Input;
        manager.Show();
        interaction.SetOutput(null);
    }
    
    private async void DoShowGlobalsManagerAsync(InteractionContext<GlobalsWindowViewModel, Unit> interaction)
    {
        interaction.SetOutput(Unit.Default);
        var manager = new GlobalsWindow();
        manager.DataContext = interaction.Input;
        await manager.ShowDialog(this);
    }

    public MainWindow()
    {
        InitializeComponent();

        _searchWindow = new SearchWindow();
        // _searchWindow.Unloaded += (o, e) => _isSearching = false;
        _searchWindow.Closing += (o, e) => { 
            if (o == null) return; 
            ((SearchWindow)o).Hide();
            _isSearching = false;
            e.Cancel = true; 
        };

        AddHandler(DragDrop.DropEvent, (s, e) =>
        {
            if (e.Data.GetFiles() is { } files && ViewModel != null)
            {
                foreach (var file in files)
                {
                    var lp = file.Path.LocalPath;
                    if (DragDropExtensions.Contains(System.IO.Path.GetExtension(lp)))
                        HoloContext.Instance.Workspace.AddFileToWorkspace(file.Path);
                }
            }
        });

        this.Closing += async (o, e) =>
        {
            if (!_canClose)
            {
                e.Cancel = true;
                if (ViewModel != null)
                {
                    var toClose = await IOCommandService.CloseWindow(ViewModel.ShowSaveAsFileDialog, ViewModel);
                    if (toClose)
                    {
                        _canClose = true;
                        Close();
                    }
                }
            }
        };

        this.WhenActivated((CompositeDisposable disposables) =>
        {
            if (ViewModel != null)
            {
                ViewModel.ShowAboutDialog.RegisterHandler(DoShowAboutDialogAsync);
                ViewModel.ShowOpenFileDialog.RegisterHandler(DoShowOpenFileDialogAsync);
                ViewModel.ShowSaveAsFileDialog.RegisterHandler(DoShowSaveAsFileDialogAsync);
                ViewModel.ShowSaveAsWorkspaceDialog.RegisterHandler(DoShowSaveAsWorkspaceDialogAsync);
                ViewModel.ShowOpenWorkspaceDialog.RegisterHandler(DoShowOpenWorkspaceDialogAsync);
                
                ViewModel.ShowStylesManager.RegisterHandler(DoShowStylesManager);
                ViewModel.ShowSearchDialog.RegisterHandler(DoShowSearchWindow);
                ViewModel.ShowShiftTimesDialog.RegisterHandler(DoShowShiftTimesDialog);
                ViewModel.ShowDependencyControlWindow.RegisterHandler(DoShowDependencyControlWindow);
                ViewModel.ShowGlobalsWindow.RegisterHandler(DoShowGlobalsManagerAsync);
            }

            Disposable.Create(() => { }).DisposeWith(disposables);
        });
    }
}
