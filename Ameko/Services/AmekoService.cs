using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.Services
{
    public class AmekoService
    {
        public static string VERSION_BUG
        {
            get
            {
                var version = ThisAssembly.Git.SemVer.Label;
                if (!version.Equals(string.Empty)) version = $"{version} ";
                var position = $"{ThisAssembly.Git.Branch}-{ThisAssembly.Git.Commit}";
                return $"{version}@ {position}";
            }
        }
    }
}
