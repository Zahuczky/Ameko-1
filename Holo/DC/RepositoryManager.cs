using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Holo.DC
{
    public class RepositoryManager
    {
        private Repository? BaseRepository;
        private readonly Dictionary<string, Repository> RepositoryMap;

        public readonly ObservableCollection<ScriptEntity> RepoScripts;
        public readonly ObservableCollection<Repository> Repositories;

        public async void GatherRepositories(Repository repo)
        {
            if (!RepositoryMap.ContainsKey(repo.Name!))
            {
                RepositoryMap.Add(repo.Name!, repo);
                Repositories.Add(repo);
            }

            foreach (string url in repo.Repositories!)
            {
                var r = await Repository.Build(url);
                if (!RepositoryMap.ContainsKey(r!.Name!))
                {
                    GatherRepositories(r);
                }
            }
        }

        public void GatherRepoScripts()
        {
            RepoScripts.Clear();
            foreach (var repo in RepositoryMap.Values)
                if (repo.Scripts != null)
                    foreach (var script in repo.Scripts)
                        RepoScripts.Add(script);
        }

        private async void SetUpBaseRepository()
        {
            BaseRepository = await Repository.Build("https://gist.githubusercontent.com/9vult/f48f3d03f6b0b913299f27eb0b3a122c/raw/17711621416adda920429ca9d038748fe37b19cd/ameko-base-depctl.json");
            if (BaseRepository == null) return;
            GatherRepositories(BaseRepository);
            GatherRepoScripts();
        }

        public RepositoryManager()
        {
            RepositoryMap = new Dictionary<string, Repository>();
            Repositories = new ObservableCollection<Repository>();
            RepoScripts = new ObservableCollection<ScriptEntity>();
            SetUpBaseRepository();
        }
    }
}
