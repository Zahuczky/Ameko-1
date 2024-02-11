using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.ViewModels
{
    public class ColorWindowViewModel : ViewModelBase
    {
        public bool UseRing { get; set; }

        private AssCS.Color assColor;
        private Avalonia.Media.Color rgbColor;

        public AssCS.Color Color
        {
            get => assColor;
        }

        public Avalonia.Media.Color RGBColor
        {
            get => rgbColor;
            set
            {
                assColor.Alpha = value.A;
                assColor.Red = value.R;
                assColor.Green = value.G;
                assColor.Blue = value.B;
                this.RaiseAndSetIfChanged(ref rgbColor, value);
                this.RaisePropertyChanged(nameof(Color));
            }
        }

        public ColorWindowViewModel(AssCS.Color color)
        {
            assColor = color;
            rgbColor = new Avalonia.Media.Color((byte)(255 - assColor.Alpha), (byte)assColor.Red, (byte)assColor.Green, (byte)assColor.Blue);
        }
    }
}
