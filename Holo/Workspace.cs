using AssCS;
using AssCS.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tomlyn;

namespace Holo
{
    public class Workspace
    {
        private int _id;
        public int NextId => _id++;

        private readonly List<Link> ReferencedFiles;
        private readonly Dictionary<int, AssCS.File> LoadedFiles;

        public int WorkingIndex { get; set; }

        public bool AddFileToWorkspace(string filePath)
        {
            try
            {
                AssParser parser = new AssParser();
                var file = parser.Load(filePath);
                var link = new Link(NextId, filePath);
                ReferencedFiles.Add(link);
                LoadedFiles.Add(link.Id, file);
                WorkingIndex = link.Id;
                return true;
            }
            catch { return false; }
        }

        public bool AddFileToWorkspace()
        {
            var dummyLink = new Link(NextId, string.Empty);
            ReferencedFiles.Add(dummyLink);

            var dummyFile = new AssCS.File();
            dummyFile.LoadDefault();
            LoadedFiles.Add(dummyLink.Id, dummyFile);
            return true;
        }

        public bool RemoveFileFromWorkspace(int id)
        {
            CloseFileInWorkspace(id);
            var removed = ReferencedFiles.RemoveAll(f => f.Id == id);
            return removed > 0;
        }

        public bool OpenFileFromWorkspace(int id)
        {
            try
            {
                AssParser parser = new AssParser();
                var link = ReferencedFiles.Find(f => f.Id == id);
                if (link == null) return false;

                var file = parser.Load(link.Path);
                LoadedFiles.Add(link.Id, file);
                WorkingIndex = link.Id;
                return true;
            }
            catch { return false; }
        }

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

        public Workspace(string filePath)
        {
            if (!System.IO.File.Exists(filePath)) throw new FileNotFoundException($"Workspace file {filePath} was not found");
            try
            {
                using var reader = new StreamReader(filePath);
                var configContents = reader.ReadToEnd();
                Workfile space = Toml.ToModel<Workfile>(filePath);
                ReferencedFiles = space.ReferencedFiles.Select(f => new Link(NextId, f)).ToList();
                LoadedFiles = new Dictionary<int, AssCS.File>();
                WorkingIndex = 0;
            }
            catch { throw new IOException($"An error occured while loading workspace file {filePath}"); }
        }

        public Workspace()
        {
            ReferencedFiles = new List<Link>();
            LoadedFiles = new Dictionary<int, AssCS.File>();
            AddFileToWorkspace();
            WorkingIndex = 0;
        }

        private class Workfile
        {
            public List<string>? ReferencedFiles;
        }

        private class Link : Tuple<int, string>
        {
            public Link(int id, string path) : base(id, path) { }
            public int Id => Item1;
            public string Path => Item2;
        }
    }
}
