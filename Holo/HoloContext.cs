using Holo.DC;
using System;
using System.Diagnostics;
using System.IO;

namespace Holo
{
    public class HoloContext
    {
        private static readonly Lazy<HoloContext> _instance = new Lazy<HoloContext>(() => new HoloContext());
        private static readonly string localappdata = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "Ameko", "Holo");
        
        public static HoloContext Instance => _instance.Value;
        public static string HoloDirectory => localappdata;
        
        public PluginHandler PluginHandler { get; }
        public ConfigurationManager ConfigurationManager { get; }
        public GlobalsManager GlobalsManager { get; }
        public RepositoryManager RepositoryManager { get; }
        public Workspace Workspace { get; set; }

        private HoloContext()
        {
            PluginHandler = new PluginHandler(Path.Combine(HoloDirectory, "plugins"));
            ConfigurationManager = new ConfigurationManager(HoloDirectory);
            RepositoryManager = new RepositoryManager();
            GlobalsManager = new GlobalsManager();
            Workspace = new Workspace();
            
            if (!ConfigurationManager.LoadConfiguration())
            {
                ConfigurationManager.GenerateDefaultConfiguration(); // TODO
            }
        }
    }
}
