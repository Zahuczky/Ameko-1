using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
    public class StyleManager
    {
        private readonly Dictionary<int, Style> styles;

        private int _id = 0;
        public int NextId => _id++;

        public int Set(int id, Style s)
        {
            styles[id] = s;
            return s.Id;
        }

        public int Set(Style s)
        {
            styles[s.Id] = s;
            return s.Id;
        }

        public int Set(Commit<Style> commit)
        {
            var committedStyle = commit.Snapshot;
            styles[committedStyle.Id] = committedStyle;
            return committedStyle.Id;
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
                if (style.Name == name) return styles.Remove(style.Id);
            }
            return false;
        }

        public bool Remove(int id)
        {
            return styles.Remove(id);
        }

        public void Clear()
        {
            styles.Clear();
        }

        public void LoadDefault()
        {
            Clear();
            Set(new Style(NextId));
        }

        public StyleManager(StyleManager source)
        {
            styles = new Dictionary<int, Style>(source.styles);
        }

        public StyleManager(File source)
        {
            styles = new Dictionary<int, Style>(source.StyleManager.styles);
        }

        public StyleManager()
        {
            styles = new Dictionary<int, Style>();
        }
    }
}
