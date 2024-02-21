using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tomlet;

namespace Holo
{
    public class ConfigurationManager
    {
        private readonly string configDirectory;
        private Configuration? _config;
        public Configuration? CurrentConfiguration => _config;

        public bool LoadConfiguration()
        {
            var configFilePath = Path.Join(configDirectory, "config.toml");
            if (!File.Exists(configFilePath)) return false;

            try
            {
                using var reader = new StreamReader(configFilePath);
                var configContents = reader.ReadToEnd();
                _config = TomletMain.To<Configuration>(configContents);
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
                writer.Write(TomletMain.TomlStringFrom(CurrentConfiguration));
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

    public class Configuration
    {
        public string? AudioProvider;
        public string? VideoProvider;
        public int CpsThreshold;
    }

}
