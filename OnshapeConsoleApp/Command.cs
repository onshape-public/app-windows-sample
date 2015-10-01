using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.ConsoleApp
{
    internal class Command
    {
        public delegate Task DoWork(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values);
        public string Name { get; set; }
        public List<CommandOption> Options { get; set; }
        public string Description { get; set; }
        public DoWork Worker { get; set; }
        public int MinArgs { get; set; }
        public int MaxArgs { get; set; }
    }
}
