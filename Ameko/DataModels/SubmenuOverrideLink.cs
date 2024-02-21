using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.DataModels
{
    public struct SubmenuOverrideLink
    {
        public string Name { get; set; }
        public string QualifiedName { get; set; }
        public string? SubmenuOverride { get; set; }
    }
}
