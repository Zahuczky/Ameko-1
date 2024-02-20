using Ameko.Services;
using Holo;
using Holo.DC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.ViewModels
{
    public class DependencyControlWindowViewModel : ViewModelBase
    {
        public ObservableCollection<ScriptEntity> InstalledScripts { get; }
        public ObservableCollection<Repository> Repositories { get; }
        public ObservableCollection<ScriptEntity> RepoScripts { get; }

        public DependencyControlWindowViewModel()
        {
            InstalledScripts = new ObservableCollection<ScriptEntity>( 
                ScriptService.Instance.LoadedScripts.Select(s =>
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
                }).ToList()
            );
            Repositories = HoloContext.Instance.RepositoryManager.Repositories;
            RepoScripts = HoloContext.Instance.RepositoryManager.RepoScripts;
        }
    }
}
