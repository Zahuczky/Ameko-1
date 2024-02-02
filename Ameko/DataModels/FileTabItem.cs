using Holo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.DataModels
{
    public class FileTabItem
    {
        public string Name { get; set; }
        public FileWrapper File { get; }
        public int ID { get; }

        public FileTabItem(string name, FileWrapper file)
        {
            Name = name;
            File = file;
            ID = file.ID;
        }
    }
}
