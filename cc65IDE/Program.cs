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

            var project = new CC65Project
            {
                TargetPlatform = "pet",
                OptimiseCode = true,
                WorkingDirectory = @"D:\commodore stuff\Pet Stuff\csource\inflate",
                InputFiles = inputFiles,
                OutputFile = "inflate"                
            };
            
            var result = await Cc65Wrapper.Compile(project);
        }
    }
}
