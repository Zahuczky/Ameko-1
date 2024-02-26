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
        private readonly Dictionary<string, string[]> functions;
        
        public static ScriptService Instance => _instance.Value;
        public ObservableCollection<Tuple<string, string>> LoadedScripts { get; private set; }

        public List<HoloScript> HoloScripts => new List<HoloScript>(scripts.Values);
        public Dictionary<string, string[]> FunctionMap => new Dictionary<string, string[]>(functions);

        /// <summary>
        /// Get a script by its qualified name
        /// </summary>
        /// <param name="qualifiedName"></param>
        /// <returns></returns>
        public HoloScript? Get(string qualifiedName)
        {
            if (scripts.TryGetValue(qualifiedName, out HoloScript? value))
                return value;
            return null;
        }

        /// <summary>
        /// Execute a script or function
        /// </summary>
        /// <remarks>
        /// Execution as a script will be attempted first, followed by as a function
        /// </remarks>
        /// <param name="qname">Qualified name of the script or function</param>
        /// <returns>Execution result of the script or function</returns>
        public async Task<ExecutionResult> ExecuteScriptOrFunction(string qname)
        {
            // Try running as a script
            if (scripts.TryGetValue(qname, out HoloScript? script))
                return await script.Execute();

            // Try running as a method
            var scriptName = qname[..qname.LastIndexOf('.')];
            if (scripts.TryGetValue(scriptName, out HoloScript? methodScript))
                return await methodScript.Execute(qname);

            // Neither of these worked, fail
            return new ExecutionResult { Status = ExecutionStatus.Failure, Message = $"The script or function {qname} could not be found." };
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
            functions.Clear();
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

                    if (script.ExportedMethods != null)
                        functions[qname] = script.ExportedMethods;
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
            scriptRoot = Path.Combine(HoloContext.Directories.HoloDataHome, "scripts");
            LoadedScripts = new ObservableCollection<Tuple<string, string>>();
            scripts = new Dictionary<string, HoloScript>();
            functions = new Dictionary<string, string[]>();
            Reload(false);
        }
    }
}
