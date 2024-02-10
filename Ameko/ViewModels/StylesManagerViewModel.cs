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
        public Workspace Workspace
        {
            get => _workspace;
            set => this.RaiseAndSetIfChanged(ref _workspace, value);
        }

        public string? SelectedFileStyleName { get; set; }
        public string? SelectedWorkspaceStyleName { get; set; }
        public string? SelectedGlobalStyleName { get; set; }

        public Interaction<StyleEditorViewModel, StyleEditorViewModel?> ShowStyleEditor { get; }

        public ICommand CopyFromFileToWorkspaceCommand { get; }
        public ICommand DeleteFileStyleCommand { get; }
        public ICommand EditFileStyleCommand { get; }

        public ICommand CopyFromWorkspaceToFileCommand { get; }
        public ICommand DeleteWorkspaceStyleCommand { get; }

        public ObservableCollection<string> GlobalStyles { get; private set; }
        
        public StylesManagerViewModel()
        {
            _workspace = HoloContext.Instance.Workspace;

            ShowStyleEditor = new Interaction<StyleEditorViewModel, StyleEditorViewModel?>();

            CopyFromFileToWorkspaceCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedFileStyleName == null) return;
                var style = Workspace.WorkingFile.File.StyleManager.Get(SelectedFileStyleName);
                if (style == null) return;
                Workspace.AddStyle(style);
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

            DeleteWorkspaceStyleCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedWorkspaceStyleName == null) return;
                Workspace.RemoveStyle(SelectedWorkspaceStyleName);
            });

            GlobalStyles = new ObservableCollection<string>(); // TODO
        }
    }
}
