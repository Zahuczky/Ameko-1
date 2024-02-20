using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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
using System.Windows.Input;

namespace Ameko.Services
{
    public class ScriptService
    {
        private static readonly Lazy<ScriptService> _instance = new Lazy<ScriptService>(() => new ScriptService());
        private readonly string scriptRoot;
        private readonly Dictionary<string, HoloScript> scripts;
        
        public static ScriptService Instance => _instance.Value;
        public ObservableCollection<Tuple<string, string>> LoadedScripts { get; private set; }

        /// <summary>
        /// Get a script by its qualified name
        /// </summary>
        /// <param name="qualifiedName"></param>
        /// <returns></returns>
        public HoloScript? Get(string qualifiedName)
        {
            if (scripts.ContainsKey(qualifiedName))
                return scripts[qualifiedName];
            return null;
        }

        /// <summary>
        /// Reload the scripts
        /// </summary>
        /// <param name="manual">Was the reload manually triggered</param>
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
                    var qname = script.QualifiedName;
                    scripts[qname] = script;
                    LoadedScripts.Add(new Tuple<string, string>(qname, name));
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

        public List<TemplatedControl> GetScriptsMenuItems(ICommand ActivateScriptCommand)
        {
            var list = new List<TemplatedControl>();
            var scriptSvg = new Avalonia.Svg.Skia.Svg(new Uri("avares://Ameko/Assets/B5/code-slash.svg")) { Path = new Uri("avares://Ameko/Assets/B5/code-slash.svg").LocalPath };

            var subMenuParents = new List<MenuItem>();

            foreach (var script in LoadedScripts)
            {
                var mi = new MenuItem
                {
                    Header = script.Item2,
                    Command = ActivateScriptCommand,
                    CommandParameter = script.Item1,
                    Icon = scriptSvg
                };

                string? submenuName = null;
                if (false) // TODO: Configuration-based overrides: QualifiedName → MenuName link
                {
                    // if configuration.menuoverrides.contains(Item1) submenuName = blah blah
                }
                else
                {
                    var real = Instance.Get(script.Item1);
                    if (real != null)
                        submenuName = real.SubmenuName; // may be null
                }

                if (submenuName != null)
                {
                    // See if there's already a submenu with that name, and if so, add to it
                    var potentialParents = subMenuParents.Where(p => p.Header != null && p.Header.Equals(submenuName));
                    if (potentialParents.Any())
                    {
                        potentialParents.First().Items.Add(mi);
                    }
                    else
                    {
                        // No submenu exists, so add a new one
                        var parent = new MenuItem { Header = submenuName };
                        parent.Items.Add(mi);
                        subMenuParents.Add(parent);
                    }
                }
                else
                {
                    // Submenu Name is null, add to main
                    list.Add(mi);
                }

            }

            if (subMenuParents.Count > 0)
                list.InsertRange(0, subMenuParents.ToArray());
            return list;
        }

        private ScriptService()
        {
            scriptRoot = Path.Combine(HoloContext.HoloDirectory, "scripts");
            LoadedScripts = new ObservableCollection<Tuple<string, string>>();
            scripts = new Dictionary<string, HoloScript>();
            Reload(false);
        }
    }
}
