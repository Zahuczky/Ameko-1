using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
    public class File
    {
        public InfoManager InfoManager { get; }
        public StyleManager StyleManager { get; }
        public EventManager EventManager { get; }
        public AttachmentManager AttachmentManager { get; }
        public ExtradataManager ExtradataManager { get; }
        public PropertiesManager PropertiesManager { get; }

        public void LoadDefault()
        {
            InfoManager.LoadDefault();
            StyleManager.LoadDefault();
            EventManager.LoadDefault();
        }

        public File(File source)
        {
            InfoManager = new InfoManager(source);
            StyleManager = new StyleManager(source);
            EventManager = new EventManager(source);
            AttachmentManager = new AttachmentManager(source);
            ExtradataManager = new ExtradataManager(source);
            PropertiesManager = new PropertiesManager(source);
        }

        public File()
        {
            InfoManager = new InfoManager();
            StyleManager = new StyleManager();
            EventManager = new EventManager();
            AttachmentManager = new AttachmentManager();
            ExtradataManager = new ExtradataManager();
            PropertiesManager = new PropertiesManager();
        }
    }
}
