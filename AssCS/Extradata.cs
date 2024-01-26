using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
    /// <summary>
    /// Implementation of Aegisub Extradata with a few changes.
    /// Value is always stored in Base64, denoted with the format
    /// character 'b'.
    /// </summary>
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

        /// <summary>
        /// ID comparison for sorting
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Extradata other) => Id.CompareTo(other.Id);
    }
}
