using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AssCS
{
    /// <summary>
    /// The meat and potatoes of the subtitle file
    /// </summary>
    public class Event : IAssComponent, ICommitable
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

        /// <summary>
        /// Characters per second. It is recommended to keep dialogue events under 18 CPS.
        /// </summary>
        public double Cps => Math.Round(GetStrippedText().Length / (End - Start).TotalSeconds);
        /// <summary>
        /// Maximum line length (in characters).
        /// </summary>
        public int MaxLineWidth => GetStrippedText().Split("\\N").Select(l => l.Length).Max();

        /// <summary>
        /// Bootstrap this event from its representation in a file
        /// </summary>
        /// <param name="data">Line</param>
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
            Margins.Right = Convert.ToInt32(match.Groups[8].Value);
            Margins.Vertical = Convert.ToInt32(match.Groups[9].Value);
            Effect = match.Groups[10].Value;
            Text = match.Groups[11].Value;
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
                var rawId = match.Groups[i].Value[1..]; // =123 → 123
                var id = Convert.ToInt32(rawId);
                LinkedExtradatas.Add(id);
            }

        }

        /// <summary>
        /// Parse this event's override tags
        /// </summary>
        /// <returns>This event, split up into a list of Blocks</returns>
        public List<Block> ParseTags()
        {
            List<Block> blocks = new List<Block>();
            if (Text.Length <= 0)
            {
                blocks.Add(new PlainBlock(""));
                return blocks;
            }

            int drawingLevel = 0;
            string text = string.Copy(Text);
            string work;
            int endPlain;
            for (int len = text.Length, cur = 0; cur < len;)
            {
                // Override block
                if (text[cur] == '{')
                {
                    int end = text.IndexOf('}', cur);
                    if (end == -1)
                    {
                        // ----- Plain -----
                        endPlain = text.IndexOf('{', cur + 1);
                        if (endPlain == -1)
                        {
                            work = text.Substring(cur);
                            cur = len;
                        }
                        else
                        {
                            work = text.Substring(cur, endPlain - cur);
                            cur = endPlain;
                        }
                        if (drawingLevel == 0) blocks.Add(new PlainBlock(work));
                        else blocks.Add(new DrawingBlock(work, drawingLevel));
                        // ----- End Plain -----
                    }
                    else
                    {
                        ++cur;
                        // Get block contents
                        work = text.Substring(cur, end - cur);
                        cur = end + 1;

                        if (work.Length > 0 && work.IndexOf('\\') == -1)
                        {
                            // Comment
                            blocks.Add(new CommentBlock(work));
                        }
                        else
                        {
                            // Create block
                            var block = new OverrideBlock(work);
                            block.ParseTags();
                            // Search for drawings
                            foreach (var tag in block.Tags)
                            {
                                if (tag.Name == "\\p") drawingLevel = tag.Parameters[0].GetInt();
                            }
                            blocks.Add(block);
                        }
                        continue;
                    }
                }
                // ----- Plain 2 electric boogaloo -----
                endPlain = text.IndexOf('{', cur + 1);
                if (endPlain == -1)
                {
                    work = text.Substring(cur);
                    cur = len;
                }
                else
                {
                    work = text.Substring(cur, endPlain - cur);
                    cur = endPlain;
                }
                if (drawingLevel == 0) blocks.Add(new PlainBlock(work));
                else blocks.Add(new DrawingBlock(work, drawingLevel));
                // ----- End Plain -----
            }
            return blocks;
        }

        /// <summary>
        /// Strip away any override tags or comments
        /// </summary>
        public void StripTags()
        {
            Text = GetStrippedText();
        }

        /// <summary>
        /// Get the text without override tags or comments
        /// </summary>
        /// <returns>Stripped text</returns>
        public string GetStrippedText()
        {
            var blocks = ParseTags();
            return string.Join("", blocks.Where(b => b.Type == BlockType.PLAIN).Select(b => b.Text));
        }

        /// <summary>
        /// Check if this event collides with another event.
        /// Events collide if their timestamps overlap.
        /// </summary>
        /// <param name="other">Event to check against</param>
        /// <returns>True if the events collide</returns>
        public bool CollidesWith(Event other)
        {
            if (other == null) return false;
            return (Start < other.Start) ? (other.Start < End) : (Start < other.End);
        }

        /// <summary>
        /// Replace the text in this line.
        /// Operation is skipped if the input is empty.
        /// </summary>
        /// <param name="blocks"></param>
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

        /// <summary>
        /// Clone an event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Compose a new Event
        /// </summary>
        /// <param name="id"></param>
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

        /// <summary>
        /// Load an event from a string
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public Event(int id, string data) : this(id)
        {
            FromAss(data);
        }
    }
}
