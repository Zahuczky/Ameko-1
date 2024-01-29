using Ameko.Services;

namespace Ameko.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        HoloService.HoloInstance.Workspace.AddFileToWorkspace("c:\\test.ass");
    }
}
