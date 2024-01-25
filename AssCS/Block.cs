using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssCS
{
    public abstract class Block : IAssComponent
    {
        public string Text { get; }
        public BlockType Type { get; }
        public Block(string data)
        {
            Text = data;
        }

        protected Block(string data, BlockType type)
        {
            Text = data;
            Type = type;
        }

        public string AsAss() => Text;
        public string? AsOverride() => Text;
    }

    public class PlainBlock : Block
    {
        public PlainBlock(string data) :
            base(
                data,
                BlockType.PLAIN
                )
        { }
    }

    public class CommentBlock : Block
    {
        public CommentBlock(string data) :
            base(
                $"{{{data}}}",
                BlockType.COMMENT
                )
        { }
    }

    public class DrawingBlock : Block
    {
        public int Scale { get; set; }
        public DrawingBlock(string data, int scale) :
            base(
                data,
                BlockType.DRAWING
                )
        { }
    }

    public class OverrideBlock : Block
    {
        public List<OverrideTag> Tags { get; }

        public OverrideBlock(string data) : base(data, BlockType.OVERRIDE)
        {
            Tags = new List<OverrideTag>();
        }

        public void ParseTags()
        {
            Tags.Clear();
            var depth = 0;
            var start = 0;
            for (int i = 0; i < Text.Length; ++i)
            {
                if (depth > 0 && Text[i] == ')') --depth;
                else if (Text[i] == '\\')
                {
                    Tags.Add(new OverrideTag(Text[start..i]));
                    start = i;
                }
                else if (Text[i] == '(') ++depth;
            }
            if (Text.Length > 0) Tags.Add(new OverrideTag(Text[start..]));
        }

        public void AddTag(string tag)
        {
            Tags.Add(new OverrideTag(tag));
        }

        public string GetText()
        {
            return $"{{{string.Join("", Tags.Select(t => t.ToString()))}}}";
        }
    }

    public enum BlockType
    {
        PLAIN,
        COMMENT,
        OVERRIDE,
        DRAWING
    }
}
