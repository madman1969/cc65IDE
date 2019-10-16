using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace cc65Wrapper
{
    /// <summary>
    /// Class defining the elements of a project
    /// </summary>
    public class Cc65Project
    {
        public const int VERSION = 1000;

        public string WorkingDirectory { get; set; }
        public string TargetPlatform { get; set; }
        public List<string> InputFiles { get; set; }
        public string OutputFile { get; set; }
        public bool OptimiseCode { get; set; }
        public int Version { get; set; }

        public string FullOutputFilePath => Path.Combine(WorkingDirectory, OutputFile);

        /// <summary>
        ///  Class Constructor
        /// </summary>
        public Cc65Project()
        {
            WorkingDirectory = string.Empty;
            TargetPlatform = "pet";
            InputFiles = new List<string>();
            OutputFile = string.Empty;
            OptimiseCode = false;
            Version = VERSION;
        }

        /// <summary>
        /// Retrieves JSON representation of a project
        /// </summary>
        /// <returns></returns>
        public string AsJson()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
