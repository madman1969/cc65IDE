using CliWrap;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace cc65Wrapper
{
    /// <summary>
    /// Wrapper class for CC65. Allows building of defined project
    /// </summary>
    public class Cc65Build
    {
        /// <summary>
        /// Define more readable placeholders for cl65 cmd line options
        /// </summary>
        const string CL65 = "cl65";
        const string CC65_TARGET = "CC65_TARGET";
        const string TARGET_OPTION = "-t";
        const string OUTPUT_FILE_OPTION = "-o";
        const string OPTIMISE_OPTION = "-O";

        /// <summary>
        /// Compiles source file associated with project file into output file using 'cl65'
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static async Task<CliWrap.Models.ExecutionResult> Compile(Cc65Project project)
        {
            CliWrap.Models.ExecutionResult result;

            var originalDir = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(project.WorkingDirectory);

                var argumentList = BuildArgumentsList(project);

                result = await Cli.Wrap(CL65)
                .SetEnvironmentVariable(CC65_TARGET, project.TargetPlatform)
                .SetArguments(argumentList)
                .EnableExitCodeValidation(false)
                .ExecuteAsync();

                var exitCode = result.ExitCode;
                var stdOut = result.StandardOutput;
                var stdErr = result.StandardError;
                var startTime = result.StartTime;
                var exitTime = result.ExitTime;
                var runTime = result.RunTime;
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDir);
            }

            return result;
        }

        /// <summary>
        /// Builds from supplied project file a list of string arguments to pass to 'cl65'
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        private static List<string> BuildArgumentsList(Cc65Project project)
        {
            var result = new List<string>();

            // Add target args ...
            result.Add(TARGET_OPTION);
            result.Add(project.TargetPlatform);

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
    }
}
