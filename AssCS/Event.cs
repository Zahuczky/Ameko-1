using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AssCS
{
    public class Event : AssComponent
    {
        public int Id { get; }
        public bool Comment { get; set; }
        public int Layer { get; set; }
        public Time Start { get; set; }
        public Time End { get; set; }
        public string Style { get; set; }
        public string Actor { get; set; }
        public Margins Margins { get; set; }
        public string Effect { get; set; }
        public string Text { get; set; }
        public List<int> LinkedExtradatas { get; set; }

        public void FromAss(string data)
        {
            var eventRegex = @"^(Comment|Dialogue):\ (\d+),(\d+:\d+:\d+.\d+),(\d+:\d+:\d+.\d+),([^,]*),([^,]*),(-?\d+),(-?\d+),(-?\d+),([^,]*),(.*)";
            var match = Regex.Match(data, eventRegex);
            if (!match.Success) throw new ArgumentException($"Event {data} is invalid or malformed.");

            Comment = match.Groups[1].Value == "Comment";
            Layer = Convert.ToInt32(match.Groups[2].Value);
            Start = Time.FromAss(match.Groups[3].Value);
            End = Time.FromAss(match.Groups[4].Value);
            Style = match.Groups[5].Value;
            Actor = match.Groups[6].Value;
            Margins.Left = Convert.ToInt32(match.Groups[7].Value);
            Margins.Right = Convert.ToInt32(match.Groups[9].Value);
            Margins.Vertical = Convert.ToInt32(match.Groups[10].Value);
            Effect = match.Groups[11].Value;
            Text = match.Groups[12].Value;
            LoadExtradata(data);
        }

        private void LoadExtradata(string data)
        {
            if (data.Length < 2) return;
            if (!data.StartsWith("{=")) return;
            var extraRegex = @"^\{(=\d+)+\}";
            var match = Regex.Match(data, extraRegex);
            if (!match.Success) return;

            for (int i = 1; i < match.Groups.Count; i++)
            {
                var rawId = match.Groups[i].Value.Substring(1); // =123 → 123
                var id = Convert.ToInt32(rawId);
                LinkedExtradatas.Add(id);
            }

        }

        public List<Block> ParseTags()
        {
            List<Block> blocks = new List<Block>();
            if (Text.Length <= 0)
            {
                blocks.Add(new PlainBlock(""));
                return blocks;
            }

            int drawingLevel = 0;
            for (int i = 0; i < Text.Length;)
            {
                string work;
                int nextOverride = 0;
                char c = Text[i];
                // Overrides
                if (c == '{')
                {
                    int end = Text.IndexOf('}', i);
                    if (end == -1)
                    {
                        // Entering Plain Text Block [Goto] // TODO: Refactor
                        // Unclosed block → Plain (VSFilter) (Libass does not have this requirement)
                        nextOverride = Text.IndexOf('{', i);
                        if (nextOverride == -1)
                        {
                            work = Text[i..];
                            i = Text.Length - 1;
                        }
                        else
                        {
                            work = Text[i..nextOverride];
                            i = nextOverride;
                        }
                        if (drawingLevel == 0) blocks.Add(new PlainBlock(work));
                        else blocks.Add(new DrawingBlock(work, drawingLevel));
                    }
                    else
                    {
                        // Leaving Plain Text Block
                        i++;
                        // Get block contents
                        work = Text.Substring(i, nextOverride - i);
                        i = nextOverride + 1;

                        if (work.Length > 0 && work.IndexOf('\\') == -1)
                        {

                        } else
                        {
                            // Create a block
                            var block = new OverrideBlock(work);
                            block.ParseTags();

                            // Look for p (drawing) tags
                            foreach (var tag in block.Tags)
                            {
                                if (tag.Name == "\\p") drawingLevel = tag.Parameters[0].GetInt();
                            }
                            blocks.Add(block);
                        }
                        continue;
                    }
                }
                // Entering Plain Text Block [Goto] // TODO: Refactor
                nextOverride = Text.IndexOf('{', i);
                if (nextOverride == -1)
                {
                    work = Text[i..];
                    i = Text.Length - 1;
                }
                else
                {
                    work = Text[i..nextOverride];
                    i = nextOverride;
                }
                if (drawingLevel == 0) blocks.Add(new PlainBlock(work));
                else blocks.Add(new DrawingBlock(work, drawingLevel));
                // Leaving Plain Text Block
            }
            return blocks;
        }

        public void StripTags()
        {
            Text = GetStrippedText();
        }

        public string GetStrippedText()
        {
            var blocks = ParseTags();
            return string.Join("", blocks.Where(b => b.Type == BlockType.PLAIN).Select(b => b.Text));
        }

        public bool CollidesWith(Event other)
        {
            if (other == null) return false;
            return (Start < other.Start) ? (other.Start < End) : (Start < other.End);
        }

        public void UpdateText(List<Block> blocks)
        {
            if (blocks.Count == 0) return;
            Text = string.Join("", blocks.Select(b => b.Text));
        }

        public string AsAss()
        {
            string extradatas = LinkedExtradatas.Count > 0 ? $"{{{string.Join("=", LinkedExtradatas)}}}" : "";

            return $"{(Comment ? "Comment" : "Dialogue")}: {Layer},{Start.AsAss()},{End.AsAss()},{Style},{Actor}," +
                $"{Margins.Left},{Margins.Right},{Margins.Vertical},{Effect},{extradatas}" +
                $"{Text.Replace("\n", "").Replace("\r", "")}";
        }

        public string? AsOverride() => null;

        public Event(int id, Event e)
        {
            Id = id;
            Comment = e.Comment;
            Layer = e.Layer;
            Start = new Time(e.Start);
            End = new Time(e.End);
            Style = e.Style;
            Actor = e.Actor;
            Margins = new Margins(e.Margins);
            Effect = e.Effect;
            Text = e.Text;
            LinkedExtradatas = new List<int>(e.LinkedExtradatas);
        }

        public Event(int id)
        {
            Id = id;
            Comment = false;
            Layer = 0;
            Start = Time.FromSeconds(0);
            End = Time.FromSeconds(5);
            Style = "Default";
            Actor = "";
            Margins = new Margins(0, 0, 0);
            Effect = "";
            Text = "";
            LinkedExtradatas = new List<int>();
        }
    }
}
