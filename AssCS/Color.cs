using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AssCS
{
    public class Color : AssComponent
    {
        public uint Red { get; set; }
        public uint Green { get; set; }
        public uint Blue { get; set; }
        public uint Alpha { get; set; }

        public double Luminance => (((0.2126 * Red) + (0.7152 * Green) + (0.0722 * Blue)) / 255) * Alpha;

        public string AsAss()
        {
            return $"&H{Alpha:X2}{Blue:X2}{Green:X2}{Red:X2}&";
        }
        public string AsOverride()
        {
            return $"&H{Blue:X2}{Green:X2}{Red:X2}&";
        }

        public Color(string data)
        {
            var rgbRegex = @"&H([\da-fA-F]{2}){3}&?";
            var rgbaRegex = @"&H([\da-fA-F]{2}){4}&?";
            var rgbMatch = Regex.Match(data, rgbRegex);
            var rgbaMatch = Regex.Match(data, rgbaRegex);
            if (!rgbMatch.Success && !rgbaMatch.Success) throw new ArgumentException($"Color {data} is invalid or malformed.");

            if (rgbaMatch.Success)
            {
                Alpha = Convert.ToUInt32(rgbaMatch.Groups[1].Value, 16);
                Blue = Convert.ToUInt32(rgbaMatch.Groups[2].Value, 16);
                Green = Convert.ToUInt32(rgbaMatch.Groups[3].Value, 16);
                Red = Convert.ToUInt32(rgbaMatch.Groups[4].Value, 16);
            } else
            {
                Alpha = 255;
                Blue = Convert.ToUInt32(rgbMatch.Groups[1].Value, 16);
                Green = Convert.ToUInt32(rgbMatch.Groups[2].Value, 16);
                Red = Convert.ToUInt32(rgbMatch.Groups[3].Value, 16);
            }
        }

        public Color(Color c)
        {
            Red = c.Red;
            Green = c.Green;
            Blue = c.Blue;
            Alpha = c.Alpha;
        }

        public Color(uint red, uint green, uint blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = 255;
        }

        public Color(uint red, uint green, uint blue, uint alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public Color()
        {
            Red = 0;
            Green = 0;
            Blue = 0;
            Alpha = 255;
        }

        public static Color operator +(Color a, Color b)
        {
            return new Color(
                Math.Clamp(a.Alpha + b.Alpha, 0, 255),
                Math.Clamp(a.Red + b.Red, 0, 255),
                Math.Clamp(a.Green + b.Green, 0, 255),
                Math.Clamp(a.Blue + b.Blue, 0, 255)
            );
        }

        public static Color operator -(Color a, Color b)
        {
            return new Color(
                Math.Clamp(a.Alpha - b.Alpha, 0, 255),
                Math.Clamp(a.Red - b.Red, 0, 255),
                Math.Clamp(a.Green - b.Green, 0, 255),
                Math.Clamp(a.Blue - b.Blue, 0, 255)
            );
        }

        public static double Contrast(Color a, Color b)
        {
            return (a.Luminance + 0.05) / (b.Luminance + 0.05);
        }
    }
}
