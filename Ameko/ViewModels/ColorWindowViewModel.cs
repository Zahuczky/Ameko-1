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
    public class ColorWindowViewModel : ViewModelBase
    {
        private bool ring;
        public bool UseRing
        {
            get => ring;
            set => this.RaiseAndSetIfChanged(ref ring, value);
        }

        private AssCS.Color assColor;
        private Avalonia.Media.HsvColor hsvColor;

        public ReactiveCommand<Unit, AssCS.Color> SelectColorCommand { get; }

        public AssCS.Color Color
        {
            get => assColor;
        }

        public string AssColorStr => Color.AsAss();

        public Avalonia.Media.HsvColor HSVColor
        {
            get => hsvColor;
            set
            {
                var rgb = value.ToRgb();
                assColor.Alpha = rgb.A;
                assColor.Red = rgb.R;
                assColor.Green = rgb.G;
                assColor.Blue = rgb.B;
                this.RaiseAndSetIfChanged(ref hsvColor, value);
                this.RaisePropertyChanged(nameof(Color));
                this.RaisePropertyChanged(nameof(AssColorStr));
            }
        }

        public ColorWindowViewModel(AssCS.Color color)
        {
            assColor = color;
            hsvColor = new Avalonia.Media.HsvColor(new Avalonia.Media.Color((byte)(255 - assColor.Alpha), (byte)assColor.Red, (byte)assColor.Green, (byte)assColor.Blue));

            SelectColorCommand = ReactiveCommand.Create(() =>
            {
                return assColor;
            });
        }
    }
}
