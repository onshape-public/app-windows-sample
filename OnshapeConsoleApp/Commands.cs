using Newtonsoft.Json;
using Onshape.Api.Client.Model;
using Onshape.Api.ConsoleApp.CommandImpl;
using System;
using System.Collections.Generic;
using System.IO;
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
                Description = @"Generic GET request",
                Examples = new List<string> {
                    @"get <uri>",
                    @"g /api/documents",
                    @"g /api/users/current"
                },
                Worker = GenericGet, 
                Options = new List<CommandOption> { 
                }}},
            {@"POST", new Command {
                Description = @"Generic POST request",
                Examples = new List<string> {
                    @"post uri 'jsonBody'",
                    @"p /api/documents '{""name"":""Doc name""}'"
                },
                Worker = GenericPost, 
                Options = new List<CommandOption> { 
                }}},
            {@"DELETE", new Command {
                Description = @"Generic DELETE request",
                Examples = new List<string> {
                    @"delete <uri>",
                    @"d /api/documents/<did>"
                },
                Worker = GenericDelete, 
                Options = new List<CommandOption> { 
                }}},
            {@"GET_DOCUMENTS", new Command {
                Description = @"Get documents", 
                Worker = GetDocuments, 
                Examples = new List<string> {
                    @"get documents [<did>]",
                    @"g documents",
                    @"g documents 8b803ff47462494dafecc822"
                },
                Options = new List<CommandOption> { 
                    new CommandOption {Required = false, Token = Constants.DOCUMENT_ID, MinArgs = 1, MaxArgs = 1, Description = "Document Id"}
                }}},
            {@"POST_DOCUMENTS", new Command { 
                Description = @"Create a new document",
                Worker = CreateDocument,
                Examples = new List<string> {
                    @"post documents <documentName>",
                    @"p documents 'New document'"
                },
                MinArgs = 1,
                MaxArgs = 1,
                Options = new List<CommandOption> { 
                }}},
            {@"GET_WORKSPACES",new Command { 
                Description = @"Get workspaces",
                Worker = GetWorkspaces,
                Examples = new List<string> {
                    @"get workspaces -d <did> [-w <wid>]"
                },
                Options = new List<CommandOption> { 
                    new CommandOption {Required = true, Token = Constants.DOCUMENT_ID, MinArgs = 1, MaxArgs = 1, Description = "document id"},
                    new CommandOption {Required = false, Token = Constants.WORKSPACE_ID, MinArgs = 1, MaxArgs = 1, Description = "workspace id"}
                }}},
            {@"GET_VERSIONS", new Command {
                Description = @"Get versions: 'onshapeConsoleApp GET versions -d <did> [-v <vid>]'",
                Worker = GetVersions,
                Examples = new List<string> {
                    @"get versions -d <did> [-v <vid>]",
                    @"g versions -d <did>"
                },
                Options = new List<CommandOption> { 
                    new CommandOption {Required = true, Token = Constants.DOCUMENT_ID, MinArgs = 1, MaxArgs = 1, Description = "document id"},
                    new CommandOption {Required = false, Token = Constants.VERSION_ID, MinArgs = 1, MaxArgs = 1}
                }}},
            {@"GET_USERS", new Command {
                Description = @"Get user: 'onshapeConsoleApp GET users -u <uid>'",
                Worker = GetUser,
                Examples = new List<string> {
                    @"get users <uid>",
                    @"g users current"
                },
                Options = new List<CommandOption> { 
                    new CommandOption {Required = true, Token = Constants.USER_ID, MinArgs = 1, MaxArgs = 1, Description = "user id"}
                }}},
            {@"GET_ELEMENTS", new Command {
                Description = @"Get elements",
                Worker = GetElements,
                Examples = new List<string> {
                    @"get elements -d <did> [-v <vid> | -w <wid>] [-e <eid>]"
                },
                Options = new List<CommandOption> { 
                    new CommandOption {Required = true, Token = Constants.DOCUMENT_ID, MinArgs = 1, MaxArgs = 1, Description = "document id"},
                    new CommandOption {Required = true, Token = Constants.WORKSPACE_ID, MinArgs = 1, MaxArgs = 1, MutuallyExclusive = new HashSet<String> {Constants.VERSION_ID}, Description = "workspace id"},
                    new CommandOption {Required = true, Token = Constants.VERSION_ID, MinArgs = 1, MaxArgs = 1, MutuallyExclusive = new HashSet<String> {Constants.WORKSPACE_ID}, Description = "version id"},
                    new CommandOption {Required = false, Token = Constants.ELEMENT_ID, MinArgs = 1, MaxArgs = 1},
                }}},
            {@"DOWNLOAD_PARTSTUDIO", new Command {
                Description = @"Download partstudio",
                Worker = DownloadPartstudio,
                Examples = new List<string> {
                    @"download partstudio -d <did> (-v <vid> | -w <wid>) -e <eid> [-f fileName] [stl|parasolid]",
                    @"download partstudio -d DID -w WID -e EID -f 'c:/dev/p.stl' stl"
                },
                Options = new List<CommandOption> { 
                    new CommandOption {Required = true, Token = Constants.DOCUMENT_ID, MinArgs = 1, MaxArgs = 1, Description = "document id"},
                    new CommandOption {Required = true, Token = Constants.WORKSPACE_ID, MinArgs = 1, MaxArgs = 1, MutuallyExclusive = new HashSet<String> {Constants.VERSION_ID}, Description = "workspace id"},
                    new CommandOption {Required = true, Token = Constants.VERSION_ID, MinArgs = 1, MaxArgs = 1, MutuallyExclusive = new HashSet<String> {Constants.WORKSPACE_ID}, Description = "version id"},
                    new CommandOption {Required = true, Token = Constants.ELEMENT_ID, MinArgs = 1, MaxArgs = 1},
                    new CommandOption {Required = false, Token = Constants.FILE, MinArgs = 0, MaxArgs = 1, Description = "file name"},
                }}},
            {@"GET_PLANS", new Command {
                Description = @"Get purchasable plans",
                Worker = BillingCommands.GetPlans,
                Examples = new List<string> {
                    @"GET plans [<planId>]",
                    @"g plans"
                },
                Options = new List<CommandOption> { 
                }}},
            {@"GET_PURCHASES", new Command {
                Description = @"Get purchases",
                Worker = BillingCommands.GetPurchases,
                Examples = new List<string> {
                    @"get purchases"
                },
                Options = new List<CommandOption> { 
                }}},
            {@"CANCEL_PURCHASE", new Command {
                Description = @"Cancel purchase",
                Worker = BillingCommands.CancelPurchase,
                Examples = new List<string> {
                    @"cancel purchase <purchaseId>"
                },
                Options = new List<CommandOption> { 
                }}},
            {@"PURCHASE", new Command {
                Description = @"Purchase an application purchasable item",
                Worker = BillingCommands.Purchase,
                Examples = new List<string> {
                    @"purchase <sku>",
                    @"purchase ENTERPRISE"
                },
                Options = new List<CommandOption> { 
                }}},
            {@"HELP", new Command {
                Name = @"HELP",
                Description = @"Get help",
                Worker = Help,
                Examples = new List<string> {
                    @"help [<topic>]",
                    @"help download partstudio",
                    @"? -i",
                    @"?"
                },
                MinArgs = 0,
                MaxArgs = 1,
                Options = new List<CommandOption> { 
                }}},
            {@"EXIT", new Command {
                Description = @"Exit interactive mode",
                Worker = Exit,
                Examples = new List<string> {
                    @"exit",
                    @"q"
                },
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
                var response = await context.Client.HttpGet(values[0].StartsWith(context.BaseURL) ? values[0]: context.BaseURL + values[0]);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    response = await context.Client.HttpGet(response.RequestMessage.RequestUri.ToString());
                }
                Console.WriteLine(String.Format("Response:\n{0}\nBody:\n{1}", response, await response.Content.ReadAsStringAsync()));
            }
        }

        internal static async Task GenericPost(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            if (values != null && values.Count == 2)
            {
                var response = await context.Client.HttpPostJson(values[0].StartsWith(context.BaseURL) ? values[0] : context.BaseURL + values[0], values[1]);
                Console.WriteLine(String.Format("Response:\n{0}\nBody:\n{1}", response, await response.Content.ReadAsStringAsync()));
            }
        }

        internal static async Task GenericDelete(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            if (values != null && values.Count == 1)
            {
                Console.WriteLine(await context.Client.HttpDelete(values[0].StartsWith(context.BaseURL) ? values[0] : context.BaseURL + values[0]));
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
                documents.Print();
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
        
        internal static async Task DownloadPartstudio(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            string documentId = options[Constants.DOCUMENT_ID][0];
            string wmvSelector = (options.ContainsKey(Constants.WORKSPACE_ID)) ? "w" : "v";
            string selectorId = (options.ContainsKey(Constants.WORKSPACE_ID)) ? options[Constants.WORKSPACE_ID][0] : options[Constants.VERSION_ID][0];
            string elementId = options[Constants.ELEMENT_ID][0];
            string format = (values != null && values.Count == 1) ? values[0] : "stl";
            using(Stream contentStream = await context.Client.DownloadPartstudio(documentId, wmvSelector, selectorId, elementId, format)) 
            {
                if (contentStream != null)
                {
                    string fileName = options.ContainsKey(Constants.FILE) ? options[Constants.FILE][0] : null;
                    if (fileName != null)
                    {
                        using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, 1012 * 1024, true))
                        {
                            await contentStream.CopyToAsync(stream);
                            Console.WriteLine("The partstudio has been downloaded to {0}", fileName);
                        }
                    }
                    else
                    {
                        long bufferLength = (contentStream.Length>Constants.MAX_FILE_LENGTH_TO_PRINT_OUT)?Constants.MAX_FILE_LENGTH_TO_PRINT_OUT:contentStream.Length;
                        byte[] buffer = new byte[bufferLength];
                        contentStream.Read(buffer, 0, (int)contentStream.Length);
                        Console.WriteLine(Encoding.ASCII.GetString(buffer));
                        if (contentStream.Length > Constants.MAX_FILE_LENGTH_TO_PRINT_OUT)
                        {
                            Console.WriteLine("...");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Download failed");
                }
            }
        }

        internal static async Task Help(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            await Task.Run(() => PrintHelp(options, values));
        }

        internal static void PrintCommandHelp(string prefix, Command command)
        {
            Console.WriteLine(String.Format("{0}{1}:", prefix, command.Description));
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                foreach (var s in command.Examples)
                {
                    Console.WriteLine(String.Format("{0}{0}>{2}", prefix, command.Description, s));
                }
            }
            finally
            {
                Console.ResetColor();
            }
            
        }

        internal static void PrintHelp(Dictionary<string, List<string>> options, List<string> values)
        {
            if (values != null && values.Count > 0)
            {
                string commandName = String.Join(@"_", values).ToUpperInvariant();
                if (commands.ContainsKey(commandName))
                {
                    PrintCommandHelp("  ", commands[commandName]);
                }
                else
                {
                    Console.WriteLine(String.Format("  Unknown command: {0}", commandName));
                }
            }
            else
            {
                Console.WriteLine("Usage: ConsoleApp.exe COMMAND [-i] [OPTIONS]");
                foreach (String name in commands.Keys)
                {
                    PrintCommandHelp("  ", commands[name]);
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
