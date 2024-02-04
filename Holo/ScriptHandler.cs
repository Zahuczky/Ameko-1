using CSScriptLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Holo
{
    public class ScriptHandler
    {
        private readonly string scriptRoot;
        private readonly Dictionary<string, HoloScript> scripts;

        public ObservableCollection<string> LoadedScripts { get; private set; }

        public HoloScript? Get(string name)
        {
            if (scripts.ContainsKey(name))
                return scripts[name];
            return null;
        }

        public bool LoadAll()
        {
            if (!Directory.Exists(scriptRoot))
            {
                Directory.CreateDirectory(scriptRoot);
                return false;
            }

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
            return true;
        }

        public ScriptHandler(string scriptRoot)
        {
            this.scriptRoot = scriptRoot;
            LoadedScripts = new ObservableCollection<string>();
            scripts = new Dictionary<string, HoloScript>();
        }
    }
}
