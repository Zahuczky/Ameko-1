using Ameko.DataModels;
using Ameko.Services;
using DynamicData;
using Holo;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ameko.ViewModels
{
    public class KeybindsWindowViewModel
    {
        public ObservableCollection<KeybindLink> GlobalBinds { get; private set; }
        public ObservableCollection<KeybindLink> GridBinds { get; private set; }
        public ObservableCollection<KeybindLink> EditBinds { get; private set; }
        public ObservableCollection<KeybindLink> AudioBinds { get; private set; }
        public ObservableCollection<KeybindLink> VideoBinds { get; private set; }

        public ICommand SaveKeybindsCommand { get; }

        public KeybindsWindowViewModel()
        {
            var reg = HoloContext.Instance.ConfigurationManager.KeybindsRegistry;
            var scriptNames = ScriptService.Instance.LoadedScripts
                .Select(script => script.Item1)
                .Concat(ScriptService.Instance.FunctionMap.Values.SelectMany(methods => methods))
                .ToList();

            var global = KeybindsRegistry.GetBuiltins(KeybindContext.GLOBAL).Concat(scriptNames);
            var grid = KeybindsRegistry.GetBuiltins(KeybindContext.GRID).Concat(scriptNames);
            var edit = KeybindsRegistry.GetBuiltins(KeybindContext.EDIT).Concat(scriptNames);
            var audio = KeybindsRegistry.GetBuiltins(KeybindContext.AUDIO).Concat(scriptNames);
            var video = KeybindsRegistry.GetBuiltins(KeybindContext.VIDEO).Concat(scriptNames);

            GlobalBinds = new ObservableCollection<KeybindLink>(global.Select(n => new KeybindLink { Key = n, Value = reg.GlobalBinds.GetValueOrDefault(n, string.Empty) }));
            GridBinds = new ObservableCollection<KeybindLink>(grid.Select(n => new KeybindLink { Key = n, Value = reg.GridBinds.GetValueOrDefault(n, string.Empty) }));
            EditBinds = new ObservableCollection<KeybindLink>(edit.Select(n => new KeybindLink { Key = n, Value = reg.EditBinds.GetValueOrDefault(n, string.Empty) }));
            AudioBinds = new ObservableCollection<KeybindLink>(audio.Select(n => new KeybindLink { Key = n, Value = reg.AudioBinds.GetValueOrDefault(n, string.Empty) }));
            VideoBinds = new ObservableCollection<KeybindLink>(video.Select(n => new KeybindLink { Key = n, Value = reg.VideoBinds.GetValueOrDefault(n, string.Empty) }));

            SaveKeybindsCommand = ReactiveCommand.Create(() =>
            {
                HoloContext.Instance.ConfigurationManager.SetKeybinds(
                    KeybindContext.GLOBAL,
                    new Dictionary<string, string>(GlobalBinds.Select(k => new KeyValuePair<string, string>(k.Key, k.Value)))
                );
                HoloContext.Instance.ConfigurationManager.SetKeybinds(
                    KeybindContext.GRID,
                    new Dictionary<string, string>(GridBinds.Select(k => new KeyValuePair<string, string>(k.Key, k.Value)))
                );
                HoloContext.Instance.ConfigurationManager.SetKeybinds(
                    KeybindContext.EDIT,
                    new Dictionary<string, string>(EditBinds.Select(k => new KeyValuePair<string, string>(k.Key, k.Value)))
                );
                HoloContext.Instance.ConfigurationManager.SetKeybinds(
                    KeybindContext.AUDIO,
                    new Dictionary<string, string>(AudioBinds.Select(k => new KeyValuePair<string, string>(k.Key, k.Value)))
                );
                HoloContext.Instance.ConfigurationManager.SetKeybinds(
                    KeybindContext.VIDEO,
                    new Dictionary<string, string>(VideoBinds.Select(k => new KeyValuePair<string, string>(k.Key, k.Value)))
                );
            });
        }
    }
}
