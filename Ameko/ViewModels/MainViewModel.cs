using Ameko.Services;

namespace Ameko.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly string label = ThisAssembly.Git.SemVer.Label.Equals(string.Empty) ? $"{ThisAssembly.Git.SemVer.Label} " : "";
    public string CompiledTitle { get; }
    public MainViewModel()
    {
        CompiledTitle = $"Ameko {label}@ {ThisAssembly.Git.Branch}-{ThisAssembly.Git.Commit}";
        HoloService.HoloInstance.Workspace.AddFileToWorkspace("c:\\test.ass");
    }
}
