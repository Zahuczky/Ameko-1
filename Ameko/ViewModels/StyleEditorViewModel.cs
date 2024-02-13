using AssCS;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ameko.ViewModels
{
    public class StyleEditorViewModel : ViewModelBase
    {
        public Style Style { get; set; }

        public Interaction<ColorWindowViewModel, Color?> ShowDialog { get; }

        public ICommand EditPrimaryCommand { get; }
        public ICommand EditSecondaryCommand { get; }
        public ICommand EditOutlineCommand { get; }
        public ICommand EditShadowCommand { get; }

        public StyleEditorViewModel(Style style)
        {
            this.Style = style;

            ShowDialog = new Interaction<ColorWindowViewModel, Color?>();

            EditPrimaryCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var input = new ColorWindowViewModel(Style.Primary);
                var result = await ShowDialog.Handle(input);
                if (result != null)
                {
                    Style.Primary = result;
                }
            });
            EditSecondaryCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var input = new ColorWindowViewModel(Style.Secondary);
                var result = await ShowDialog.Handle(input);
                if (result != null)
                {
                    Style.Secondary = result;
                }
            });
            EditOutlineCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var input = new ColorWindowViewModel(Style.Outline);
                var result = await ShowDialog.Handle(input);
                if (result != null)
                {
                    Style.Outline = result;
                }
            });
            EditShadowCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var input = new ColorWindowViewModel(Style.Shadow);
                var result = await ShowDialog.Handle(input);
                if (result != null)
                {
                    Style.Shadow = result;
                }
            });
        }
    }
}
