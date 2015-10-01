using Onshape.Api.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.ConsoleApp
{
    internal class CommandExecutionContext
    {
        public String BaseURL { get; set; }
        public String OAuthToken { get; set; }
        public String OAuthRefreshToken { get; set; }
        public Boolean InteractiveMode { get; set; }
        public OnshapeClient Client { get; set; }
    }
}
