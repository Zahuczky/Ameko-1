using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Holo
{
    /// <summary>
    /// Provide a base interface for scripts to use.
    /// </summary>
    public abstract class HoloScript
    {
        /// <summary>
        /// Name of the script
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// A unique name for identifying the script.
        /// Usually takes the form of <c>author.scriptname</c>, but this isn't a requirement.
        /// Script file name should be the qualified name.
        /// </summary>
        public string QualifiedName { get; }
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
        /// Name of the default submenu to put the script under
        /// </summary>
        public string? SubmenuName { get; }
        /// <summary>
        /// Array of qualified names being exported
        /// </summary>
        /// <remarks>
        /// Qualified name should follow the format <c>[scriptqname].func</c>.
        /// Handling of the qname must be provided in <c>Execute(qname)</c>.
        /// </remarks>
        public string[] ExportedMethods { get; }
        
        /// <summary>
        /// Entry point
        /// </summary>
        /// <returns>Result of the script's execution</returns>
        public abstract Task<ExecutionResult> Execute();
        /// <summary>
        /// Entry point for exported functions
        /// </summary>
        /// <remarks>
        /// You <b>MUST</b> override this method if your script uses exported functions!
        /// The default implementation calls <c>Execute()</c> outright.
        /// </remarks>
        /// <param name="qname">Qualified name of the method</param>
        /// <returns>Result of the script's execution</returns>
        public virtual Task<ExecutionResult> Execute(string qname) { return Execute(); }

        public HoloScript() : this("", "", "", "", -1.0, new string[0], null)
        {

        }

        protected HoloScript(
            string name,
            string qualifiedName,
            string description,
            string author,
            double version,
            string[] exportedMethods,
            string? submenuName = null)
        {
            Name = name;
            QualifiedName = qualifiedName;
            Description = description;
            Author = author;
            Version = version;
            ExportedMethods = exportedMethods;
            SubmenuName = submenuName;
        }
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
