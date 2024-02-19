using System;
using System.Collections.Generic;
using System.Text;

namespace Holo.DC
{
    public struct Repository
    {
        public string Name;
        public string Description;
        public double Version;
        public string Maintainer;
        public List<string> Repositories;
        public List<ScriptEntity> Scripts;

        public static Repository Build(string url)
        {
            return new Repository(); // TODO
        }
    }

    public struct ScriptEntity
    {
        public string Name;
        public string QualifiedName;
        public string Description;
        public string Author;
        public double CurrentVersion;
        public List<string> Dependencies;
        public string Url;
    }
}
