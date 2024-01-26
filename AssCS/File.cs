using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
    public class File
    {
        public FileVersion Version { get; set; }
        public InfoManager InfoManager { get; }
        public StyleManager StyleManager { get; }
        public EventManager EventManager { get; }
        public AttachmentManager AttachmentManager { get; }
        public ExtradataManager ExtradataManager { get; }
        public PropertiesManager PropertiesManager { get; }
        public HistoryManager HistoryManager { get; }

        public void LoadDefault()
        {
            Version = FileVersion.V400P;
            InfoManager.LoadDefault();
            StyleManager.LoadDefault();
            EventManager.LoadDefault();
            PropertiesManager.LoadDefault();
        }

        public File(File source)
        {
            Version = source.Version;
            InfoManager = new InfoManager(source);
            StyleManager = new StyleManager(source);
            EventManager = new EventManager(source);
            AttachmentManager = new AttachmentManager(source);
            ExtradataManager = new ExtradataManager(source);
            PropertiesManager = new PropertiesManager(source);
            HistoryManager = new HistoryManager();
        }

        public File()
        {
            Version = FileVersion.V400P;
            InfoManager = new InfoManager();
            StyleManager = new StyleManager();
            EventManager = new EventManager();
            AttachmentManager = new AttachmentManager();
            ExtradataManager = new ExtradataManager();
            PropertiesManager = new PropertiesManager();
            HistoryManager = new HistoryManager();
        }
    }

    public enum FileVersion
    {
        V400 = 0,
        V400P = 1,
        V400PP = 2,
        UNKNOWN = -1
    }
}
