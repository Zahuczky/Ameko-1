using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tomlyn;

namespace Holo
{
    public class ConfigurationManager
    {
        private readonly string configDirectory;
        private HoloConfiguration? _config;
        public HoloConfiguration? CurrentConfiguration => _config;

        public bool LoadConfiguration()
        {
            var configFilePath = Path.Join(configDirectory, "config.toml");
            if (!File.Exists(configFilePath)) return false;

            try
            {
                using var reader = new StreamReader(configFilePath);
                var configContents = reader.ReadToEnd();
                _config = Toml.ToModel<HoloConfiguration>(configContents);
                return true;
            }
            catch { return false; }
        }

        public bool WriteConfiguration()
        {
            if (CurrentConfiguration == null) return false;
            var configFilePath = Path.Join(configDirectory, "config.toml");
            try
            {
                using var writer = new StreamWriter(configFilePath);
                writer.Write(Toml.FromModel(CurrentConfiguration));
                return true;
            } catch { return false; }
        }

        public bool GenerateDefaultConfiguration()
        {
            // TODO 
            return true;
        }

        public ConfigurationManager(string configDir)
        {
            configDirectory = configDir;
        }
    }

    public class HoloConfiguration
    {
        public string? AudioProvider;
        public string? VideoProvider;
        public PanelLayoutConfiguration? PanelConfig;
    }

    public class PanelLayoutConfiguration
    {
        public int? Rows;
        public int? Columns;
        public int[]? Widths;
        public int[]? Heights;
    }
}
