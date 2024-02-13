using Holo;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ameko.ViewModels
{
    public class StylesManagerViewModel : ViewModelBase
    {
        private Workspace _workspace;
        private GlobalsManager _globalsManager;
        public Workspace Workspace
        {
            get => _workspace;
            set => this.RaiseAndSetIfChanged(ref _workspace, value);
        }

        public GlobalsManager GlobalsManager
        {
            get => _globalsManager;
            set => this.RaiseAndSetIfChanged(ref _globalsManager, value);
        }

        public string? SelectedFileStyleName { get; set; }
        public string? SelectedWorkspaceStyleName { get; set; }
        public string? SelectedGlobalStyleName { get; set; }

        public Interaction<StyleEditorViewModel, StyleEditorViewModel?> ShowStyleEditor { get; }

        public ICommand CopyFromFileToWorkspaceCommand { get; }
        public ICommand CopyFromFileToGlobalsCommand { get; }
        public ICommand DeleteFileStyleCommand { get; }
        public ICommand EditFileStyleCommand { get; }

        public ICommand CopyFromWorkspaceToFileCommand { get; }
        public ICommand CopyFromWorkspaceToGlobalsCommand { get; }
        public ICommand DeleteWorkspaceStyleCommand { get; }
        public ICommand EditWorkspaceStyleCommand { get; }

        public ICommand CopyFromGlobalsToFileCommand { get; }
        public ICommand CopyFromGlobalsToWorkspaceCommand { get; }
        public ICommand DeleteGlobalsStyleCommand { get; }
        public ICommand EditGlobalsStyleCommand { get; }
        
        public StylesManagerViewModel()
        {
            _workspace = HoloContext.Instance.Workspace;
            _globalsManager = HoloContext.Instance.GlobalsManager;
            ShowStyleEditor = new Interaction<StyleEditorViewModel, StyleEditorViewModel?>();

            CopyFromFileToWorkspaceCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedFileStyleName == null) return;
                var style = Workspace.WorkingFile.File.StyleManager.Get(SelectedFileStyleName);
                if (style == null) return;
                Workspace.AddStyle(style);
            });

            CopyFromFileToGlobalsCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedFileStyleName == null) return;
                var style = Workspace.WorkingFile.File.StyleManager.Get(SelectedFileStyleName);
                if (style == null) return;
                GlobalsManager.AddStyle(style);
            });

            DeleteFileStyleCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedFileStyleName == null) return;
                Workspace.WorkingFile.File.StyleManager.Remove(SelectedFileStyleName);
            });

            EditFileStyleCommand = ReactiveCommand.Create(async () =>
            {
                if (SelectedFileStyleName == null) return;
                var style = Workspace.WorkingFile.File.StyleManager.Get(SelectedFileStyleName);
                if (style == null) return;
                var editor = new StyleEditorViewModel(style);
                await ShowStyleEditor.Handle(editor);
            });

            CopyFromWorkspaceToFileCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedWorkspaceStyleName == null) return;
                var style = Workspace.GetStyle(SelectedWorkspaceStyleName);
                if (style == null) return;
                Workspace.WorkingFile.File.StyleManager.SetOrReplace(style);
            });

            CopyFromWorkspaceToGlobalsCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedWorkspaceStyleName == null) return;
                var style = Workspace.GetStyle(SelectedWorkspaceStyleName);
                if (style == null) return;
                GlobalsManager.AddStyle(style);
            });

            DeleteWorkspaceStyleCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedWorkspaceStyleName == null) return;
                Workspace.RemoveStyle(SelectedWorkspaceStyleName);
            });

            EditWorkspaceStyleCommand = ReactiveCommand.Create(async () =>
            {
                if (SelectedWorkspaceStyleName == null) return;
                var style = Workspace.GetStyle(SelectedWorkspaceStyleName);
                if (style == null) return;
                var editor = new StyleEditorViewModel(style);
                await ShowStyleEditor.Handle(editor);
            });

            CopyFromGlobalsToFileCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedGlobalStyleName == null) return;
                var style = Workspace.GetStyle(SelectedGlobalStyleName);
                if (style == null) return;
                Workspace.WorkingFile.File.StyleManager.SetOrReplace(style);
            });

            CopyFromGlobalsToWorkspaceCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedGlobalStyleName == null) return;
                var style = GlobalsManager.GetStyle(SelectedGlobalStyleName);
                if (style == null) return;
                Workspace.AddStyle(style);
            });

            DeleteGlobalsStyleCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedGlobalStyleName == null) return;
                GlobalsManager.RemoveStyle(SelectedGlobalStyleName);
            });

            EditGlobalsStyleCommand = ReactiveCommand.Create(async () =>
            {
                if (SelectedGlobalStyleName == null) return;
                var style = GlobalsManager.GetStyle(SelectedGlobalStyleName);
                if (style == null) return;
                var editor = new StyleEditorViewModel(style);
                await ShowStyleEditor.Handle(editor);
            });
        }
    }
}
