using Ameko.Services;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.ViewModels
{
    public class AboutWindowViewModel : ViewModelBase
    {
        public string CompiledVersion { get; }
        public string Licensing { get; }

        public AboutWindowViewModel()
        {
            CompiledVersion = AmekoService.VERSION_BUG;
            Licensing = new StreamReader(AssetLoader.Open(new Uri("avares://Ameko/Assets/Licensing.txt"))).ReadToEnd();
            Console.WriteLine(Licensing);
        }
    }
}
