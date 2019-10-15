using Newtonsoft.Json;
using System.Collections.Generic;

namespace cc65Wrapper
{
    public class CC65Project
    {
        const int VERSION = 1000;

        public string WorkingDirectory { get; set; }
        public string TargetPlatform { get; set; }
        public List<string> InputFiles { get; set; }
        public string OutputFile { get; set; }
        public bool OptimiseCode { get; set; }

        public int Version { get; set; }

        public CC65Project()
        {
            Version = VERSION;
        }

        public string AsJson()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
