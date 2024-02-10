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
            editor.Show();
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
    }
}
