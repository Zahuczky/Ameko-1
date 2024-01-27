using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssCS
{
    public class OverrideTag
    {
        private bool _valid = false;
        public List<OverrideParameter> Parameters { get; }
        public string Name { get; set; }
        public bool Valid => _valid;

        public void Clear()
        {
            Parameters.Clear();
            _valid = false;
        }

        public void SetText(string data)
        {
            OverridePrototype.LoadPrototypes();

            foreach (OverridePrototype p in OverridePrototype.Prototypes.Values)
            {
                if (data.StartsWith(p.Name))
                {
                    _name = p.Name;
                    ParseParameters(data[p.Name.Length..]);
                    _valid = true;
                    return;
                }
            }
            // Garbage :poppo:
            _name = data;
            _valid = false;
        }

        public OverrideTag()
        {
            _name = "";
            Parameters = new List<OverrideParameter>();
        }

        public OverrideTag(string data)
        {
            _name = "";
            Parameters = new List<OverrideParameter>();
            SetText(data);
        }

        public OverrideTag(OverrideTag tag)
        {
            _name = tag.Name;
            Parameters = new List<OverrideParameter>(tag.Parameters);
            _valid = tag.Valid;
        }

        public List<string> Tokenize(string data)
        {
            List<string> paramList = new List<string>();
            if (data.Length <= 0) return paramList;

            if (data[0] != '(')
            {
                // No parentheses → single-parameter override
                paramList.Add(data.Trim());
                return paramList;
            }

            // Parsing time uwu
            var i = 0;
            var parDepth = 1;
            while (i < data.Length && parDepth > 0)
            {
                // Hunt for next comma or parenthesis.
                var start = ++i;
                while (i < data.Length && parDepth > 0)
                {
                    char c = data[i];
                    if (c == ',' && parDepth == 1) break;
                    if (c == '(') parDepth++;
                    else if (c == ')')
                    {
                        if (--parDepth == 0)
                        {
                            break;
                        }
                    }
                    i++;
                }
                paramList.Add(data[start..i].Trim());
            }

            if (i + 1 < data.Length)
            {
                paramList.Add(data[(i + 1)..]);
            }
            return paramList;
        }

        public void ParseParameters(string data)
        {
            Clear();
            List<string> paramList = Tokenize(data);
            int parsFlag = 1 << (paramList.Count - 1); // Optional parameters flag

            OverridePrototype p;
            if (Name == "\\clip" && paramList.Count != 4) p = OverridePrototype.Prototypes["\\clip2"];
            else if (Name == "\\clip") p = OverridePrototype.Prototypes["\\clip4"];
            else if (Name == "\\iclip" && paramList.Count != 4) p = OverridePrototype.Prototypes["\\iclip2"];
            else if (Name == "\\iclip") p = OverridePrototype.Prototypes["\\iclip4"];
            else p = OverridePrototype.Prototypes[Name];

            int curPar = 0;
            foreach (var param in p.Parameters)
            {
                Parameters.Add(new OverrideParameter(param.Type, param.Classification));
                if (((uint)param.Optional & parsFlag) == 0 || curPar >= paramList.Count)
                    continue;

                Parameters.Last().Set(paramList[curPar++]);
            }
        }

        public override string ToString()
        {
            string result = Name;
            bool parentheses = Parameters.Count > 1;
            if (parentheses) result += "(";
            result += string.Join(",", Parameters.Where(p => !p.Omitted).Select(p => p.GetString()));
            if (parentheses ) result += ")";
            return result;
        }
    }

    public class OverrideParameter
    {

        private OverrideBlock? block;
        public VariableType Type { get; }
        public ParamType Classification { get; }
        private bool _omitted = true;
        public bool Omitted => _omitted;
        private string value = "";

        public OverrideParameter(VariableType type, ParamType classification)
        {
            Type = type;
            Classification = classification;
        }

        public string GetString()
        {
            if (Omitted) { throw new InvalidOperationException("OverrideParameter: Get() called on omitted parameter!"); }
            if (block != null)
            {
                var blockText = block.Text;
                if (blockText.StartsWith('{')) blockText = blockText[1..];
                if (blockText.EndsWith('}')) blockText = blockText[..^1];
                return blockText;
            }
            return value ?? "";
        }

        public int GetInt()
        {
            if (Classification == ParamType.ALPHA)
            {
                var strippedHex = new string(value.Where(Uri.IsHexDigit).ToArray());
                return Math.Clamp(Convert.ToInt32(strippedHex, 16), 0, 255);
            }
            return Convert.ToInt32(GetString());
        }

        public double GetDouble()
        {
            return Convert.ToDouble(value);
        }

        public float GetFloat()
        {
            return Convert.ToSingle(value);
        }

        public bool GetBool()
        {
            return GetInt() != 0;
        }

        public Color GetColor()
        {
            return new Color(GetString());
        }

        public OverrideBlock GetOverrideBlock()
        {
            if (block == null)
            {
                block = new OverrideBlock(GetString());
                block.ParseTags();
            }
            return block;
        }

        public void Set(string newValue)
        {
            _omitted = false;
            value = newValue;
            block = null;
        }

        public void Set(int newValue)
        {
            if (Classification == ParamType.ALPHA)
                Set($"&H{Math.Clamp(newValue, 0, 255):X2}&");
            else
                Set(Convert.ToString(newValue));
        }

        public void Set(double newValue)
        {
            Set(Convert.ToString(newValue));
        }

        public void Set(bool newValue)
        {
            Set(newValue ? 1 : 0);
        }
    }

    public enum ParamType
    {
        NORMAL,
        ABSOLUTE_SIZE,
        ABSOLUTE_POS_X,
        ABSOLUTE_POS_Y,
        RELATIVE_SIZE_X,
        RELATIVE_SIZE_Y,
        RELATIVE_TIME_START,
        RELATIVE_TIME_END,
        KARAOKE,
        DRAWING,
        ALPHA,
        COLOR
    }

    public enum VariableType
    {
        INT,
        FLOAT,
        TEXT,
        BOOL,
        BLOCK
    }
}
