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
    public class ConfigWindowViewModel : ViewModelBase
    {
        private int cps;
        private bool autosaveEnabled;
        private int autosaveInterval;

        public List<SubmenuOverrideLink> SelectedScripts { get; set; }
        public string OverrideTextBoxText { get; set; }
        public ObservableCollection<SubmenuOverrideLink> OverrideLinks { get; private set; }

        public int Cps
        {
            get => cps;
            set => this.RaiseAndSetIfChanged(ref cps, value);
        }

        public bool AutosaveEnabled
        {
            get => autosaveEnabled;
            set => this.RaiseAndSetIfChanged(ref autosaveEnabled, value);
        }

        public int AutosaveInterval
        {
            get => autosaveInterval;
            set => this.RaiseAndSetIfChanged(ref autosaveInterval, value);
        }

        public ICommand SetOverrideCommand { get; }
        public ICommand RemoveOverrideCommand { get; }
        public ICommand SaveConfigCommand { get; }

        private void GenerateOverrideLinks()
        {
            var currentOverrides = HoloContext.Instance.ConfigurationManager.GetSubmenuOverrides();

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

        public ConfigWindowViewModel()
        {
            OverrideLinks = new ObservableCollection<SubmenuOverrideLink>();
            GenerateOverrideLinks();
            
            SelectedScripts = new List<SubmenuOverrideLink>();
            OverrideTextBoxText = string.Empty;
            Cps = HoloContext.Instance.ConfigurationManager.Cps;
            AutosaveEnabled = HoloContext.Instance.ConfigurationManager.Autosave;
            AutosaveInterval = HoloContext.Instance.ConfigurationManager.AutosaveInterval;

            SetOverrideCommand = ReactiveCommand.Create(() =>
            {
                if (OverrideTextBoxText.Trim().Equals(string.Empty)) return;
                foreach (var script in SelectedScripts)
                {
                    HoloContext.Instance.ConfigurationManager.SetSubmenuOverride(script.QualifiedName, OverrideTextBoxText.Trim());
                }
                GenerateOverrideLinks();
            });

            RemoveOverrideCommand = ReactiveCommand.Create(() =>
            {
                foreach (var script in SelectedScripts)
                {
                    HoloContext.Instance.ConfigurationManager.RemoveSubmenuOverride(script.QualifiedName);
                }
                GenerateOverrideLinks();
            });

            SaveConfigCommand = ReactiveCommand.Create(() =>
            {
                HoloContext.Instance.ConfigurationManager.Cps = Cps;
                HoloContext.Instance.ConfigurationManager.Autosave = AutosaveEnabled;
                HoloContext.Instance.ConfigurationManager.AutosaveInterval = AutosaveInterval;
            });
        }
    }
}
