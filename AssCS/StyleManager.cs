using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
    public class StyleManager
    {
        private readonly List<Style> styles;

        public void Add(Style style)
        {
            styles.Add(style);
        }

        public Style? Get(string name)
        {
            foreach (Style style in styles)
            {
                if (style.Name == name) return style;
            }
            return null;
        }

        public bool Remove(string name)
        {
            foreach (Style style in styles)
            {
                if (style.Name == name) return styles.Remove(style);
            }
            return false;
        }

        public void Clear()
        {
            styles.Clear();
        }

        public void LoadDefault()
        {
            Clear();
            styles.Add(new Style());
        }

        public StyleManager(StyleManager source)
        {
            styles = new List<Style>(source.styles);
        }

        public StyleManager(File source)
        {
            styles = new List<Style>(source.StyleManager.styles);
        }

        public StyleManager()
        {
            styles = new List<Style>();
        }
    }
}
