using CliWrap;
using CliWrap.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace cc65Wrapper
{
    /// <summary>
    /// Wrapper class for CC65. Allows building of defined project
    /// </summary>
    public class Cc65Build
    {

        #region Constants

        /// <summary>
        /// Define more readable placeholders for cl65 cmd line options
        /// </summary>
        const string CL65 = "cl65";
        const string CC65_TARGET = "CC65_TARGET";
        const string TARGET_OPTION = "-t";
        const string OUTPUT_FILE_OPTION = "-o";
        const string OPTIMISE_OPTION = "-O";

        #endregion

        #region Public methods

        /// <summary>
        /// Compiles source file associated with project file into output file using 'cl65'
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static async Task<ExecutionResult> Compile(Cc65Project project)
        {
            CliWrap.Models.ExecutionResult result;

            // Take a copy of the current working directory ...
            var originalDir = Directory.GetCurrentDirectory();

            try
            {
                // Switch to projects working directory ...
                Directory.SetCurrentDirectory(project.WorkingDirectory);

                // Build an arguments list from the project settings to pass to CL65 ...
                var argumentList = BuildArgumentsList(project);

                // Call CL65 with project settings ...
                result = await Cli.Wrap(CL65)
                .SetEnvironmentVariable(CC65_TARGET, project.TargetPlatform)
                .SetArguments(argumentList)
                .EnableExitCodeValidation(false)
                .ExecuteAsync();
            }
            finally
            {
                // Always restore the original working directory ...
                Directory.SetCurrentDirectory(originalDir);
            }

            return result;
        }

        #endregion

        /// <summary>
        /// Converts the ExecutionResult.StandardErrors string into a List of strings
        /// 
        /// N.B. We also de-duplicate the errors
        /// 
        /// </summary>
        /// <returns>A List of strings representing the individual errors</returns>
        public static List<string> ErrorsAsList(ExecutionResult executionResult)
        {
            var splitErrors = executionResult.StandardError.Split(
                new string[] { "\r\n", "\r", "\n" },
                System.StringSplitOptions.RemoveEmptyEntries);

            var errorsList = new List<string>(splitErrors);
            var dedupedList = errorsList.Distinct().ToList();

            return dedupedList; 
        }

        #region Private Methods

        /// <summary>
        /// Builds from supplied project file a list of string arguments to pass to 'cl65'
        /// </summary>
        /// <param name="project"></param>
        /// <returns>A List of strings representing the CL65 arguments</returns>
        private static List<string> BuildArgumentsList(Cc65Project project)
        {
            // Add the target platform ...
            var result = new List<string>
            {
                // Add target args ...
                TARGET_OPTION,
                project.TargetPlatform
            };

            // Add input files ...
            foreach (var inputFile in project.InputFiles)
            {
                result.Add(inputFile);
            }

            // Add optimise arg, if needed ...
            if (project.OptimiseCode)
            {
                result.Add(OPTIMISE_OPTION);
            }

            // Add output file args ...
            result.Add(OUTPUT_FILE_OPTION);
            result.Add(project.OutputFile);

            return result;
        }

        #endregion
    }
}
