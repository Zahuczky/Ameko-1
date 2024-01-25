using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
    public class OverridePrototype
    {
        public string Name { get; }
        public List<OverridePrototypeParam> Parameters { get; }

        public void AddParam(VariableType type, ParamType classification = ParamType.NORMAL, Optional opt = Optional.NOT_OPTIONAL)
        {
            Parameters.Add(new OverridePrototypeParam(type, classification, opt));
        }

        public OverridePrototype(string name, List<OverridePrototypeParam> paramList)
        {
            Name = name;
            Parameters = paramList;
        }

        public OverridePrototype(string name, VariableType type, ParamType classification = ParamType.NORMAL, Optional opt = Optional.NOT_OPTIONAL)
        {
            Name = name;
            Parameters = new List<OverridePrototypeParam>();
            AddParam(type, classification, opt);
        }

        public static readonly Dictionary<string, OverridePrototype> Prototypes = new Dictionary<string, OverridePrototype>();
        public static void LoadPrototypes()
        {
            if (Prototypes.Count > 0) return;

            Prototypes.Add("\\alpha", new OverridePrototype("\\alpha", VariableType.TEXT, ParamType.ALPHA));
            Prototypes.Add("\\bord", new OverridePrototype("\\bord", VariableType.FLOAT, ParamType.ABSOLUTE_SIZE));
            Prototypes.Add("\\xbord", new OverridePrototype("\\xbord", VariableType.FLOAT, ParamType.ABSOLUTE_SIZE));
            Prototypes.Add("\\ybord", new OverridePrototype("\\ybord", VariableType.FLOAT, ParamType.ABSOLUTE_SIZE));
            Prototypes.Add("\\shad", new OverridePrototype("\\shad", VariableType.FLOAT, ParamType.ABSOLUTE_SIZE));
            Prototypes.Add("\\xshad", new OverridePrototype("\\xshad", VariableType.FLOAT, ParamType.ABSOLUTE_SIZE));
            Prototypes.Add("\\yshad", new OverridePrototype("\\yshad", VariableType.FLOAT, ParamType.ABSOLUTE_SIZE));
            Prototypes.Add("\\fade", new OverridePrototype("\\fade", new List<OverridePrototypeParam>
            {
                new OverridePrototypeParam(VariableType.INT),
                new OverridePrototypeParam(VariableType.INT),
                new OverridePrototypeParam(VariableType.INT),
                new OverridePrototypeParam(VariableType.INT, ParamType.RELATIVE_TIME_START),
                new OverridePrototypeParam(VariableType.INT, ParamType.RELATIVE_TIME_START),
                new OverridePrototypeParam(VariableType.INT, ParamType.RELATIVE_TIME_START),
                new OverridePrototypeParam(VariableType.INT, ParamType.RELATIVE_TIME_START)
            }));
            Prototypes.Add("\\move", new OverridePrototype("\\move", new List<OverridePrototypeParam>
            {
                new OverridePrototypeParam(VariableType.FLOAT, ParamType.ABSOLUTE_POS_X),
                new OverridePrototypeParam(VariableType.FLOAT, ParamType.ABSOLUTE_POS_Y),
                new OverridePrototypeParam(VariableType.FLOAT, ParamType.ABSOLUTE_POS_X),
                new OverridePrototypeParam(VariableType.FLOAT, ParamType.ABSOLUTE_POS_Y),
                new OverridePrototypeParam(VariableType.INT, ParamType.RELATIVE_TIME_START),
                new OverridePrototypeParam(VariableType.INT, ParamType.RELATIVE_TIME_START)
            }));
            Prototypes.Add("\\clip4", new OverridePrototype("\\clip", new List<OverridePrototypeParam>
            {
                new OverridePrototypeParam(VariableType.INT, ParamType.ABSOLUTE_POS_X),
                new OverridePrototypeParam(VariableType.INT, ParamType.ABSOLUTE_POS_Y),
                new OverridePrototypeParam(VariableType.INT, ParamType.ABSOLUTE_POS_X),
                new OverridePrototypeParam(VariableType.INT, ParamType.ABSOLUTE_POS_Y)
            }));
            Prototypes.Add("\\clip2", new OverridePrototype("\\clip", new List<OverridePrototypeParam>
            {
                new OverridePrototypeParam(VariableType.INT, ParamType.NORMAL, Optional.OPTIONAL_2),
                new OverridePrototypeParam(VariableType.TEXT, ParamType.DRAWING)
            }));
            Prototypes.Add("\\iclip4", new OverridePrototype("\\iclip", new List<OverridePrototypeParam>
            {
                new OverridePrototypeParam(VariableType.INT, ParamType.ABSOLUTE_POS_X),
                new OverridePrototypeParam(VariableType.INT, ParamType.ABSOLUTE_POS_Y),
                new OverridePrototypeParam(VariableType.INT, ParamType.ABSOLUTE_POS_X),
                new OverridePrototypeParam(VariableType.INT, ParamType.ABSOLUTE_POS_Y)
            }));
            Prototypes.Add("\\iclip2", new OverridePrototype("\\iclip", new List<OverridePrototypeParam>
            {
                new OverridePrototypeParam(VariableType.INT, ParamType.NORMAL, Optional.OPTIONAL_2),
                new OverridePrototypeParam(VariableType.TEXT, ParamType.DRAWING)
            }));
            Prototypes.Add("\\fscx", new OverridePrototype("\\fscx", VariableType.FLOAT, ParamType.RELATIVE_SIZE_X));
            Prototypes.Add("\\fscy", new OverridePrototype("\\fscy", VariableType.FLOAT, ParamType.RELATIVE_SIZE_Y));
            Prototypes.Add("\\pos", new OverridePrototype("\\pos", new List<OverridePrototypeParam>
            {
                new OverridePrototypeParam(VariableType.FLOAT, ParamType.ABSOLUTE_POS_X),
                new OverridePrototypeParam(VariableType.FLOAT, ParamType.ABSOLUTE_POS_Y),
            }));
            Prototypes.Add("\\org", new OverridePrototype("\\org", new List<OverridePrototypeParam>
            {
                new OverridePrototypeParam(VariableType.INT, ParamType.ABSOLUTE_POS_X),
                new OverridePrototypeParam(VariableType.INT, ParamType.ABSOLUTE_POS_Y),
            }));
            Prototypes.Add("\\pbo", new OverridePrototype("\\pbo", VariableType.INT, ParamType.ABSOLUTE_POS_Y));
            Prototypes.Add("\\fad", new OverridePrototype("\\fad", new List<OverridePrototypeParam>
            {
                new OverridePrototypeParam(VariableType.INT, ParamType.RELATIVE_TIME_START),
                new OverridePrototypeParam(VariableType.INT, ParamType.RELATIVE_TIME_END),
            }));
            Prototypes.Add("\\fsp", new OverridePrototype("\\fsp", VariableType.FLOAT, ParamType.ABSOLUTE_SIZE));
            Prototypes.Add("\\frx",new OverridePrototype("\\frx", VariableType.FLOAT));
            Prototypes.Add("\\fry", new OverridePrototype("\\fry", VariableType.FLOAT));
            Prototypes.Add("\\frz", new OverridePrototype("\\frz", VariableType.FLOAT));
            Prototypes.Add("\\fr", new OverridePrototype("\\fr", VariableType.FLOAT));
            Prototypes.Add("\\fax", new OverridePrototype("\\fax", VariableType.FLOAT));
            Prototypes.Add("\\fay", new OverridePrototype("\\fay", VariableType.FLOAT));
            Prototypes.Add("\\1c", new OverridePrototype("\\1c", VariableType.TEXT, ParamType.COLOR));
            Prototypes.Add("\\2c", new OverridePrototype("\\2c", VariableType.TEXT, ParamType.COLOR));
            Prototypes.Add("\\3c", new OverridePrototype("\\3c", VariableType.TEXT, ParamType.COLOR));
            Prototypes.Add("\\4c", new OverridePrototype("\\4c", VariableType.TEXT, ParamType.COLOR));
            Prototypes.Add("\\1a", new OverridePrototype("\\1a", VariableType.TEXT, ParamType.ALPHA));
            Prototypes.Add("\\2a", new OverridePrototype("\\2a", VariableType.TEXT, ParamType.ALPHA));
            Prototypes.Add("\\3a", new OverridePrototype("\\3a", VariableType.TEXT, ParamType.ALPHA));
            Prototypes.Add("\\4a", new OverridePrototype("\\4a", VariableType.TEXT, ParamType.ALPHA));
            Prototypes.Add("\\fe", new OverridePrototype("\\fe", VariableType.TEXT));
            Prototypes.Add("\\ko", new OverridePrototype("\\ko", VariableType.INT, ParamType.KARAOKE));
            Prototypes.Add("\\kf", new OverridePrototype("\\kf", VariableType.INT, ParamType.KARAOKE));
            Prototypes.Add("\\be", new OverridePrototype("\\be", VariableType.INT, ParamType.ABSOLUTE_SIZE));
            Prototypes.Add("\\blur", new OverridePrototype("\\blur", VariableType.FLOAT, ParamType.ABSOLUTE_SIZE));
            Prototypes.Add("\\fn", new OverridePrototype("\\fn", VariableType.TEXT));
            Prototypes.Add("\\fs+", new OverridePrototype("\\fs+", VariableType.FLOAT));
            Prototypes.Add("\\fs-", new OverridePrototype("\\fs-", VariableType.FLOAT));
            Prototypes.Add("\\fs", new OverridePrototype("\\fs", VariableType.FLOAT, ParamType.ABSOLUTE_SIZE));
            Prototypes.Add("\\an", new OverridePrototype("\\an", VariableType.INT));
            Prototypes.Add("\\c", new OverridePrototype("\\c", VariableType.TEXT, ParamType.COLOR));
            Prototypes.Add("\\b", new OverridePrototype("\\b", VariableType.INT));
            Prototypes.Add("\\i", new OverridePrototype("\\i", VariableType.BOOL));
            Prototypes.Add("\\u", new OverridePrototype("\\u", VariableType.BOOL));
            Prototypes.Add("\\s", new OverridePrototype("\\s", VariableType.BOOL));
            Prototypes.Add("\\a", new OverridePrototype("\\a", VariableType.INT));
            Prototypes.Add("\\k", new OverridePrototype("\\k", VariableType.INT, ParamType.KARAOKE));
            Prototypes.Add("\\K", new OverridePrototype("\\K", VariableType.INT, ParamType.KARAOKE));
            Prototypes.Add("\\q", new OverridePrototype("\\q", VariableType.INT));
            Prototypes.Add("\\p", new OverridePrototype("\\p", VariableType.INT));
            Prototypes.Add("\\r", new OverridePrototype("\\r", VariableType.INT));
            Prototypes.Add("\\t", new OverridePrototype("\\t", new List<OverridePrototypeParam>
            {
                new OverridePrototypeParam(VariableType.INT, ParamType.RELATIVE_TIME_START, Optional.OPTIONAL_3 | Optional.OPTIONAL_4),
                new OverridePrototypeParam(VariableType.INT, ParamType.RELATIVE_TIME_START, Optional.OPTIONAL_3 | Optional.OPTIONAL_4),
                new OverridePrototypeParam(VariableType.FLOAT, ParamType.NORMAL, Optional.OPTIONAL_2 | Optional.OPTIONAL_4),
                new OverridePrototypeParam(VariableType.BLOCK)
            }));
        }
    }

    public class OverridePrototypeParam
    {
        public Optional Optional { get; }
        public VariableType Type { get; }
        public ParamType Classification { get; }

        public OverridePrototypeParam(VariableType type, ParamType classification = ParamType.NORMAL, Optional optional = Optional.NOT_OPTIONAL)
        {
            Type = type;
            Classification = classification;
            Optional = optional;
        }
    }

    [Flags]
    public enum Optional : uint
    {
        NOT_OPTIONAL = 0xFF,
        OPTIONAL_1 = 0x01,
        OPTIONAL_2 = 0x02,
        OPTIONAL_3 = 0x04,
        OPTIONAL_4 = 0x08,
        OPTIONAL_5 = 0x10,
        OPTIONAL_6 = 0x20,
        OPTIONAL_7 = 0x40
    }
}
