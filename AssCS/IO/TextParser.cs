using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AssCS.IO
{
    public class TextParser : IFileParser
    {
        private char comment;
        private char actor;
        public File Load(string filepath)
        {
            File assFile = new File();
            assFile.LoadDefault();

            using var ioFile = System.IO.File.Open(filepath, System.IO.FileMode.Open);
            var reader = new System.IO.StreamReader(ioFile);
            while (reader.ReadLine() is { } line)
            {
                if (line.Equals(string.Empty)) continue;

                var isComment = false;
                var actorStr = string.Empty;

                if (line.StartsWith(comment))
                {
                    line = line.Substring(1).Trim();
                    isComment = true;
                }
                else
                {
                    var actorRegex = $@"^(.*){actor} (.+)";
                    var match = Regex.Match(line, actorRegex);
                    if (match.Success)
                    {
                        actorStr = match.Groups[1].Value;
                        line = match.Groups[2].Value;
                    }
                }

                var e = new Event(assFile.EventManager.NextId)
                {
                    Text = line,
                    Comment = isComment,
                    Actor = actorStr,
                    Start = new Time(),
                    End = new Time()
                };
                assFile.EventManager.AddLast(e);
            }

            reader.Close();
            ioFile.Close();
            return assFile;
        }

        public TextParser(char commentDelim = '#', char actorDelim = ':')
        {
            comment = commentDelim;
            actor = actorDelim;
        }
    }
}
