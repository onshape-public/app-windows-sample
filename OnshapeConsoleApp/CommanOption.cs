using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.ConsoleApp
{
    internal class CommandOption
    {
        public HashSet<String> MutuallyExclusive { get; set; }
        public String Description { get; set; }
        public Boolean Required { get; set; }
        public string Token { get; set; }
        public int MinArgs { get; set; }
        public int MaxArgs { get; set; }
    }
}
