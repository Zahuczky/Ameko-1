using AssCS;
using AssCS.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tomlet;

namespace Holo
{
    /// <summary>
    /// Represents a workspace consisting of multiple, potentially open, files.
    /// All files are opened in a workspace. Not all files in a workspace need be opened.
    /// Workspaces may be saved to allow multiple files to be opened at a time, and to
    /// facilitate sharing of workspaces.
    /// </summary>
    public class Workspace : INotifyPropertyChanged
    {
        private int _id;
        public int NextId => _id++;
        private int _workingIndex = 0;

        public readonly ObservableCollection<Link> ReferencedFiles;
        private readonly Dictionary<int, FileWrapper> loadedFiles;
        
        public Uri? FilePath { get; private set; }
        public ObservableCollection<Style> Styles { get; private set; }

        /// <summary>
        /// Index of the currently selected open file in the workspace
        /// </summary>
        public int WorkingIndex
        {
            get { return _workingIndex; }
            set { _workingIndex = value; OnPropertyChanged(nameof(WorkingIndex)); }
        }

        /// <summary>
        /// The currently selected open file
        /// </summary>
        public FileWrapper WorkingFile => loadedFiles[WorkingIndex];

        public FileWrapper GetFile(int id) => loadedFiles[id];
        public List<FileWrapper> Files => loadedFiles.Values.ToList();

        /// <summary>
        /// Add a file to the current workspace and open it
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns>ID of the file</returns>
        public int AddFileToWorkspace(Uri filePath)
        {
            try
            {
                AssParser parser = new AssParser();
                var file = parser.Load(filePath.LocalPath);
                var link = new Link(NextId, filePath.LocalPath);
                ReferencedFiles.Add(link);
                loadedFiles.Add(link.Id, new FileWrapper(file, link.Id, filePath));
                WorkingIndex = link.Id;
                return link.Id;
            }
            catch { return -1; }
        }

        /// <summary>
        /// Adds a new empty file to the workspace
        /// </summary>
        /// <returns>ID of the file</returns>
        public int AddFileToWorkspace()
        {
            var dummyLink = new Link(NextId, string.Empty);
            ReferencedFiles.Add(dummyLink);

            var dummyFile = new AssCS.File();
            dummyFile.LoadDefault();
            loadedFiles.Add(dummyLink.Id, new FileWrapper(dummyFile, dummyLink.Id, null));
            WorkingIndex = dummyLink.Id;
            return dummyLink.Id;
        }

        /// <summary>
        /// Closes a file and removes it from the workspace
        /// </summary>
        /// <param name="id">ID of the file</param>
        /// <returns>True if the file was removed</returns>
        public bool RemoveFileFromWorkspace(int id)
        {
            CloseFileInWorkspace(id);
            var removable = ReferencedFiles.Where(f => f.Id == id).Single();
            var removed = ReferencedFiles.Remove(removable);
            return removed;
        }

        /// <summary>
        /// Open a file currently in the workspace
        /// </summary>
        /// <param name="id">ID of the file</param>
        /// <returns>ID of the file</returns>
        public int OpenFileFromWorkspace(int id)
        {
            try
            {
                AssParser parser = new AssParser();
                var links = ReferencedFiles.Where(f => f.Id == id);
                if (links == null) return -1;
                var link = links.First();
                var file = parser.Load(link.Path);
                loadedFiles.Add(link.Id, new FileWrapper(file, link.Id, new Uri(link.Path)));
                WorkingIndex = link.Id;
                return id;
            }
            catch { return -1; }
        }

        /// <summary>
        /// Closes a file without removing it from the workspace
        /// </summary>
        /// <param name="id">ID of the file to close</param>
        /// <returns>True if the file was closed</returns>
        public bool CloseFileInWorkspace(int id)
        {
            // TODO: Do we want to assume that the caller already saved the file?
            if (loadedFiles.Remove(id))
            {
                if (loadedFiles.Count > 0) WorkingIndex = loadedFiles.Keys.Min();
                else AddFileToWorkspace();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Write the workspace to disk
        /// </summary>
        /// <param name="filePath">Path to save the file</param>
        /// <returns>True if the file was saved</returns>
        public bool WriteWorkspaceFile(Uri filePath)
        {
            try
            {
                var fp = filePath.LocalPath;
                var dir = Path.GetDirectoryName(fp);
                var file = new WorkspaceModel
                {
                    WorkspaceVersion = 1.0,
                    // Relative the paths going in
                    ReferencedFiles = this.ReferencedFiles.Where(f => !f.Path.Equals(string.Empty)).Select(f => Path.GetRelativePath(dir, f.Path)).ToList(),
                    Styles = this.Styles.Select(s => new Tuple<int, string>(s.Id, s.AsAss())).ToList()
                };
                using var writer = new StreamWriter(fp, false);
                string m = TomletMain.TomlStringFrom(file);
                writer.Write(m);
                return true;
            }
            catch { return false; }
        }

        public FileWrapper this[int key]
        {
            get => loadedFiles[key];
        }

        /// <summary>
        /// Instantiate a Workspace by loading a workspace file from disk
        /// </summary>
        /// <param name="filePath">Path to the workspace file</param>
        /// <exception cref="FileNotFoundException">If the file was not found</exception>
        /// <exception cref="IOException">If an error occured during reading / parsing</exception>
        public Workspace(Uri filePath)
        {
            var fp = filePath.LocalPath;
            var dir = Path.GetDirectoryName(fp);
            if (!System.IO.File.Exists(fp)) throw new FileNotFoundException($"Workspace file {filePath} was not found");
            try
            {
                using var reader = new StreamReader(fp);
                var configContents = reader.ReadToEnd();
                WorkspaceModel space = TomletMain.To<WorkspaceModel>(configContents);
                // De-relative the paths coming out of the workspace
                ReferencedFiles = new ObservableCollection<Link>(space.ReferencedFiles.Select(f => new Link(NextId, Path.Combine(dir, f))).ToList());
                Styles = new ObservableCollection<Style>(space.Styles.Select(s => new Style(s.Item1, s.Item2)));
                loadedFiles = new Dictionary<int, FileWrapper>();
                WorkingIndex = 0;
                FilePath = filePath;
            }
            catch { throw new IOException($"An error occured while loading workspace file {filePath}"); }
        }

        /// <summary>
        /// Instantiate a Workspace with a new empty file
        /// </summary>
        public Workspace()
        {
            ReferencedFiles = new ObservableCollection<Link>();
            loadedFiles = new Dictionary<int, FileWrapper>();
            Styles = new ObservableCollection<Style>();
            AddFileToWorkspace();
            WorkingIndex = 0;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Simplified representation of a workspace for saving
        /// </summary>
        private class WorkspaceModel
        {
            /// <summary>
            /// Version of the workspace to allow for future additions to the spec
            /// </summary>
            public double WorkspaceVersion;
            /// <summary>
            /// List of filenames referenced in the workspace
            /// </summary>
            public List<string>? ReferencedFiles;
            /// <summary>
            /// List of workspace styles
            /// </summary>
            public List<Tuple<int,string>>? Styles;
        }

        /// <summary>
        /// Link between an ID and a filepath
        /// </summary>
        public class Link
        {
            private int id;
            private string path;
            public Link(int id, string path)
            {
                this.id = id;
                this.path = path;
            }
            public int Id
            {
                get => id;
                set => id = value;
            }
            public string Path
            {
                get => path;
                set => path = value;
            }
            public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);
        }
    }
}
