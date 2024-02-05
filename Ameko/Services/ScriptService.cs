using CSScriptLib;
using Holo;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.Services
{
    public class ScriptService
    {
        private static readonly Lazy<ScriptService> _instance = new Lazy<ScriptService>(() => new ScriptService());
        private readonly string scriptRoot;
        private readonly Dictionary<string, HoloScript> scripts;
        
        public static ScriptService Instance => _instance.Value;
        public ObservableCollection<string> LoadedScripts { get; private set; }

        public HoloScript? Get(string name)
        {
            if (scripts.ContainsKey(name))
                return scripts[name];
            return null;
        }

        public async void Reload(bool manual)
        {
            if (!Directory.Exists(scriptRoot))
            {
                Directory.CreateDirectory(scriptRoot);
            }

            scripts.Clear();
            LoadedScripts.Clear();

            foreach (var path in Directory.EnumerateFiles(scriptRoot))
            {
                try
                {
                    if (!Path.GetExtension(path).Equals(".cs")) continue;
                    HoloScript script = CSScript.Evaluator.LoadFile<HoloScript>(path);
                    if (script == null) continue;

                    var name = script.Name;
                    scripts[name] = script;
                    LoadedScripts.Add(name);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    continue;
                }
            }
            if (manual)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("Ameko Script Service", "Scripts have been reloaded.", ButtonEnum.Ok);
                await box.ShowAsync();
            }
        }

        private ScriptService()
        {
            scriptRoot = Path.Combine(HoloContext.HoloDirectory, "scripts");
            LoadedScripts = new ObservableCollection<string>();
            scripts = new Dictionary<string, HoloScript>();
            Reload(false);
        }
    }
}
