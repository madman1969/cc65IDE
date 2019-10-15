using CliWrap;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace cc65Wrapper
{
    public class Cc65Wrapper
    {
        const string CL65 = "cl65";
        const string CC65_TARGET = "CC65_TARGET";
        const string TARGET_OPTION = "-t";
        const string OUTPUT_FILE_OPTION = "-o";
        const string OPTIMISE_OPTION = "-O";

        public static async Task<CliWrap.Models.ExecutionResult> Compile(CC65Project project)
        {
	        CliWrap.Models.ExecutionResult result;
	
	        var originalDir = Directory.GetCurrentDirectory();
	
	        try
	        {
		        Directory.SetCurrentDirectory(project.WorkingDirectory);

		        result = await Cli.Wrap(CL65)
		        .SetEnvironmentVariable(CC65_TARGET, project.TargetPlatform)
		        .SetArguments(BuildArgumentsList(project))
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

        private static List<string> BuildArgumentsList(CC65Project project)
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
