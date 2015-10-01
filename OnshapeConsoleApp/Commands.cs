using Newtonsoft.Json;
using Onshape.Api.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Onshape.Api.ConsoleApp
{
    internal static class Commands
    {
        #region Commands dictionary

        internal static Dictionary<String, Command> commands = new Dictionary<String, Command>() {
            {@"GET", new Command {
                Description = @"Generic GET: 'onshapeConsoleApp GET uri'", 
                Worker = GenericGet, 
                Options = new List<CommandOption> { 
                }}},
            {@"POST", new Command {
                Description = @"Generic POST: 'onshapeConsoleApp POST uri 'jsonBody''", 
                Worker = GenericPost, 
                Options = new List<CommandOption> { 
                }}},
            {@"DELETE", new Command {
                Description = @"Generic DELETE: 'onshapeConsoleApp DELETE uri'", 
                Worker = GenericDelete, 
                Options = new List<CommandOption> { 
                }}},
            {@"GET_DOCUMENTS", new Command {
                Description = @"Get documents: 'onshapeConsoleApp GET documents [-d <did>]'", 
                Worker = GetDocuments, 
                Options = new List<CommandOption> { 
                    new CommandOption {Reqiuired = false, Token = Constants.DOCUMENT_ID, MinArgs = 1, MaxArgs = 1}
                }}},
            {@"POST_DOCUMENTS", new Command { 
                Description = @"Create a new document: 'onshapeConsoleApp POST documents <documentName>'",
                Worker = CreateDocument,
                MinArgs = 1,
                MaxArgs = 1,
                Options = new List<CommandOption> { 
                }}},
            {@"DELETE_DOCUMENTS", new Command {
                Description = @"Delete existing document: 'onshapeConsoleApp DELETE documents -d <did>'",
                Worker = DeleteDocument,
                Options = new List<CommandOption> { 
                    new CommandOption {Reqiuired = true, Token = Constants.DOCUMENT_ID, MinArgs = 1, MaxArgs = 1}
                }}},
            {@"GET_WORKSPACES",new Command { 
                Description = @"Get workspaces: GET workspaces 'onshapeConsoleApp -d <did> [-w <wid>]'",
                Worker = GetWorkspaces,
                Options = new List<CommandOption> { 
                    new CommandOption {Reqiuired = true, Token = Constants.DOCUMENT_ID, MinArgs = 1, MaxArgs = 1},
                    new CommandOption {Reqiuired = false, Token = Constants.WORKSPACE_ID, MinArgs = 1, MaxArgs = 1}
                }}},
            {@"DELETE_WORKSPACES", new Command {
                Description = @"Delete existing workspace: 'onshapeConsoleApp DELETE workspaces -d <did> -w <wid>'",
                Worker = GetWorkspaces,
                Options = new List<CommandOption> { 
                    new CommandOption {Reqiuired = true, Token = Constants.DOCUMENT_ID, MinArgs = 1, MaxArgs = 1},
                    new CommandOption {Reqiuired = true, Token = Constants.WORKSPACE_ID, MinArgs = 1, MaxArgs = 1}
                }}},
            {@"GET_VERSIONS", new Command {
                Description = @"Get versions: 'onshapeConsoleApp GET versions -d <did> [-v <vid>]'",
                Worker = GetVersions,
                Options = new List<CommandOption> { 
                    new CommandOption {Reqiuired = true, Token = Constants.DOCUMENT_ID, MinArgs = 1, MaxArgs = 1},
                    new CommandOption {Reqiuired = false, Token = Constants.VERSION_ID, MinArgs = 1, MaxArgs = 1}
                }}},
            {@"DELETE_VERSIONS", new Command {
                Description = @"Detele existing version: 'onshapeConsoleApp DELETE versions -d <did> -v <vid>'",
                Worker = DeleteVersion,
                Options = new List<CommandOption> { 
                    new CommandOption {Reqiuired = true, Token = Constants.DOCUMENT_ID, MinArgs = 1, MaxArgs = 1},
                    new CommandOption {Reqiuired = true, Token = Constants.VERSION_ID, MinArgs = 1, MaxArgs = 1}
                }}},
            {@"GET_USERS", new Command {
                Description = @"Get user: 'onshapeConsoleApp GET users -u <uid>'",
                Worker = GetUser,
                Options = new List<CommandOption> { 
                    new CommandOption {Reqiuired = true, Token = Constants.USER_ID, MinArgs = 1, MaxArgs = 1}
                }}},
            {@"GET_ELEMENTS", new Command {
                Description = @"Get elements: 'onshapeConsoleApp GET elements -d <did> [-v <vid> | -w <wid>] [-e <eid>]'",
                Worker = GetElements,
                Options = new List<CommandOption> { 
                    new CommandOption {Reqiuired = true, Token = Constants.DOCUMENT_ID, MinArgs = 1, MaxArgs = 1},
                    new CommandOption {Reqiuired = true, Token = Constants.WORKSPACE_ID, MinArgs = 1, MaxArgs = 1, MutuallyExclusive = new HashSet<String> {Constants.VERSION_ID}},
                    new CommandOption {Reqiuired = true, Token = Constants.VERSION_ID, MinArgs = 1, MaxArgs = 1, MutuallyExclusive = new HashSet<String> {Constants.WORKSPACE_ID}},
                    new CommandOption {Reqiuired = false, Token = Constants.ELEMENT_ID, MinArgs = 1, MaxArgs = 1},
                }}},
            {@"HELP", new Command {
                Name = @"HELP",
                Description = @"Get help: 'onshapeConsoleApp -h [topic]'",
                Worker = Help,
                MinArgs = 0,
                MaxArgs = 1,
                Options = new List<CommandOption> { 
                }}},
            {@"EXIT", new Command {
                Description = @"Exit interactive mode: 'exit'",
                Worker = Exit,
                MinArgs = 0,
                MaxArgs = 0,
                Options = new List<CommandOption> { 
                }}}
        };

        #endregion

        #region Command implementation

        internal static async Task GenericGet(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            if (values != null && values.Count == 1)
            {
                var response = await context.Client.HttpGet(context.BaseURL + values[0]);
                Console.WriteLine(String.Format("Response:\n{0}\nBody:\n{1}", response, await response.Content.ReadAsStringAsync()));
            }
        }

        internal static async Task GenericPost(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            if (values != null && values.Count == 2)
            {
                var response = await context.Client.HttpPostJson(context.BaseURL + values[0], values[1]);
                Console.WriteLine(String.Format("Response:\n{0}\nBody:\n{1}", response, await response.Content.ReadAsStringAsync()));
            }
        }

        internal static async Task GenericDelete(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            if (values != null && values.Count == 1)
            {
                Console.WriteLine(await context.Client.HttpDelete(context.BaseURL + values[0]));
            }
        }

        internal static async Task Upload(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            Dictionary<String, String> formFields = new Dictionary<String, String> {
                {"flattenAssemblies", "false"},
                {"yAcisIsUp", "false"},
                {"ownerId", "undefined"},
                {"encodedFileName", HttpUtility.UrlEncode(System.IO.Path.GetFileName(options[Constants.FILE][0]))}
            };
            var response = await context.Client.HttpPostMultipartFormData(context.BaseURL + "/api/elements/upload/" + options[Constants.DOCUMENT_ID][0], formFields, options[Constants.FILE][0]);
            Console.WriteLine(String.Format("Response:\n{0}\nBody:\n{1}", response, await response.Content.ReadAsStringAsync()));
        }

        internal static async Task GetDocuments(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            if (options.ContainsKey(Constants.DOCUMENT_ID))
            {
                OnshapeDocument document = await context.Client.GetDocument(options[Constants.DOCUMENT_ID][0]);
                Console.WriteLine(JsonConvert.SerializeObject(document));
            }
            else if (values != null && values.Count > 0)
            {
                foreach (String v in values)
                {
                    OnshapeDocument document = await context.Client.GetDocument(v);
                    Console.WriteLine(JsonConvert.SerializeObject(document));
                }
            }
            else 
            {
                List<OnshapeDocument> documents = await context.Client.GetDocuments();
                Console.WriteLine(JsonConvert.SerializeObject(documents));
            }
        }

        internal static async Task CreateDocument(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            OnshapeDocument document = await context.Client.CreateDocument(values[0]);
            Console.WriteLine(JsonConvert.SerializeObject(document));
        }

        internal static async Task DeleteDocument(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            await context.Client.DeleteDocument(options[Constants.DOCUMENT_ID][0]);
        }

        internal static async Task GetWorkspaces(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            if (options.ContainsKey(Constants.WORKSPACE_ID))
            {
                OnshapeWorkspace workspace = await context.Client.GetWorkspace(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0]);
                Console.WriteLine(JsonConvert.SerializeObject(workspace));
            }
            else
            {
                List<OnshapeWorkspace> workspaces = await context.Client.GetWorkspaces(options[Constants.DOCUMENT_ID][0]);
                Console.WriteLine(JsonConvert.SerializeObject(workspaces));
            }
        }

        internal static async Task DeleteWorkspace(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            await context.Client.DeleteWorkspace(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0]);
        }

        internal static async Task GetVersions(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            if (options.ContainsKey(Constants.VERSION_ID))
            {
                OnshapeVersion version = await context.Client.GetVersion(options[Constants.DOCUMENT_ID][0], options[Constants.VERSION_ID][0]);
                Console.WriteLine(JsonConvert.SerializeObject(version));
            }
            else
            {
                List<OnshapeVersion> versions = await context.Client.GetVersions(options[Constants.DOCUMENT_ID][0]);
                Console.WriteLine(JsonConvert.SerializeObject(versions));
            }
        }

        internal static async Task DeleteVersion(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            await context.Client.DeleteVersion(options[Constants.DOCUMENT_ID][0], options[Constants.VERSION_ID][0]);
        }

        internal static async Task GetUser(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            OnshapeUser user = await context.Client.GetUser(options[Constants.USER_ID][0]);
            Console.WriteLine(JsonConvert.SerializeObject(user));
        }

        internal static async Task GetElements(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            if (options.ContainsKey(Constants.WORKSPACE_ID))
            {
                if (options.ContainsKey(Constants.ELEMENT_ID))
                {
                    OnshapeElement element = await context.Client.GetWorkspaceElement(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0], options[Constants.ELEMENT_ID][0]);
                    Console.WriteLine(JsonConvert.SerializeObject(element));
                }
                else
                {
                    List<OnshapeElement> elements = await context.Client.GetWorkspaceElements(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0]);
                    Console.WriteLine(JsonConvert.SerializeObject(elements));
                }
            }
            else
            {
                if (options.ContainsKey(Constants.ELEMENT_ID))
                {
                    OnshapeElement element = await context.Client.GetVersionElement(options[Constants.DOCUMENT_ID][0], options[Constants.VERSION_ID][0], options[Constants.ELEMENT_ID][0]);
                    Console.WriteLine(JsonConvert.SerializeObject(element));
                }
                else
                {
                    List<OnshapeElement> elements = await context.Client.GetVersionElements(options[Constants.DOCUMENT_ID][0], options[Constants.VERSION_ID][0]);
                    Console.WriteLine(JsonConvert.SerializeObject(elements));
                }
            }
        }

        internal static async Task Help(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            await Task.Run(() => PrintHelp(options, values));
        }

        internal static void PrintCommandHelp(string prefix, Command command)
        {
            Console.WriteLine(String.Format("{0}{1}", prefix, command.Description));
        }

        internal static void PrintHelp(Dictionary<string, List<string>> options, List<string> values)
        {
            if (values != null && values.Count > 0)
            {
                string commandName = String.Join(@"_", values).ToUpperInvariant();
                if (commands.ContainsKey(commandName))
                {
                    PrintCommandHelp("\t", commands[commandName]);
                }
                else
                {
                    Console.WriteLine(String.Format("\tUnknown command: {0}", commandName));
                }
            }
            else
            {
                Console.WriteLine("Usage: ConsoleApp.exe COMMAND [-i] [OPTIONS]");
                foreach (String name in commands.Keys)
                {
                    PrintCommandHelp("\t", commands[name]);
                }
            }
        }

        internal static async Task Exit(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            await Task.Run(() => context.InteractiveMode = false);
        }

        #endregion
    }
}
