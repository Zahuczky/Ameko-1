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

        public List<ScriptEntity> SelectedRepoScripts { get; }
        public List<ScriptEntity> SelectedInstalledScripts { get; }

        public ICommand InstallScriptCommand { get; }
        public ICommand UninstallScriptCommand { get; }

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
        }

        public DependencyControlWindowViewModel()
        {
            SelectedRepoScripts = new List<ScriptEntity>();
            SelectedInstalledScripts = new List<ScriptEntity>();
            InstalledScripts = new ObservableCollection<ScriptEntity>();
            PopulateInstalledScriptsList();

            Repositories = HoloContext.Instance.RepositoryManager.Repositories;
            RepoScripts = HoloContext.Instance.RepositoryManager.RepoScripts;

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
        }
    }
}
