using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AssCS.IO
{
    /// <summary>
    /// Concrete implementation of a <cref>IFileParser</cref> for .ASS files.
    /// This implementation targets ASS V4+ files using Aegisub headers.
    /// </summary>
    public class AssParser : IFileParser
    {
        public File Load(string filepath)
        {
            ParseFunc parseState = ParseUnknown;
            File assFile = new File();

            using var ioFile = System.IO.File.Open(filepath, System.IO.FileMode.Open);
            var reader = new System.IO.StreamReader(ioFile);
            while (reader.ReadLine() is { } line)
            {
                if (line.Equals(string.Empty)) continue;

                var headerRegex = @"^\[(.+)\]";
                var match = Regex.Match(line, headerRegex);
                if (match.Success)
                {
                    var lower = match.Groups[1].Value.ToLower();
                    parseState = lower switch
                    {
                        "v4+ styles" => ParseStyle,
                        "events" => ParseEvent,
                        // TODO
                        _ => ParseUnknown
                    };
                    continue; // Skip further processing of this line
                }
                parseState(line, assFile);
            }

            return assFile;
        }

        private delegate void ParseFunc(string line, File file);

        private void ParseStyle(string line, File file)
        {
            if (!line.StartsWith("Style:")) return;
            file.StyleManager.Set(
                new Style(file.StyleManager.NextId, line)
            );
        }

        private void ParseEvent(string line, File file)
        {
            if (!line.StartsWith("Dialogue:") && !line.StartsWith("Comment:")) return;
            file.EventManager.AddLast(
                new Event(file.EventManager.NextId, line)
            );
        }

        private void ParseScriptInfo(string line, File file)
        {
            // TODO
        }

        private void ParseMetadata(string line, File file)
        {
            // TODO
        }

        private void ParseExtradata(string line, File file)
        {
            // TODO
        }

        private void ParseUnknown(string line, File file)
        {
            // Do nothing (for now)
            return;
        }
    }
}
