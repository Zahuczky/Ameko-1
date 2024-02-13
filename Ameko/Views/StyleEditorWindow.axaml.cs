using Ameko.ViewModels;
using AssCS;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Ameko.Views
{
    public partial class StyleEditorWindow : ReactiveWindow<StyleEditorViewModel>
    {
        private async Task DoShowDialogAsync(InteractionContext<ColorWindowViewModel, Color?> interaction)
        {
            var dialog = new ColorWindow();
            dialog.DataContext = interaction.Input;
            var result = await dialog.ShowDialog<Color?>(this);
            interaction.SetOutput(result);
        }

        public StyleEditorWindow()
        {
            InitializeComponent();
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                if (ViewModel != null)
                {
                    ViewModel.ShowDialog.RegisterHandler(DoShowDialogAsync);
                }

                Disposable.Create(() => { }).DisposeWith(disposables);
            });
        }
    }
}
