using Ameko.Services;

namespace Ameko.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string CompiledTitle { get; }
    public MainViewModel()
    {
        CompiledTitle = $"Ameko {AmekoService.VERSION_BUG}";
        HoloService.HoloInstance.Workspace.AddFileToWorkspace("c:\\test.ass");
    }
}
