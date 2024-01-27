using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssCS
{
    /// <summary>
    /// Representation of a single segment of an Event.
    /// </summary>
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

    /// <summary>
    /// Any text in an event that is not a comment, override, or drawing
    /// should be considered to be in a Plaintext Block.
    /// </summary>
    public class PlainBlock : Block
    {
        public PlainBlock(string data) :
            base(
                data,
                BlockType.PLAIN
                )
        { }
    }

    /// <summary>
    /// Text within a set of override brackets <code>{ }</code>
    /// that does not contain an override are Comments.
    /// </summary>
    public class CommentBlock : Block
    {
        public CommentBlock(string data) :
            base(
                $"{{{data}}}",
                BlockType.COMMENT
                )
        { }
    }

    /// <summary>
    /// Text following any drawing level greater than 0 is assumed
    /// to be a Drawing. A drawing is initiated by a preceeding Override
    /// Block containg the drawing override P tag:
    /// <code>{\p[drawinglevel]}</code>
    /// </summary>
    public class DrawingBlock : Block
    {
        public int Scale { get; set; }
        public DrawingBlock(string data, int scale) :
            base(
                data,
                BlockType.DRAWING
                )
        { 
            Scale = scale;
        }
    }

    /// <summary>
    /// An Override block is composed of a set of override brackets
    /// containing one or more Override Tags
    /// </summary>
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
            for (int i = 1; i < Text.Length; ++i)
            {
                if (depth > 0)
                {
                    if (Text[i] == ')') --depth;

                }
                else if (Text[i] == '\\')
                {
                    Tags.Add(new OverrideTag(Text.Substring(start, i - start)));
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

    /// <summary>
    /// Type of Block
    /// </summary>
    public enum BlockType
    {
        PLAIN,
        COMMENT,
        OVERRIDE,
        DRAWING
    }
}
