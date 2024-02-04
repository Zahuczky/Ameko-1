using System;
using System.Collections.Generic;
using System.Text;

namespace Holo
{
    /// <summary>
    /// Provide a base interface for scripts to use.
    /// </summary>
    public interface IScript
    {
        /// <summary>
        /// Name of the script
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Short description of the functionality provided by the script
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Author(s) of the script
        /// </summary>
        public string Author { get; }
        /// <summary>
        /// Version of the script.
        /// Will be used for updates via Dependency Control
        /// </summary>
        public double Version { get; }
        /// <summary>
        /// Entry point
        /// </summary>
        /// <returns>Result of the script's execution</returns>
        public ExecutionResult Execute();
    }

    /// <summary>
    /// Provides a means for scripts to return a result type and a message
    /// </summary>
    public struct ExecutionResult
    {
        /// <summary>
        /// Status of the script on exit
        /// </summary>
        public ExecutionStatus Status;
        /// <summary>
        /// Optional message to be displayed to the user
        /// </summary>
        public string? Message;
    }

    /// <summary>
    /// Status of the execution on exit
    /// </summary>
    public enum ExecutionStatus
    {
        Success,
        Failure,
        Warning
    }
}
