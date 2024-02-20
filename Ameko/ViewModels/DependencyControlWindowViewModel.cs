using Ameko.Services;
using DynamicData;
using Holo;
using Holo.DC;
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
    public class DependencyControlWindowViewModel : ViewModelBase
    {
        public ObservableCollection<ScriptEntity> InstalledScripts { get; }
        public ObservableCollection<Repository> Repositories { get; }
        public ObservableCollection<ScriptEntity> RepoScripts { get; }
        public ObservableCollection<ScriptEntity> UpdateCandidates { get; }

        public List<ScriptEntity> SelectedRepoScripts { get; }
        public List<ScriptEntity> SelectedInstalledScripts { get; }

        public ICommand InstallScriptCommand { get; }
        public ICommand UninstallScriptCommand { get; }
        public ICommand UpdateScriptCommand { get; }
        public ICommand UpdateAllCommand { get; }

        private void PopulateInstalledScriptsList()
        {
            var scriptEntities = ScriptService.Instance.LoadedScripts.Select(s =>
            {
                var realScript = ScriptService.Instance.Get(s.Item1);
                return new ScriptEntity
                {
                    Name = realScript?.Name,
                    QualifiedName = realScript?.QualifiedName,
                    Author = realScript?.Author,
                    CurrentVersion = realScript?.Version ?? 0,
                    Description = realScript?.Description
                };
            }).ToList();

            InstalledScripts.Clear();
            InstalledScripts.AddRange(scriptEntities);

            PopulateRepoScriptsList();
            PopulateUpdateCandidatesList();
        }

        private void PopulateUpdateCandidatesList()
        {
            UpdateCandidates.Clear();
            var candidates = DCScriptManager.GetUpdateCandidates(HoloContext.Instance.RepositoryManager.RepoScripts.ToList(), InstalledScripts.ToList());
            UpdateCandidates.AddRange(candidates);
        }

        private void PopulateRepoScriptsList()
        {
            RepoScripts.Clear();
            RepoScripts.AddRange(
                HoloContext.Instance.RepositoryManager.RepoScripts.Where(s => !DCScriptManager.IsDCScriptInstalled(s.QualifiedName ?? ""))
            );
        }

        private async Task<bool> TryUpdate(ScriptEntity script)
        {
            if (script.QualifiedName == null) return false;

            // Check if the script is an update candidate
            var candidates = UpdateCandidates.Where(s => s.QualifiedName!.Equals(script.QualifiedName));
            if (candidates.Any())
            {
                var serverScript = candidates.First();
                if (serverScript.Url == null) return false;
                return await DCScriptManager.UpdateDCScript(script.QualifiedName,serverScript.Url);
            }
            return false;
        }

        public DependencyControlWindowViewModel()
        {
            SelectedRepoScripts = new List<ScriptEntity>();
            SelectedInstalledScripts = new List<ScriptEntity>();

            Repositories = HoloContext.Instance.RepositoryManager.Repositories;

            RepoScripts = new ObservableCollection<ScriptEntity>();
            InstalledScripts = new ObservableCollection<ScriptEntity>();
            UpdateCandidates = new ObservableCollection<ScriptEntity>();
            PopulateInstalledScriptsList();

            InstallScriptCommand = ReactiveCommand.Create(async () =>
            {
                foreach (var script in SelectedRepoScripts)
                {
                    if (script.QualifiedName != null && script.Url != null)
                        await DCScriptManager.InstallDCScript(script.QualifiedName, script.Url);
                }
                ScriptService.Instance.Reload(false);
                PopulateInstalledScriptsList();
            });

            UninstallScriptCommand = ReactiveCommand.Create(() =>
            {
                foreach (var script in SelectedInstalledScripts)
                {
                    if (script.QualifiedName != null)
                        DCScriptManager.UninstallDCScript(script.QualifiedName);
                }
                ScriptService.Instance.Reload(false);
                PopulateInstalledScriptsList();
            });

            UpdateScriptCommand = ReactiveCommand.Create(async () =>
            {
                foreach (var script in SelectedInstalledScripts)
                {
                    await TryUpdate(script);
                }
                ScriptService.Instance.Reload(false);
                PopulateInstalledScriptsList();
            });

            UpdateAllCommand = ReactiveCommand.Create(async () =>
            {
                foreach (var script in InstalledScripts)
                {
                    await TryUpdate(script);
                }
                ScriptService.Instance.Reload(false);
                PopulateInstalledScriptsList();
            });
        }
    }
}
