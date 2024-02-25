using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Tomlet;

namespace Holo
{
    public class ConfigurationManager : INotifyPropertyChanged
    {
        private readonly string _configFilePath;
        private readonly string _keybindsFilePath;

        private string _audioProvider;
        private string _videoProvider;
        private int _cps;
        private bool _autosave;
        private int _autosaveInterval;
        private ObservableCollection<string> _repositories;
        private Dictionary<string, string> _submenuOverrides;
        private Dictionary<string, string> _keybinds;

        /// <summary>
        /// Character-per-second warning limit
        /// </summary>
        public int Cps
        {
            get => _cps;
            set { _cps = value; OnPropertyChanged(nameof(Cps)); WriteConfig(); }
        }

        /// <summary>
        /// Is autosave enabled
        /// </summary>
        public bool Autosave
        {
            get => _autosave;
            set { _autosave = value; OnPropertyChanged(nameof(Autosave)); WriteConfig(); }
        }

        /// <summary>
        /// Autosave writing interval in seconds
        /// </summary>
        public int AutosaveInterval
        {
            get => _autosaveInterval;
            set { _autosaveInterval = value; OnPropertyChanged(nameof(AutosaveInterval)); WriteConfig(); }
        }

        /// <summary>
        /// Add a new DepCtl repository
        /// </summary>
        /// <param name="repoUrl"></param>
        public void AddRepository(string repoUrl)
        {
            _repositories.Add(repoUrl);
            WriteConfig();
        }

        /// <summary>
        /// Remove a DepCtl repository
        /// </summary>
        /// <param name="repoUrl"></param>
        /// <returns>True if it was removed</returns>
        public bool RemoveRepository(string repoUrl)
        {
            var removed = _repositories.Remove(repoUrl);
            if (removed) WriteConfig();
            return removed;
        }

        /// <summary>
        /// Override a script's submenu
        /// </summary>
        /// <param name="qualifiedName">Script's qualified name</param>
        /// <param name="value">Submenu name</param>
        public void SetSubmenuOverride(string qualifiedName, string value)
        {
            _submenuOverrides[qualifiedName] = value;
            WriteConfig();
        }

        /// <summary>
        /// Remove a script's submenu override
        /// </summary>
        /// <param name="qualifiedName">Script's qualified name</param>
        /// <returns>True if it was removed</returns>
        public bool RemoveSubmenuOverride(string qualifiedName)
        {
            var removed = _submenuOverrides.Remove(qualifiedName);
            if (removed) WriteConfig();
            return removed;
        }

        /// <summary>
        /// Apply a keybind
        /// </summary>
        /// <param name="qualifiedName">Action's qualified name</param>
        /// <param name="value">Keybind</param>
        public void SetKeybind(string qualifiedName, string value)
        {
            _keybinds[qualifiedName] = value;
            WriteKeybinds();
            OnPropertyChanged(nameof(KeybindsMap));
        }

        /// <summary>
        /// Remove a keybind
        /// </summary>
        /// <param name="qualifiedName">Action's qualified name</param>
        /// <returns>True if it was removed</returns>
        public bool RemoveKeybind(string qualifiedName)
        {
            var removed = _keybinds.Remove(qualifiedName);
            if (removed) WriteKeybinds();
            OnPropertyChanged(nameof(KeybindsMap));
            return removed;
        }

        /// <summary>
        /// Get a list of repositories
        /// </summary>
        /// <returns>List of repositories</returns>
        public List<string> GetRepositories()
        {
            return _repositories.ToList();
        }

        /// <summary>
        /// Get a list of submenu overrides
        /// </summary>
        /// <returns>List of Tuples containing the submenu overrides</returns>
        public List<Tuple<string, string>> GetSubmenuOverrides()
        {
            return _submenuOverrides.Select(k => new Tuple<string, string>(k.Key, k.Value)).ToList();
        }

        public Dictionary<string, string> SubmenuOverridesMap => new Dictionary<string, string>(_submenuOverrides);
        public Dictionary<string, string> KeybindsMap => new Dictionary<string, string>(_keybinds);

        public void ReadConfig()
        {
            if (!File.Exists(_configFilePath))
            {
                WriteConfig();
                return;
            }

            try
            {
                using var reader = new StreamReader(_configFilePath);
                var configContents = reader.ReadToEnd();
                FromConfigurationModel(TomletMain.To<ConfigurationModel>(configContents));
            }
            catch { throw new IOException($"An error occured while loading config file {_configFilePath}"); }
        }

        public void ReadKeybinds()
        {
            if (!File.Exists(_keybindsFilePath))
            {
                WriteKeybinds();
                return;
            }

            try
            {
                using var reader = new StreamReader(_keybindsFilePath);
                var keybindContents = reader.ReadToEnd();
                _keybinds = JsonSerializer.Deserialize<Dictionary<string, string>>(keybindContents)!;
            }
            catch { throw new IOException($"An error occured while loading keybinds file {_keybindsFilePath}"); }
        }

        public async void WriteConfig()
        {
            try
            {
                using var configWriter = new StreamWriter(_configFilePath);
                string m = TomletMain.TomlStringFrom(ToConfigurationModel(), TomlSerializerOptions.Default);
                await configWriter.WriteAsync(m);
            } catch { return; }
        }

        public async void WriteKeybinds()
        {
            try
            {
                using var keybindsWriter = new StreamWriter(_keybindsFilePath);
                string kb = JsonSerializer.Serialize(_keybinds);
                await keybindsWriter.WriteAsync(kb);
            }
            catch { return; }
        }

        private ConfigurationModel ToConfigurationModel()
        {
            return new ConfigurationModel
            {
                ConfigVersion = 1.0,
                AV = new AVConfigModel
                {
                    AudioProvider = this._audioProvider,
                    VideoProvider = this._videoProvider
                },
                General = new GeneralConfigModel
                {
                    Cps = this.Cps,
                    Autosave = this._autosave,
                    AutosaveInterval = this._autosaveInterval
                },
                DependencyControl = new DCConfigModel
                {
                    Repositories = this._repositories.ToList(),
                    SubmenuOverrides = this._submenuOverrides
                }
            };
        }

        private void FromConfigurationModel(ConfigurationModel model)
        {
            this._audioProvider = model.AV?.AudioProvider ?? string.Empty;
            this._videoProvider = model.AV?.VideoProvider ?? string.Empty;
            this.Cps = model.General?.Cps ?? 0;
            this._autosave = model.General?.Autosave ?? true;
            this._autosaveInterval = model.General?.AutosaveInterval ?? 300; // 5 minutes
            this._repositories = new ObservableCollection<string>(model.DependencyControl?.Repositories);
            this._submenuOverrides = new Dictionary<string, string>(model.DependencyControl?.SubmenuOverrides);
        }

        public ConfigurationManager()
        {
            _configFilePath = Path.Join(HoloContext.HoloDirectory, "config.toml");
            _keybindsFilePath = Path.Join(HoloContext.HoloDirectory, "keybinds.json");
            _audioProvider = string.Empty;
            _videoProvider = string.Empty;
            _cps = 0;
            _autosave = true;
            _autosaveInterval = 300; // 5 minutes
            _repositories = new ObservableCollection<string>();
            _submenuOverrides = new Dictionary<string, string>();
            _keybinds = DefaultKeybinds();
            ReadConfig();
            ReadKeybinds();
        }

        #region Models
        private class ConfigurationModel
        {
            public double? ConfigVersion;
            public AVConfigModel? AV;
            public GeneralConfigModel? General;
            public DCConfigModel? DependencyControl;
        }

        private class AVConfigModel
        {
            public string? AudioProvider;
            public string? VideoProvider;
        }

        private class GeneralConfigModel
        {
            public int? Cps;
            public bool? Autosave;
            public int? AutosaveInterval;
        }

        private class DCConfigModel
        {
            public List<string>? Repositories;
            public Dictionary<string, string>? SubmenuOverrides;
        }
        #endregion Models

        private Dictionary<string, string> DefaultKeybinds()
        {
            var defaults = new Dictionary<string, string>
            {
                ["ameko.file.new"] = "Ctrl+N",
                ["ameko.file.open"] = "Ctrl+O",
                ["ameko.file.save"] = "Ctrl+S",
                ["ameko.file.saveas"] = "Ctrl+Shift+S",
                ["ameko.file.search"] = "Ctrl+F",
                ["ameko.file.shift"] = "Ctrl+I",
                ["ameko.event.duplicate"] = "Ctrl+D",
                ["ameko.event.copy"] = "Ctrl+C",
                ["ameko.event.cut"] = "Ctrl+X",
                ["ameko.event.paste"] = "Ctrl+V",
                ["ameko.event.pasteover"] = "Ctrl+Shift+V",
                ["ameko.event.delete"] = "Shift+Delete",
                ["ameko.app.about"] = "Shift+F1",
                ["ameko.app.quit"] = "Ctrl+Q"
            };

            return defaults;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
