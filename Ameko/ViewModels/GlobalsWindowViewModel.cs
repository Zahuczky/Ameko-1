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
    public class GlobalsWindowViewModel : ViewModelBase
    {
        public List<SubmenuOverrideLink> SelectedScripts { get; set; }
        public string OverrideTextBoxText { get; set; }

        public ObservableCollection<SubmenuOverrideLink> OverrideLinks { get; private set; }

        public ICommand SetOverrideCommand { get; }
        public ICommand RemoveOverrideCommand { get; }

        private void GenerateOverrideLinks()
        {
            var currentOverrides = HoloContext.Instance.GlobalsManager.GetSubmenuOverrides();

            OverrideLinks.Clear();
            foreach (var script in ScriptService.Instance.LoadedScripts)
            {
                string? setOverride = null;
                var ovrs = currentOverrides.Where(o => o.Item1.Equals(script.Item1));
                if (ovrs.Any()) setOverride = ovrs.First().Item2;

                var mol = new SubmenuOverrideLink
                {
                    Name = script.Item2,
                    QualifiedName = script.Item1,
                    SubmenuOverride = setOverride
                };
                OverrideLinks.Add(mol);
            }
        }

        public GlobalsWindowViewModel()
        {
            OverrideLinks = new ObservableCollection<SubmenuOverrideLink>();
            GenerateOverrideLinks();
            
            SelectedScripts = new List<SubmenuOverrideLink>();
            OverrideTextBoxText = string.Empty;

            SetOverrideCommand = ReactiveCommand.Create(() =>
            {
                if (OverrideTextBoxText.Trim().Equals(string.Empty)) return;
                foreach (var script in SelectedScripts)
                {
                    HoloContext.Instance.GlobalsManager.SetSubmenuOverride(script.QualifiedName, OverrideTextBoxText.Trim());
                }
                GenerateOverrideLinks();
            });

            RemoveOverrideCommand = ReactiveCommand.Create(() =>
            {
                foreach (var script in SelectedScripts)
                {
                    HoloContext.Instance.GlobalsManager.RemoveSubmenuOverride(script.QualifiedName);
                }
                GenerateOverrideLinks();
            });
        }
    }
}
