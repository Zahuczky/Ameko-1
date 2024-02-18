using Ameko.DataModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ameko.ViewModels
{
    public class PasteOverWindowViewModel : ViewModelBase
    {
        public PasteOverField Fields { get; set; }

        public ReactiveCommand<Unit, PasteOverField> SelectFieldsCommand { get; }

        //public ICommand SelectAllCommand { get; }
        //public ICommand SelectNoneCommand { get; }
        //public ICommand SelectTimesCommand { get; }
        //public ICommand SelectTextCommand { get; }

        public PasteOverWindowViewModel()
        {
            // Fields = PasteOverField.Text;

            SelectFieldsCommand = ReactiveCommand.Create(() =>
            {
                return Fields;
            });

            //SelectTextCommand = ReactiveCommand.Create(() =>
            //{
            //    Fields = PasteOverField.Text;
            //    // RaiseAndSetIfChanged
            //});

            //SelectTimesCommand = ReactiveCommand.Create(() =>
            //{
            //    Fields = PasteOverField.StartTime | PasteOverField.EndTime;
            //});

            //SelectNoneCommand = ReactiveCommand.Create(() =>
            //{
            //    Fields = PasteOverField.None;
            //});

            //SelectAllCommand = ReactiveCommand.Create(() =>
            //{
            //    Fields = PasteOverField.Comment | PasteOverField.Layer | PasteOverField.StartTime | PasteOverField.EndTime
            //            | PasteOverField.Style | PasteOverField.Actor | PasteOverField.MarginLeft | PasteOverField.MarginRight
            //            | PasteOverField.MarginVertical | PasteOverField.Effect | PasteOverField.Text;
            //});
        }
    }
}
