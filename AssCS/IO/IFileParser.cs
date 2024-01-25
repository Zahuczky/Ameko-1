using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS.IO
{
    /// <summary>
    /// A FileParser supports the reading and parsing of subtitle files.
    /// </summary>
    public interface IFileParser
    {
        /// <summary>
        /// Load a subtitle file from the filesystem.
        /// It is expected that the parsing of any supported format
        /// will result in an Advanced Substation Alpha file.
        /// </summary>
        /// <param name="filepath">Path to the file to open</param>
        /// <returns><cref>File</cref> representing the contents.</returns>
        public File Load(string filepath);
    }
}
