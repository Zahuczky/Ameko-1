using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
    public class Extradata : IComparable<Extradata>
    {
        public int Id { get; }
        public int Expiration { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public Extradata(int id, int exp, string key, string value) 
        {
            Id = id;
            Expiration = exp;
            Key = key;
            Value = value;
        }

        public int CompareTo(Extradata other) => Id.CompareTo(other.Id);
    }
}
