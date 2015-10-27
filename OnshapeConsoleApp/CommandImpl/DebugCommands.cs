using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.ConsoleApp.CommandImpl
{
    internal static class DebugCommands
    {
        internal static async Task GetDebugContext(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            await Task.Run(() => Utils.PrintContext(context));
        }
        internal static async Task ClearContext(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            await Task.Run(() => Utils.ClearContext());
        }
    }
}
