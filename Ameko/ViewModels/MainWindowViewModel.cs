using Ameko.DataModels;
using Ameko.Services;
using Avalonia.Platform;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Ameko.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string CompiledTitle { get; }
    public ObservableCollection<FileTabItem> FileTabItems { get; }
    private int tcSelectedIndex = 0;
    public int TCSelectedIndex
    {
        get => tcSelectedIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref tcSelectedIndex, value);
            UpdateWorkingIndex();
        }
    }

    private void UpdateWorkingIndex()
    {
        HoloService.HoloInstance.Workspace.WorkingIndex = tcSelectedIndex;
    }

    public MainWindowViewModel()
    {
        CompiledTitle = $"Ameko {AmekoService.VERSION_BUG}";

        FileTabItems = new ObservableCollection<FileTabItem>();

        int x = HoloService.HoloInstance.Workspace.AddFileToWorkspace("c:\\test.ass");
        FileTabItems.Add(new FileTabItem("Default", HoloService.HoloInstance.Workspace.GetFile(x)));
    }
}
