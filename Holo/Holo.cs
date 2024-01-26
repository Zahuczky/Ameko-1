using System;
using System.IO;

namespace Holo
{
    public class Holo
    {
        private static readonly Lazy<Holo> _instance = new Lazy<Holo>(() => new Holo());
        public static readonly string localappdata = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "Ameko", "Holo");
        
        public static Holo Instance => _instance.Value;
        public static string HoloDirectory => localappdata;
        
        public PluginHandler PluginHandler { get; }
        public ConfigurationManager ConfigurationManager { get; }
        public Workspace Workspace { get; set; }

        private Holo()
        {
            PluginHandler = new PluginHandler(Path.Combine(HoloDirectory, "plugins"));
            ConfigurationManager = new ConfigurationManager(HoloDirectory);
            Workspace = new Workspace();
            
            if (!ConfigurationManager.LoadConfiguration())
            {
                ConfigurationManager.GenerateDefaultConfiguration(); // TODO
            }
        }
    }
}
