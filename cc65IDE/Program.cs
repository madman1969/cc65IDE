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
                "inflate.c"
            };

            // Define a new project with some source files and output file ...
            var project = new Cc65Project
            {
                TargetPlatform = "pet",
                OptimiseCode = true,
                WorkingDirectory = @"D:\commodore stuff\Pet Stuff\csource\inflate",
                InputFiles = inputFiles,
                OutputFile = "inflate"
            };

            var result = await Cc65Build.Compile(project);
        }
    }
}
