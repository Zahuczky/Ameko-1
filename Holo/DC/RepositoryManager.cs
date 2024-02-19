using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Holo.DC
{
    public class RepositoryManager
    {
        private Repository BaseRepository;
        private readonly Dictionary<string, Repository> Repositories;

        public void GatherRepositories(Repository repo)
        {
            foreach (string url in repo.Repositories)
            {
                var r = Repository.Build(url);
                if (!Repositories.ContainsKey(r.Name))
                    GatherRepositories(r);
            }
        }

        public RepositoryManager()
        {
            Repositories = new Dictionary<string, Repository>();
            BaseRepository = new Repository(); // TODO
            Repositories.Add(BaseRepository.Name!, BaseRepository);
            GatherRepositories(BaseRepository);
        }
    }
}
