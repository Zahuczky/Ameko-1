using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AssCS
{
    /// <summary>
    /// Represents a style that can be applied to an event
    /// </summary>
    public class Style : IAssComponent, ICommitable
    {
        public int Id { get; }
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

        /// <summary>
        /// Bootstrap this style from its representation in a file
        /// </summary>
        /// <param name="data">Line</param>
        public void FromAss(string data)
        {
            var styleRegex = @"Style:\ ([^,]*),([^,]*),([\d.]+),(&H[\da-fA-F]{8}&?),(&H[\da-fA-F]{8}&?),(&H[\da-fA-F]{8}&?),(&H[\da-fA-F]{8}&?),(-?[\d.]+),(-?[\d.]+),(-?[\d.]+),(-?[\d.]+),(-?[\d.]+),(-?[\d.]+),(-?[\d.]+),(-?[\d.]+),(-?[\d.]+),(-?[\d.]+),(-?[\d.]+),(-?[\d.]+),(-?[\d.]+),(-?[\d.]+),(-?[\d.]+),(-?[\d.]+)";
            var match = Regex.Match(data, styleRegex);
            if (!match.Success) throw new ArgumentException($"Style {data} is invalid or malformed.");

            Name = match.Groups[1].Value;
            Font = match.Groups[2].Value;
            FontSize = Convert.ToDouble(match.Groups[3].Value);
            Primary = new Color(match.Groups[4].Value);
            Secondary = new Color(match.Groups[5].Value);
            Outline = new Color(match.Groups[6].Value);
            Shadow = new Color(match.Groups[7].Value);
            Bold = Convert.ToInt32(match.Groups[8].Value) != 0;
            Italic = Convert.ToInt32(match.Groups[9].Value) != 0;
            Underline = Convert.ToInt32(match.Groups[10].Value) != 0;
            Strikeout = Convert.ToInt32(match.Groups[11].Value) != 0;
            ScaleX = Convert.ToDouble(match.Groups[12].Value);
            ScaleY = Convert.ToDouble(match.Groups[13].Value);
            Spacing = Convert.ToDouble(match.Groups[14].Value);
            Angle = Convert.ToDouble(match.Groups[15].Value);
            BorderStyle = Convert.ToInt32(match.Groups[16].Value);
            BorderThickness = Convert.ToDouble(match.Groups[17].Value);
            ShadowDistance = Convert.ToDouble(match.Groups[18].Value);
            Alignment = Convert.ToInt32(match.Groups[19].Value);
            Margins = new Margins(
                    Convert.ToInt32(match.Groups[20].Value),
                    Convert.ToInt32(match.Groups[21].Value),
                    Convert.ToInt32(match.Groups[22].Value)
                );
            Encoding = Convert.ToInt32(match.Groups[23].Value);
        }

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

        public Style (int id, Style s)
        {
            Id = id;
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

        public Style(int id)
        {
            Id = id;
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

        public Style(int id, string data) : this(id)
        {
            FromAss(data);
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

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!obj.GetType().Equals(typeof(Margins))) return false;
            Margins o = (Margins)obj;
            return Left == o.Left
                && Right == o.Right
                && Vertical == o.Vertical;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Left, Right, Vertical);
        }
    }
}
