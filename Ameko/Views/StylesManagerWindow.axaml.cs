using Ameko.ViewModels;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Ameko.Views
{
    public partial class StylesManagerWindow : ReactiveWindow<StylesManagerViewModel>
    {
        private void DoShowStyleEditor(InteractionContext<StyleEditorViewModel, StyleEditorViewModel?> interaction)
        {
            var editor = new StyleEditorWindow();
            editor.DataContext = interaction.Input;
            editor.ShowDialog(this);
            interaction.SetOutput(null);
        }

        public StylesManagerWindow()
        {
            InitializeComponent();
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                if (ViewModel != null)
                {
                    ViewModel.ShowStyleEditor.RegisterHandler(DoShowStyleEditor);
                }

                Disposable.Create(() => { }).DisposeWith(disposables);
            });
        }

        private void FileListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            if (sender == null || ViewModel == null) return;
            var box = (ListBox)sender;
            if (box.SelectedItem != null)
                ViewModel.EditFileStyleCommand.Execute(null);
        }

        private void WorkspaceListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            if (sender == null || ViewModel == null) return;
            var box = (ListBox)sender;
            if (box.SelectedItem != null)
                ViewModel.EditWorkspaceStyleCommand.Execute(null);
        }

        private void GlobalListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            if (sender == null || ViewModel == null) return;
            var box = (ListBox)sender;
            if (box.SelectedItem != null)
                ViewModel.EditGlobalsStyleCommand.Execute(null);
        }
    }
}
