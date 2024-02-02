using Ameko.Services;
using Avalonia.Platform;
using ReactiveUI;
using System;
using System.IO;

namespace Ameko.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string CompiledTitle { get; }

    public MainWindowViewModel()
    {
        CompiledTitle = $"Ameko {AmekoService.VERSION_BUG}";

        HoloService.HoloInstance.Workspace.AddFileToWorkspace("c:\\test.ass");
    }
}
