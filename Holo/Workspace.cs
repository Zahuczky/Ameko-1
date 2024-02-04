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
using Tomlyn;

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

        private readonly ObservableCollection<Link> ReferencedFiles;
        private readonly Dictionary<int, FileWrapper> LoadedFiles;

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
        public FileWrapper WorkingFile => LoadedFiles[WorkingIndex];

        public FileWrapper GetFile(int id) => LoadedFiles[id];
        public List<FileWrapper> Files => LoadedFiles.Values.ToList();

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
                LoadedFiles.Add(link.Id, new FileWrapper(file, link.Id, filePath));
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
            LoadedFiles.Add(dummyLink.Id, new FileWrapper(dummyFile, dummyLink.Id, null));
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
                LoadedFiles.Add(link.Id, new FileWrapper(file, link.Id, new Uri(link.Path)));
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
            if (LoadedFiles.Remove(id))
            {
                if (LoadedFiles.Count > 0) WorkingIndex = LoadedFiles.Keys.Min();
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
        public bool WriteWorkspaceFile(string filePath)
        {
            try
            {
                var file = new Workspacefile
                {
                    WorkspaceVersion = "1.0",
                    ReferencedFiles = this.ReferencedFiles.Select(f => f.Path).ToList()
                };
                using var writer = new StreamWriter(filePath);
                writer.Write(Toml.FromModel(file));
                return true;
            }
            catch { return false; }
        }

        public FileWrapper this[int key]
        {
            get => LoadedFiles[key];
        }

        /// <summary>
        /// Instantiate a Workspace by loading a workspace file from disk
        /// </summary>
        /// <param name="filePath">Path to the workspace file</param>
        /// <exception cref="FileNotFoundException">If the file was not found</exception>
        /// <exception cref="IOException">If an error occured during reading / parsing</exception>
        public Workspace(string filePath)
        {
            if (!System.IO.File.Exists(filePath)) throw new FileNotFoundException($"Workspace file {filePath} was not found");
            try
            {
                using var reader = new StreamReader(filePath);
                var configContents = reader.ReadToEnd();
                Workspacefile space = Toml.ToModel<Workspacefile>(filePath);
                ReferencedFiles = new ObservableCollection<Link>(space.ReferencedFiles.Select(f => new Link(NextId, f)).ToList());
                LoadedFiles = new Dictionary<int, FileWrapper>();
                WorkingIndex = 0;
            }
            catch { throw new IOException($"An error occured while loading workspace file {filePath}"); }
        }

        /// <summary>
        /// Instantiate a Workspace with a new empty file
        /// </summary>
        public Workspace()
        {
            ReferencedFiles = new ObservableCollection<Link>();
            LoadedFiles = new Dictionary<int, FileWrapper>();
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
        private class Workspacefile
        {
            /// <summary>
            /// Version of the workspace to allow for future additions to the spec
            /// </summary>
            public string? WorkspaceVersion;
            /// <summary>
            /// List of filenames referenced in the workspace
            /// </summary>
            public List<string>? ReferencedFiles;
        }

        /// <summary>
        /// Link between an ID and a filepath
        /// </summary>
        private class Link : Tuple<int, string>
        {
            public Link(int id, string path) : base(id, path) { }
            public int Id => Item1;
            public string Path => Item2;
        }
    }
}
