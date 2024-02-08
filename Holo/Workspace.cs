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
using static Holo.Workspace;

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

        private static readonly FileWrapper FALLBACK_WRAPPER = new FileWrapper(new AssCS.File(), -1, null);

        public ObservableCollection<Link> ReferencedFiles { get; set; }
        public ObservableCollection<FileWrapper> Files { get; set; }

        private Dictionary<int, FileWrapper> loadedFiles;
        
        public Uri? FilePath { get; private set; }
        public ObservableCollection<Style> Styles { get; private set; }
        public ObservableCollection<string> StyleNames { get; private set; }

        /// <summary>
        /// Index of the currently selected open file in the workspace
        /// </summary>
        public int WorkingIndex
        {
            get { return _workingIndex; }
            set
            {
                _workingIndex = value;
                OnPropertyChanged(nameof(WorkingIndex));
                OnPropertyChanged(nameof(WorkingFile)); 
            }
        }

        /// <summary>
        /// The currently selected open file
        /// </summary>
        public FileWrapper WorkingFile => loadedFiles.Count > WorkingIndex ? loadedFiles[WorkingIndex] : FALLBACK_WRAPPER;

        public FileWrapper GetFile(int id) => loadedFiles[id];

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
                Files.Add(GetFile(link.Id));
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
            var id = NextId;
            var dummyLink = new Link(id, string.Empty, $"New {id}");
            ReferencedFiles.Add(dummyLink);

            var dummyFile = new AssCS.File();
            dummyFile.LoadDefault();
            loadedFiles.Add(dummyLink.Id, new FileWrapper(dummyFile, dummyLink.Id, null));
            Files.Add(GetFile(dummyLink.Id));
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
            var unsaved = ReferencedFiles.Where(rf => rf.Id == id).Single().Path.Equals(string.Empty);
            CloseFileInWorkspace(id);
            if (!unsaved)
            {
                var removable = ReferencedFiles.Where(f => f.Id == id).Single();
                var removed = ReferencedFiles.Remove(removable);
                return removed;
            }
            return true;
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
                Files.Add(GetFile(link.Id));
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
        public bool CloseFileInWorkspace(int id, bool replace = true)
        {
            // TODO: Do we want to assume that the caller already saved the file?
            if (loadedFiles.Remove(id))
            {
                Files.Remove(Files.Where(f => f.ID == id).Single());
                if (ReferencedFiles.Where(rf => rf.Id == id).Single().Path.Equals(string.Empty)) 
                    ReferencedFiles.Remove(ReferencedFiles.Where(rf => rf.Id == id).Single());

                if (replace)
                {
                    if (loadedFiles.Count > 0) WorkingIndex = loadedFiles.Keys.Min();
                    else AddFileToWorkspace();
                }
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
                    Styles = this.Styles.ToDictionary(s => s.Id, s => s.AsAss())
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
        /// Load a workspace file from disk
        /// </summary>
        /// <param name="filePath">Path to the workspace file</param>
        /// <exception cref="FileNotFoundException">If the file was not found</exception>
        /// <exception cref="IOException">If an error occured during reading / parsing</exception>
        public void OpenWorkspaceFile(Uri filePath)
        {
            foreach (var f in loadedFiles)
                CloseFileInWorkspace(f.Key, false);

            var fp = filePath.LocalPath;
            var dir = Path.GetDirectoryName(fp);
            if (!System.IO.File.Exists(fp)) throw new FileNotFoundException($"Workspace file {filePath} was not found");
            try
            {
                using var reader = new StreamReader(fp);
                var configContents = reader.ReadToEnd();
                WorkspaceModel space = TomletMain.To<WorkspaceModel>(configContents);
                _id = 0;
                ReferencedFiles.Clear();
                Files.Clear();
                // De-relative the paths coming out of the workspace
                foreach (var rf in space.ReferencedFiles.Select(f => new Link(NextId, Path.Combine(dir, f))).ToList())
                {
                    ReferencedFiles.Add(rf);
                }
                Styles = new ObservableCollection<Style>(space.Styles.Select(s => new Style(s.Key, s.Value)));
                StyleNames = new ObservableCollection<string>(Styles.Select(s => s.Name));
                loadedFiles = new Dictionary<int, FileWrapper>();
                WorkingIndex = 0;
                FilePath = filePath;
            }
            catch { throw new IOException($"An error occured while loading workspace file {filePath}"); }
        }

        /// <summary>
        /// Add a style to the workspace
        /// </summary>
        /// <param name="s">Style to add</param>
        public void AddStyle(Style s)
        {
            Styles.Add(s);
            StyleNames.Add(s.Name);
        }

        /// <summary>
        /// Remove a style from the workspace
        /// </summary>
        /// <param name="name">Name of the style to remove</param>
        /// <returns>True if the style was removed</returns>
        public bool RemoveStyle(string name)
        {
            var styles = Styles.Where(s => s.Name.Equals(name));
            if (styles.Any())
            {
                Styles.Remove(Styles.FirstOrDefault());
                return StyleNames.Remove(styles.First().Name);
            }
            return false;
        }

        /// <summary>
        /// Instantiate a Workspace with a new empty file
        /// </summary>
        public Workspace()
        {
            ReferencedFiles = new ObservableCollection<Link>();
            loadedFiles = new Dictionary<int, FileWrapper>();
            Files = new ObservableCollection<FileWrapper>();
            Styles = new ObservableCollection<Style>();
            StyleNames = new ObservableCollection<string>();
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
            public Dictionary<int, string>? Styles;
        }

        /// <summary>
        /// Link between an ID and a filepath
        /// </summary>
        public class Link : INotifyPropertyChanged
        {
            private int id;
            private string path;
            private string name;
            public Link(int id, string path, string? name = null)
            {
                this.id = id;
                this.path = path;
                if (name != null) this.name = name;
                else this.name = System.IO.Path.GetFileNameWithoutExtension(path);
            }
            public int Id
            {
                get => id;
                set { id = value; OnPropertyChanged(nameof(Id)); }
            }
            public string Path
            {
                get => path;
                set { path = value; OnPropertyChanged(nameof(Path)); }
            }
            public string Name
            {
                get => name;
                set { name = value; OnPropertyChanged(nameof(Name)); }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
