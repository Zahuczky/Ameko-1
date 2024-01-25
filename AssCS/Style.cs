using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
    public class Style : AssComponent
    {
        public string Name { get; set; }
        public string Font {  get; set; }
        public double FontSize { get; set; }
        public Color Primary {  get; set; }
        public Color Secondary { get; set; }
        public Color Outline { get; set; }
        public Color Shadow { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool Underline { get; set; }
        public bool Strikeout { get; set; }
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
        public double Spacing {  get; set; }
        public double Angle { get; set; }
        public int BorderStyle { get; set; }
        public double BorderThickness { get; set; }
        public double ShadowDistance { get; set; }
        public int Alignment { get; set; }
        public Margins Margins { get; set; }
        int Encoding { get; set; }

        public string AsAss()
        {
            var cleanName = Name.Replace(',', ';');
            var cleanFont = Font.Replace(',', ';');
            return $"Style: {cleanName},{cleanFont},{FontSize},{Primary.AsAss()},{Secondary.AsAss()},{Outline.AsAss()},{Shadow.AsAss()}," +
                $"{(Bold ? -1 : 0)},{(Italic ? -1 : 0)},{(Underline ? -1 : 0)},{(Strikeout ? -1 : 0)}," +
                $"{ScaleX},{ScaleY},{Spacing},{Angle},{BorderStyle},{BorderThickness},{ShadowDistance},{Alignment}," +
                $"{Margins.Left},{Margins.Right},{Margins.Vertical},{Encoding}";
        }

        public string? AsOverride() => null;

        public Style (Style s)
        {
            Name = s.Name;
            Font = s.Font;
            FontSize = s.FontSize;
            Primary = new Color(s.Primary);
            Secondary = new Color(s.Secondary);
            Outline = new Color(s.Outline);
            Shadow = new Color(s.Shadow);
            Bold = s.Bold;
            Italic = s.Italic;
            Underline = s.Underline;
            Strikeout = s.Strikeout;
            ScaleX = s.ScaleX;
            ScaleY = s.ScaleY;
            Spacing = s.Spacing;
            Angle = s.Angle;
            BorderStyle = s.BorderStyle;
            BorderThickness = s.BorderThickness;
            ShadowDistance = s.ShadowDistance;
            Alignment = s.Alignment;
            Margins = new Margins(s.Margins);
            Encoding = s.Encoding;
        }

        public Style()
        {
            Name = "Default";
            Font = "Arial";
            FontSize = 48.0;
            Primary = new Color(255, 255, 255);
            Secondary = new Color(0, 0, 255); // Red
            Outline = new Color(0, 0, 0);
            Shadow = new Color(0, 0, 0);
            Bold = false;
            Italic = false;
            Underline = false;
            Strikeout = false;
            ScaleX = 100.0;
            ScaleY = 100.0;
            Spacing = 0.0;
            Angle = 0.0;
            BorderStyle = 1;
            BorderThickness = 2.0;
            ShadowDistance = 2.0;
            Alignment = 2;
            Margins = new Margins(0, 0, 0);
            Encoding = 1;
        }
    }

    public class Margins
    {
        public int Left { get; set; }
        public int Right { get; set; }
        public int Vertical { get; set; }

        public Margins(int left, int right, int vertical)
        {
            Left = left;
            Right = right;
            Vertical = vertical;
        }

        public Margins(Margins m)
        {
            Left = m.Left;
            Right = m.Right;
            Vertical = m.Vertical;
        }
    }
}
