using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.ConsoleApp
{
    internal class CommandLineParseException : Exception
    {
        public CommandLineParseException()
        {
        }

        public CommandLineParseException(string message)
            : base(message)
        {
        }

        public CommandLineParseException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
