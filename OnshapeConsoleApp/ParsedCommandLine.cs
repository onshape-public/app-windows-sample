using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.ConsoleApp
{
    internal class ParsedCommandLine
    {
        public string[] Args { get; set; }
        public int NextArg { get; set; }
        public Command Command { get; set; }
        public Dictionary<string, List<string>> Options { get; set; }
        public List<string> Values { get; set; }
    }
}
