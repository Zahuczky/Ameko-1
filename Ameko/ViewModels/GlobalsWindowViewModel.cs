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
    public class GlobalsWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Tuple<string, string>> Overrides { get; private set; }
        public ObservableCollection<string> LoadedScripts { get; private set; }
        public List<Tuple<string, string>> SelectedOverrides { get; set; }
        public string OverrideTextBoxText { get; set; }
        private string _selectedScript;
        public string SelectedScript
        {
            get => _selectedScript;
            set => this.RaiseAndSetIfChanged(ref _selectedScript, value);
        }

        public ICommand SetOverrideCommand { get; }
        public ICommand RemoveOverrideCommand { get; }

        public GlobalsWindowViewModel()
        {
            Overrides = new ObservableCollection<Tuple<string, string>>(HoloContext.Instance.GlobalsManager.GetSubmenuOverrides());
            LoadedScripts = new ObservableCollection<string>(ScriptService.Instance.LoadedScripts.Select(s => s.Item1));
            
            SelectedOverrides = new List<Tuple<string, string>>();
            _selectedScript = LoadedScripts.First();
            OverrideTextBoxText = string.Empty;

            SetOverrideCommand = ReactiveCommand.Create(() =>
            {
                if (OverrideTextBoxText.Trim().Equals(string.Empty)) return;

                HoloContext.Instance.GlobalsManager.SetSubmenuOverride(SelectedScript, OverrideTextBoxText.Trim());
                Overrides.Clear();
                Overrides.AddRange(HoloContext.Instance.GlobalsManager.GetSubmenuOverrides());
            });

            RemoveOverrideCommand = ReactiveCommand.Create(() =>
            {
                foreach (var script in SelectedOverrides)
                {
                    HoloContext.Instance.GlobalsManager.RemoveSubmenuOverride(script.Item1);
                }

                Overrides.Clear();
                Overrides.AddRange(HoloContext.Instance.GlobalsManager.GetSubmenuOverrides());
            });
        }
    }
}
