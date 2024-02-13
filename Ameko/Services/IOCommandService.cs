using Ameko.DataModels;
using Ameko.ViewModels;
using AssCS.IO;
using Holo;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.Services
{
    public class IOCommandService
    {
        /// <summary>
        /// Display the About Ameko dialog box
        /// </summary>
        /// <param name="interaction"></param>
        public static async void DisplayAboutBox(Interaction<AboutWindowViewModel, AboutWindowViewModel?> interaction)
        {
            var about = new AboutWindowViewModel();
            await interaction.Handle(about);
        }

        /// <summary>
        /// Display the Styles Manager
        /// </summary>
        /// <param name="interaction"></param>
        public static async void DisplayStylesManager(Interaction<StylesManagerViewModel, StylesManagerViewModel?> interaction, MainViewModel vm)
        {
            var manager = new StylesManagerViewModel();
            await interaction.Handle(manager);
        }

        /// <summary>
        /// Display the Open Subtitle file dialog
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="vm"></param>
        public static async void DisplayOpenSubtitleFileDialog(Interaction<MainViewModel, Uri?> interaction, MainViewModel vm)
        {
            var uri = await interaction.Handle(vm);
            if (uri == null) return;
            HoloContext.Instance.Workspace.AddFileToWorkspace(uri);
        }

        /// <summary>
        /// Save the file, or display the Save As dialog
        /// </summary>
        /// <param name="interaction"></param>
        public static async void SaveOrDisplaySaveAsDialog(Interaction<FileWrapper, Uri?> interaction)
        {
            var workingFile = HoloContext.Instance.Workspace.WorkingFile;
            if (workingFile == null) return;

            Uri uri;
            if (workingFile.FilePath == null)
            {
                uri = await interaction.Handle(workingFile);
                if (uri == null) return;
                HoloContext.Instance.Workspace.ReferencedFiles.Where(f => f.Id == workingFile.ID).Single().Path = uri.LocalPath;
            }
            else
            {
                uri = workingFile.FilePath;
            }
            var writer = new AssWriter(workingFile.File, uri.LocalPath, AmekoInfo.Instance);
            writer.Write(false);
            workingFile.UpToDate = true;
        }

        /// <summary>
        /// Display the Save As dialog
        /// </summary>
        /// <param name="interaction"></param>
        public static async void DisplaySaveAsDialog(Interaction<FileWrapper, Uri?> interaction)
        {
            var workingFile = HoloContext.Instance.Workspace.WorkingFile;
            if (workingFile == null) return;

            var uri = await interaction.Handle(workingFile);
            if (uri == null) return;

            var writer = new AssWriter(workingFile.File, uri.LocalPath, AmekoInfo.Instance);
            writer.Write(false);
            workingFile.UpToDate = true;

            var reffile = HoloContext.Instance.Workspace.ReferencedFiles.Where(f => f.Id == workingFile.ID).Single();
            reffile.Path = uri.LocalPath;
        }

        /// <summary>
        /// Display the Open File dialog for Workspaces
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="vm"></param>
        public static async void DisplayWorkspaceOpenDialog(Interaction<MainViewModel, Uri?> interaction, MainViewModel vm)
        {
            var uri = await interaction.Handle(vm);
            if (uri == null) return;

            // TODO: save prompts and stuff!
            HoloContext.Instance.Workspace.OpenWorkspaceFile(uri);
        }

        /// <summary>
        /// Save the workspace or display the Save As dialog for it
        /// </summary>
        /// <param name="interaction"></param>
        public static async void WorkspaceSaveOrDisplaySaveAsDialog(Interaction<Workspace, Uri?> interaction)
        {
            var workspace = HoloContext.Instance.Workspace;
            Uri uri;
            if (workspace.FilePath == null)
            {
                uri = await interaction.Handle(workspace);
                if (uri == null) return;
            }
            else
            {
                uri = workspace.FilePath;
            }
            workspace.WriteWorkspaceFile(uri);
        }
    }
}
