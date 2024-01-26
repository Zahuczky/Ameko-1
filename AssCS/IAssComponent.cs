using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
    /// <summary>
    /// Component in an Ass file
    /// </summary>
    public interface IAssComponent
    {
        /// <summary>
        /// Get this component as a file string
        /// </summary>
        /// <returns></returns>
        public string AsAss();
        /// <summary>
        /// Get this component as an override string.
        /// May be null if the component is not applicable for overrides.
        /// </summary>
        /// <returns></returns>
        public string? AsOverride();
    }
}
