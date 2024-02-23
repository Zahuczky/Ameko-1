using Holo.DC;
using System;
using System.Diagnostics;
using System.IO;

namespace Holo
{
    public class HoloContext
    {
        private static readonly Lazy<HoloContext> _instance = new Lazy<HoloContext>(() => new HoloContext());
        private static readonly string localappdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create);
        private static readonly string amekoDir = Path.Combine(localappdata, "Ameko");
        private static readonly string holoDir = Path.Combine(localappdata, "Ameko", "Holo");
        
        public static HoloContext Instance => _instance.Value;
        public static string HoloDirectory => holoDir;
        public static string AmekoDirectory => amekoDir;
        
        public PluginHandler PluginHandler { get; }
        public ConfigurationManager ConfigurationManager { get; }
        public GlobalsManager GlobalsManager { get; }
        public RepositoryManager RepositoryManager { get; }
        public Workspace Workspace { get; set; }

        private HoloContext()
        {
            ConfigurationManager = new ConfigurationManager(HoloDirectory);
            GlobalsManager = new GlobalsManager();

            PluginHandler = new PluginHandler(Path.Combine(HoloDirectory, "plugins"));

            RepositoryManager = new RepositoryManager();
            RepositoryManager.LoadUrlList(GlobalsManager.GetRepositories());

            Workspace = new Workspace();
            
            if (!ConfigurationManager.LoadConfiguration())
            {
                ConfigurationManager.GenerateDefaultConfiguration(); // TODO
            }
        }
    }
}
