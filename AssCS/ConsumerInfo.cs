using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
    public class ConsumerInfo
    {
        public string Name { get; }
        public string Version { get; }
        public string Website { get; }

        public ConsumerInfo(string name, string version, string website)
        {
            Name = name;
            Version = version;
            Website = website;
        }
    }
}
