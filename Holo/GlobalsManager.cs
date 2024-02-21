using AssCS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Tomlet;

namespace Holo
{
    public class GlobalsManager
    {
        private int _styleId;
        public int NextStyleId => _styleId++;
        private string _globalsFilePath;

        private ObservableCollection<Style> Styles { get; set; }
        private ObservableCollection<Color> Colors { get; set; }
        private ObservableCollection<string> Repositories { get; set; }
        private Dictionary<string, string> SubmenuOverrides { get; set; }
        public ObservableCollection<string> StyleNames { get; private set; }

        /// <summary>
        /// Add a new DepCtl repository
        /// </summary>
        /// <param name="repoUrl"></param>
        public void AddRepository(string repoUrl)
        {
            Repositories.Add(repoUrl);
            Write();
        }

        /// <summary>
        /// Remove a DepCtl repository
        /// </summary>
        /// <param name="repoUrl"></param>
        /// <returns>True if it was removed</returns>
        public bool RemoveRepository(string repoUrl)
        {
            var removed = Repositories.Remove(repoUrl);
            if (removed) Write();
            return removed;
        }

        /// <summary>
        /// Override a script's submenu
        /// </summary>
        /// <param name="qualifiedName">Script's qualified name</param>
        /// <param name="value">Submenu name</param>
        public void SetSubmenuOverride(string qualifiedName, string value)
        {
            SubmenuOverrides[qualifiedName] = value;
            Write();
        }

        /// <summary>
        /// Remove a script's submenu override
        /// </summary>
        /// <param name="qualifiedName">Script's qualified name</param>
        /// <returns>True if it was removed</returns>
        public bool RemoveSubmenuOverride(string qualifiedName)
        {
            var removed =  SubmenuOverrides.Remove(qualifiedName);
            if (removed) Write();
            return removed;
        }

        /// <summary>
        /// Get a list of repositories
        /// </summary>
        /// <returns>List of repositories</returns>
        public List<string> GetRepositories()
        {
            return Repositories.ToList();
        }

        /// <summary>
        /// Get a list of submenu overrides
        /// </summary>
        /// <returns>List of Tuples containing the submenu overrides</returns>
        public List<Tuple<string, string>> GetSubmenuOverrides()
        {
            return SubmenuOverrides.Select(k => new Tuple<string, string>(k.Key, k.Value)).ToList();
        }

        public Dictionary<string, string> SubmenuOverridesMap => new Dictionary<string, string>(SubmenuOverrides);

        /// <summary>
        /// Add a style
        /// </summary>
        /// <param name="s">Style to add</param>
        public void AddStyle(Style s)
        {
            var conflicts = Styles.Where(st => st.Name.Equals(s.Name)).ToList();
            if (conflicts.Any())
            {
                Styles.Remove(conflicts.First());
            }
            Styles.Add(s);
            StyleNames.Add(s.Name);
            Write();
        }

        /// <summary>
        /// Remove a style
        /// </summary>
        /// <param name="name">Name of the style to remove</param>
        /// <returns>True if the style was removed</returns>
        public bool RemoveStyle(string name)
        {
            var results = Styles.Where(s => s.Name.Equals(name)).ToList();
            if (results.Any())
            {
                Styles.Remove(Styles.First());
                var removed = StyleNames.Remove(results.First().Name);
                if (removed) Write();
                return removed;
            }
            return false;
        }

        /// <summary>
        /// Get a style
        /// </summary>
        /// <param name="name">Name of the style</param>
        /// <returns>The style</returns>
        public Style? GetStyle(string name)
        {
            if (StyleNames.Contains(name))
                return Styles.Where(s => s.Name.Equals(name)).First();
            return null;
        }

        /// <summary>
        /// Write the Globals file
        /// </summary>
        public async void Write()
        {
            try
            {
                var dir = Path.GetDirectoryName(_globalsFilePath);
                var file = new GlobalsModel
                {
                    GlobalsVersion = 1.0,
                    Styles = this.Styles.Select(s => s.AsAss()).ToList(),
                    Colors = this.Colors.Select(c => c.AsAss()).ToList(),
                    DepCtl =
                    new DepCtlGlobalsModel {
                        Repositories = this.Repositories?.ToList(),
                        SubmenuOverrides = this.SubmenuOverrides
                    }
                };

                using var writer = new StreamWriter(_globalsFilePath, false);
                string m = TomletMain.TomlStringFrom(file);
                await writer.WriteAsync(m);
            }
            catch { return; }
        }

        /// <summary>
        /// Read the Globals file
        /// </summary>
        /// <exception cref="IOException"></exception>
        public void Read()
        {
            if (!System.IO.File.Exists(_globalsFilePath))
            {
                Write();
                return;
            }

            try
            {
                using var reader = new StreamReader(_globalsFilePath);
                var contents = reader.ReadToEnd();
                GlobalsModel input = TomletMain.To<GlobalsModel>(contents);

                _styleId = 0;
                Styles.Clear();
                StyleNames.Clear();
                Colors.Clear();

                Styles = new ObservableCollection<Style>(input.Styles.Select(s => new Style(NextStyleId, s)));
                Colors = new ObservableCollection<Color>(input.Colors.Select(c => new Color(c)));
                StyleNames = new ObservableCollection<string>(Styles.Select(s => s.Name));
                Repositories = new ObservableCollection<string>(input.DepCtl?.Repositories);
                SubmenuOverrides = new Dictionary<string, string>(input.DepCtl?.SubmenuOverrides);
            }
            catch { throw new IOException($"An error occured while loading global file {_globalsFilePath}"); }
        }

        public GlobalsManager()
        {
            _globalsFilePath = Path.Join(HoloContext.HoloDirectory, "globals.toml");
            Styles = new ObservableCollection<Style>();
            Colors = new ObservableCollection<Color>();
            StyleNames = new ObservableCollection<string>();
            Repositories = new ObservableCollection<string>();
            SubmenuOverrides = new Dictionary<string, string>();
            _styleId = 0;
            Read();
        }

        private class GlobalsModel
        {
            /// <summary>
            /// Version of the globals to allow for future additions to the spec
            /// </summary>
            public double GlobalsVersion;
            /// <summary>
            /// List of styles
            /// </summary>
            public List<string>? Styles;
            /// <summary>
            /// List of colors
            /// </summary>
            public List<string>? Colors;
            /// <summary>
            /// Depencency Control model
            /// </summary>
            public DepCtlGlobalsModel? DepCtl;
        }

        private class DepCtlGlobalsModel
        {
            /// <summary>
            /// List of DependencyControl repositories
            /// </summary>
            public List<string>? Repositories;
            /// <summary>
            /// Mapping of user-specified submenu overrides
            /// </summary>
            public Dictionary<string, string>? SubmenuOverrides;
        }
    }
}
