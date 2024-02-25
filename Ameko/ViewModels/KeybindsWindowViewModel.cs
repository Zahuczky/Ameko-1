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
    public class KeybindsWindowViewModel : ViewModelBase
    {
        private readonly SourceCache<KeybindLink, string> _globalCache = new(x => x.Key);
        private readonly SourceCache<KeybindLink, string> _gridCache = new(x => x.Key);
        private readonly SourceCache<KeybindLink, string> _editCache = new(x => x.Key);
        private readonly SourceCache<KeybindLink, string> _audioCache = new(x => x.Key);
        private readonly SourceCache<KeybindLink, string> _videoCache = new(x => x.Key);

        private readonly ReadOnlyObservableCollection<KeybindLink> _globalBinds;
        private readonly ReadOnlyObservableCollection<KeybindLink> _gridBinds;
        private readonly ReadOnlyObservableCollection<KeybindLink> _editBinds;
        private readonly ReadOnlyObservableCollection<KeybindLink> _audioBinds;
        private readonly ReadOnlyObservableCollection<KeybindLink> _videoBinds;

        public ReadOnlyObservableCollection<KeybindLink> GlobalBinds => _globalBinds;
        public ReadOnlyObservableCollection<KeybindLink> GridBinds => _gridBinds;
        public ReadOnlyObservableCollection<KeybindLink> EditBinds => _editBinds;
        public ReadOnlyObservableCollection<KeybindLink> AudioBinds => _audioBinds;
        public ReadOnlyObservableCollection<KeybindLink> VideoBinds => _videoBinds;

        private string _filter = string.Empty;
        public string Filter
        {
            get => _filter;
            set 
            {
                this.RaiseAndSetIfChanged(ref _filter, value);
                _globalCache.Refresh();
                _gridCache.Refresh();
                _editCache.Refresh();
                _audioCache.Refresh();
                _videoCache.Refresh();
            }
        }

        public ICommand SaveKeybindsCommand { get; }

        private ReadOnlyObservableCollection<KeybindLink> ROOCGenerator(IEnumerable<string> targets, Dictionary<string, string> sources)
        {
            return new ReadOnlyObservableCollection<KeybindLink>(
                new ObservableCollection<KeybindLink>(
                    targets.Select(n => new KeybindLink { Key = n, Value = sources.GetValueOrDefault(n, string.Empty) })
                )
            );
        }

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

            _globalBinds = ROOCGenerator(global, reg.GlobalBinds);
            _gridBinds = ROOCGenerator(grid, reg.GridBinds);
            _editBinds = ROOCGenerator(edit, reg.EditBinds);
            _audioBinds = ROOCGenerator(audio, reg.AudioBinds);
            _videoBinds = ROOCGenerator(video, reg.VideoBinds);

            _globalCache.AddOrUpdate(_globalBinds);
            _gridCache.AddOrUpdate(_gridBinds);
            _editCache.AddOrUpdate(_editBinds);
            _audioCache.AddOrUpdate(_audioBinds);
            _videoCache.AddOrUpdate(_videoBinds);

            _globalCache.Connect()
                .Filter(x => Filter.Equals(string.Empty) || x.Key.Contains(Filter, StringComparison.CurrentCultureIgnoreCase) || x.Value.Contains(Filter, StringComparison.CurrentCultureIgnoreCase))
                .Bind(out _globalBinds)
                .Subscribe();
            _gridCache.Connect()
                .Filter(x => Filter.Equals(string.Empty) || x.Key.Contains(Filter, StringComparison.CurrentCultureIgnoreCase) || x.Value.Contains(Filter, StringComparison.CurrentCultureIgnoreCase))
                .Bind(out _gridBinds)
                .Subscribe();
            _editCache.Connect()
                .Filter(x => Filter.Equals(string.Empty) || x.Key.Contains(Filter, StringComparison.CurrentCultureIgnoreCase) || x.Value.Contains(Filter, StringComparison.CurrentCultureIgnoreCase))
                .Bind(out _editBinds)
                .Subscribe();
            _audioCache.Connect()
                .Filter(x => Filter.Equals(string.Empty) || x.Key.Contains(Filter, StringComparison.CurrentCultureIgnoreCase) || x.Value.Contains(Filter, StringComparison.CurrentCultureIgnoreCase))
                .Bind(out _audioBinds)
                .Subscribe();
            _videoCache.Connect()
                .Filter(x => Filter.Equals(string.Empty) || x.Key.Contains(Filter, StringComparison.CurrentCultureIgnoreCase) || x.Value.Contains(Filter, StringComparison.CurrentCultureIgnoreCase))
                .Bind(out _videoBinds)
                .Subscribe();

            SaveKeybindsCommand = ReactiveCommand.Create(() =>
            {
                HoloContext.Instance.ConfigurationManager.SetKeybinds(
                    KeybindContext.GLOBAL,
                    new Dictionary<string, string>(_globalBinds.Select(k => new KeyValuePair<string, string>(k.Key, k.Value)))
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
