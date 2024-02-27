using Ameko.ViewModels;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;

namespace Ameko.Views
{
    public partial class DependencyControlWindow : ReactiveWindow<DependencyControlWindowViewModel>
    {
        private async void DoShowRepoManagerAsync(InteractionContext<DependencyControlWindowViewModel, Unit> interaction)
        {
            interaction.SetOutput(Unit.Default);
            var manager = new DependencyControlRepoWindow();
            manager.DataContext = interaction.Input;
            await manager.ShowDialog(this);
        }

        private async void DoShowImportExportAsync(InteractionContext<DependencyControlWindowViewModel, Unit> interaction)
        {
            interaction.SetOutput(Unit.Default);
            var impexp = new DependencyControlImportExportWindow();
            impexp.DataContext = interaction.Input;
            await impexp.ShowDialog(this);
        }

        public DependencyControlWindow()
        {
            InitializeComponent();

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                if (ViewModel != null)
                {
                    ViewModel.DisplayRepoManager.RegisterHandler(DoShowRepoManagerAsync);
                    ViewModel.DisplayImportExport.RegisterHandler(DoShowImportExportAsync);
                }
            });
        }
    }
}
