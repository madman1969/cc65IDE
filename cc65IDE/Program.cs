using cc65Wrapper;
using System.Collections.Generic;

namespace cc65IDE
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var inputFiles = new List<string>
            {
                "main.c",
                "draw.c"
            };

            // Define a new project with some source files and output file ...
            var project = new Cc65Project
            {
                TargetPlatform = "pet",
                OptimiseCode = true,
                WorkingDirectory = @"C:\Users\aross\source\repos\Draw",
                InputFiles = inputFiles,
                OutputFile = "Draw"
            };

            // Get JSON representation of project ...
            var projectJSON = project.AsJson();

            // Compile the project ...
            var result = await Cc65Build.Compile(project);

        }
    }
}
