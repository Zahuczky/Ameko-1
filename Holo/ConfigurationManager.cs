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
    public class ConfigurationManager : INotifyPropertyChanged
    {
        private readonly string _configFilePath;

        private string _audioProvider;
        private string _videoProvider;
        private int _cps;
        private bool _autosave;
        private int _autosaveInterval;
        private ObservableCollection<string> _repositories;
        private Dictionary<string, string> _submenuOverrides;

        /// <summary>
        /// Character-per-second warning limit
        /// </summary>
        public int Cps
        {
            get => _cps;
            set { _cps = value; OnPropertyChanged(nameof(Cps)); Write(); }
        }

        /// <summary>
        /// Is autosave enabled
        /// </summary>
        public bool Autosave
        {
            get => _autosave;
            set { _autosave = value; OnPropertyChanged(nameof(Autosave)); Write(); }
        }

        /// <summary>
        /// Autosave writing interval in seconds
        /// </summary>
        public int AutosaveInterval
        {
            get => _autosaveInterval;
            set { _autosaveInterval = value; OnPropertyChanged(nameof(AutosaveInterval)); Write(); }
        }

        /// <summary>
        /// Add a new DepCtl repository
        /// </summary>
        /// <param name="repoUrl"></param>
        public void AddRepository(string repoUrl)
        {
            _repositories.Add(repoUrl);
            Write();
        }

        /// <summary>
        /// Remove a DepCtl repository
        /// </summary>
        /// <param name="repoUrl"></param>
        /// <returns>True if it was removed</returns>
        public bool RemoveRepository(string repoUrl)
        {
            var removed = _repositories.Remove(repoUrl);
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
            _submenuOverrides[qualifiedName] = value;
            Write();
        }

        /// <summary>
        /// Remove a script's submenu override
        /// </summary>
        /// <param name="qualifiedName">Script's qualified name</param>
        /// <returns>True if it was removed</returns>
        public bool RemoveSubmenuOverride(string qualifiedName)
        {
            var removed = _submenuOverrides.Remove(qualifiedName);
            if (removed) Write();
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

        public void Read()
        {
            if (!File.Exists(_configFilePath))
            {
                Write();
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

        public async void Write()
        {
            try
            {
                using var writer = new StreamWriter(_configFilePath);
                string m = TomletMain.TomlStringFrom(ToConfigurationModel(), TomlSerializerOptions.Default);
                await writer.WriteAsync(m);
            } catch { return; }
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
            _audioProvider = string.Empty;
            _videoProvider = string.Empty;
            _cps = 0;
            _autosave = true;
            _autosaveInterval = 300; // 5 minutes
            _repositories = new ObservableCollection<string>();
            _submenuOverrides = new Dictionary<string, string>();
            Read();
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
