using cc65Wrapper;
using System;
using System.IO;

namespace cc65IDE
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            //var inputFiles = new List<string>
            //{
            //    "main.c",
            //    "draw.c"
            //};

            //// Define a new project with some source files and output file ...
            //var project = new Cc65Project
            //{
            //    ProjectName = "Draw",
            //    TargetPlatform = "pet",
            //    OptimiseCode = true,
            //    WorkingDirectory = @"C:\Users\aross\source\repos\Draw",
            //    InputFiles = inputFiles,
            //    OutputFile = "Draw"
            //};

            //// Get JSON representation of project ...
            //var projectJSON = project.AsJson();

            // Load the project JSON ...
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files");
            filepath = Path.Combine(filepath, "project.json");
            var json = File.ReadAllText(filepath);
            var project = Cc65Project.FromJson(json);

            // Load the emulators JSON ...
            filepath = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files");
            filepath = Path.Combine(filepath, "emulators.json");
            json = File.ReadAllText(filepath);
            var emu = Cc65Emulators.FromJson(json);

            // Compile the project ...
            var result = await Cc65Build.Compile(project);

            if (result != null && result.ExitCode == 0)
                Console.WriteLine("Successfully built project");
            else
                Console.WriteLine("Faild to build project !");
        }
    }
}
