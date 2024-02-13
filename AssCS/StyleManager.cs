using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AssCS
{
    /// <summary>
    /// Manages the styles in a file
    /// </summary>
    public class StyleManager
    {
        private readonly Dictionary<int, Style> styles;

        public ObservableCollection<string> StyleNames { get; private set; }

        private int _id = 0;
        public int NextId => _id++;

        public int Set(int id, Style s)
        {
            styles[id] = s;
            StyleNames.Add(s.Name);
            return s.Id;
        }

        public int Set(Style s)
        {
            styles[s.Id] = s;
            StyleNames.Add(s.Name);
            return s.Id;
        }

        public int SetOrReplace(Style s)
        {
            if (StyleNames.Contains(s.Name))
            {
                var ov = styles.Values.Where(st => st.Name.Equals(s.Name)).ToList().First();
                s.Id = ov.Id;
                styles[s.Id] = s;
                return s.Id;
            }
            else
            {
                return Set(s.Id, s);
            }
        }

        public Style? Get(string name)
        {
            foreach (Style style in styles.Values)
            {
                if (style.Name == name) return style;
            }
            return null;
        }

        public Style? Get(int id)
        {
            if (styles.ContainsKey(id)) return styles[id];
            return null;
        }

        public bool Remove(string name)
        {
            foreach (Style style in styles.Values)
            {
                if (style.Name == name)
                {
                    StyleNames.Remove(name);
                    return styles.Remove(style.Id);
                }
            }
            return false;
        }

        public bool Remove(int id)
        {
            if (styles.ContainsKey(id))
            {
                var style = styles[id];
                StyleNames.Remove(style.Name);
                return styles.Remove(id);
            }
            return false;
        }

        public void Clear()
        {
            styles.Clear();
            StyleNames.Clear();
        }

        public List<Style> GetAll()
        {
            return styles.Values.ToList();
        }

        public void LoadDefault()
        {
            Clear();
            Set(new Style(NextId));
        }

        public StyleManager(StyleManager source)
        {
            styles = new Dictionary<int, Style>(source.styles);
            StyleNames = new ObservableCollection<string>(source.StyleNames);
        }

        public StyleManager(File source)
        {
            styles = new Dictionary<int, Style>(source.StyleManager.styles);
            StyleNames = new ObservableCollection<string>(source.StyleManager.StyleNames);
        }

        public StyleManager()
        {
            styles = new Dictionary<int, Style>();
            StyleNames = new ObservableCollection<string>();
        }
    }
}
