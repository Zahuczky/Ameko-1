using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
    public class ProjectProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public ProjectProperty(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
