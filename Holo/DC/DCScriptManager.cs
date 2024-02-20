using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Holo.DC
{
    /// <summary>
    /// Manages scripts on the filesystem for Dependency Control
    /// </summary>
    public class DCScriptManager
    {
        private static readonly string scriptRoot = Path.Combine(HoloContext.HoloDirectory, "scripts");

        /// <summary>
        /// Install a script
        /// </summary>
        /// <param name="qualifiedName">Name of the script</param>
        /// <param name="updateUrl">Location of the script file on the Internet</param>
        /// <returns>True if installation was successful</returns>
        public static bool InstallDCScript(string qualifiedName, string updateUrl)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    using (var stream = client.GetStreamAsync(updateUrl))
                    { 
                        using (var fs = new FileStream(ScriptPath(qualifiedName), FileMode.OpenOrCreate))
                        {
                            stream.Result.CopyTo(fs);
                        }
                    }
                    return true;
                }
                catch
                {
                    Console.Error.WriteLine($"An error occured while installing {qualifiedName}.");
                    return false;
                }
            }
        }

        /// <summary>
        /// Uninstall a script
        /// </summary>
        /// <param name="qualifiedName">Name of the script</param>
        /// <returns>True if the script was removed successfully</returns>
        public static bool UninstallDCScript(string qualifiedName)
        {
            if (IsDCScriptInstalled(qualifiedName))
                try
                {
                    File.Delete(ScriptPath(qualifiedName));
                    return true;
                }
                catch
                {
                    Console.Error.WriteLine($"An error occured while deleting {qualifiedName}.");
                    return false;
                }
            return false;
        }

        /// <summary>
        /// Update a script
        /// </summary>
        /// <param name="qualifiedName">Name of the script</param>
        /// <param name="updateUrl">Location of the script file on the Internet</param>
        /// <returns></returns>
        public static bool UpdateDCScript(string qualifiedName, string updateUrl)
        {
            if (UninstallDCScript(qualifiedName))
                return InstallDCScript(qualifiedName, updateUrl);
            return false;
        }

        /// <summary>
        /// Check if a script is installed
        /// </summary>
        /// <param name="qualifiedName">Name of the script</param>
        /// <returns>True if the script is installed</returns>
        public static bool IsDCScriptInstalled(string qualifiedName)
        {
            return File.Exists(ScriptPath(qualifiedName));
        }

        /// <summary>
        /// Get the filepath corresponding to the script
        /// </summary>
        /// <param name="qualifiedName">Name of the script</param>
        /// <returns>The filepath, ending in .cs</returns>
        private static string ScriptPath(string qualifiedName)
        {
            return Path.Combine(scriptRoot, $"{qualifiedName}.cs");
        }

        private DCScriptManager()
        {
        }
    }
}
