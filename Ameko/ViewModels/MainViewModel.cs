using Ameko.DataModels;
using Ameko.Services;
using AssCS.IO;
using Avalonia.Controls;
using DynamicData;
using ExCSS;
using Holo;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Ameko.ViewModels;

public class MainViewModel : ViewModelBase
{
    private int selectedTabIndex;
    public Workspace Workspace { get; private set; }

    public string WindowTitle { get; } = $"Ameko {AmekoService.VERSION_BUG}";
    public Interaction<AboutWindowViewModel, AboutWindowViewModel?> ShowAboutDialog { get; }
    public Interaction<MainViewModel, Uri?> ShowOpenFileDialog { get; }
    public Interaction<FileWrapper, Uri?> ShowSaveAsFileDialog { get; }
    public Interaction<MainViewModel, Uri?> ShowOpenWorkspaceDialog { get; }
    public Interaction<Workspace, Uri?> ShowSaveAsWorkspaceDialog { get; }
    public ICommand ShowAboutDialogCommand { get; }
    public ICommand ShowOpenFileDialogCommand { get; }
    public ICommand ShowSaveFileDialogCommand { get; }
    public ICommand ShowSaveAsFileDialogCommand { get; }
    public ICommand ShowOpenWorkspaceDialogCommand { get; }
    public ICommand ShowSaveWorkspaceDialogCommand { get; }

    public ICommand CloseTabCommand { get; }
    public ICommand ActivateScriptCommand { get; }
    public ICommand ReloadScriptsCommand { get; }

    public ObservableCollection<TabItemViewModel> Tabs { get; set; }
    public ObservableCollection<string> ScriptNames { get; }

    public int SelectedTabIndex
    {
        get => selectedTabIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref selectedTabIndex, value);
            if (value >= 0)
                HoloContext.Instance.Workspace.WorkingIndex = Tabs[value].ID;
        }
    }

    public void TryLoadReferenced(int fileId)
    {
        // Is the file already open?
        if (Tabs.Where(t => t.ID == fileId).Any())
        {
            SelectedTabIndex = Tabs.IndexOf(Tabs.Where(t => t.ID == fileId).Single());
            return;
        }
        // Open the file
        int id = HoloContext.Instance.Workspace.OpenFileFromWorkspace(fileId);
        var file = HoloContext.Instance.Workspace.GetFile(id);
        // TODO: Real 
        Tabs?.Add(new TabItemViewModel(
            Path.GetFileNameWithoutExtension(file?.FilePath?.LocalPath ?? "Unnamed"),
            HoloContext.Instance.Workspace.GetFile(id)
        ));
        SelectedTabIndex = (Tabs?.Count ?? 1) - 1; // lol
    }

    public MainViewModel()
    {
        Workspace = HoloContext.Instance.Workspace;
        ShowAboutDialog = new Interaction<AboutWindowViewModel, AboutWindowViewModel?>();
        ShowOpenFileDialog = new Interaction<MainViewModel, Uri?>();
        ShowSaveAsFileDialog = new Interaction<FileWrapper, Uri?>();
        ShowOpenWorkspaceDialog = new Interaction<MainViewModel, Uri?>();
        ShowSaveAsWorkspaceDialog = new Interaction<Workspace, Uri?>();

        ShowAboutDialogCommand = ReactiveCommand.Create(async () =>
        {
            var about = new AboutWindowViewModel();
            await ShowAboutDialog.Handle(about);
        });

        ShowOpenFileDialogCommand = ReactiveCommand.Create(async () =>
        {
            var uri = await ShowOpenFileDialog.Handle(this);
            if (uri == null) return;

            int id = HoloContext.Instance.Workspace.AddFileToWorkspace(uri);
            // TODO: Real 
            Tabs?.Add(new TabItemViewModel(
                Path.GetFileNameWithoutExtension(uri.LocalPath), 
                HoloContext.Instance.Workspace.GetFile(id)
            ));
            SelectedTabIndex = (Tabs?.Count ?? 1) - 1; // lol
        });

        ShowSaveFileDialogCommand = ReactiveCommand.Create(async () =>
        {
            var workingFile = HoloContext.Instance.Workspace.WorkingFile;
            if (workingFile == null) return;

            Uri uri;
            if (workingFile.FilePath == null)
            {
                uri = await ShowSaveAsFileDialog.Handle(workingFile);
                if (uri == null) return;
                HoloContext.Instance.Workspace.ReferencedFiles.Where(f => f.Id == workingFile.ID).Single().Path = uri.LocalPath;
            }
            else
            {
                uri = workingFile.FilePath;
            }
            var writer = new AssWriter(workingFile.File, uri.LocalPath, AmekoInfo.Instance);
            writer.Write(false);
            workingFile.UpToDate = true;
        });

        ShowSaveAsFileDialogCommand = ReactiveCommand.Create(async () =>
        {
            var workingFile = HoloContext.Instance.Workspace.WorkingFile;
            if (workingFile == null) return;

            var uri = await ShowSaveAsFileDialog.Handle(workingFile);
            if (uri == null) return;

            var writer = new AssWriter(workingFile.File, uri.LocalPath, AmekoInfo.Instance);
            writer.Write(false);
            workingFile.UpToDate = true;

            var reffile = HoloContext.Instance.Workspace.ReferencedFiles.Where(f => f.Id == workingFile.ID).Single();
            reffile.Path = uri.LocalPath;
        });

        ShowSaveWorkspaceDialogCommand = ReactiveCommand.Create(async () =>
        {
            var workspace = HoloContext.Instance.Workspace;
            Uri uri;
            if (workspace.FilePath == null)
            {
                uri = await ShowSaveAsWorkspaceDialog.Handle(workspace);
                if (uri == null) return;
            }
            else
            {
                uri = workspace.FilePath;
            }
            workspace.WriteWorkspaceFile(uri);
        });

        ShowOpenWorkspaceDialogCommand = ReactiveCommand.Create(async () =>
        {
            var uri = await ShowOpenWorkspaceDialog.Handle(this);
            if (uri == null) return;

            // TODO: save prompts and stuff!
            Tabs?.Clear();
            HoloContext.Instance.Workspace.OpenWorkspaceFile(uri);
        });

        CloseTabCommand = ReactiveCommand.Create<int>((int fileId) =>
        {
            // TODO: Saving and stuff
            var closed = HoloContext.Instance.Workspace.CloseFileInWorkspace(fileId);
            if (Tabs != null) {
                var tab = Tabs.Where(t => t.ID == fileId).Single();
                Tabs.Remove(tab);
            }
        });

        ActivateScriptCommand = ReactiveCommand.Create<string>(async (string scriptName) =>
        {
            var script = ScriptService.Instance.Get(scriptName);
            if (script == null) return;
            var result = await script.Execute();
            Debug.WriteLine($"Script `{script.Name}` finished executing with status `{result.Status}` and message `{result.Message}`");
        });

        ReloadScriptsCommand = ReactiveCommand.Create(() =>
        {
            ScriptService.Instance.Reload(true);
        });

        Tabs = new ObservableCollection<TabItemViewModel>(HoloContext.Instance.Workspace.Files.Select(f => new TabItemViewModel(f.Title, f)));
        ScriptNames = new ObservableCollection<string>(ScriptService.Instance.LoadedScripts);
        ScriptService.Instance.LoadedScripts.CollectionChanged += (o, e) =>
        {
            ScriptNames.Clear();
            ScriptNames.AddRange(ScriptService.Instance.LoadedScripts);
        };
    }
}
