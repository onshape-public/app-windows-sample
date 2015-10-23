using Onshape.Api.Client;
using OnshapeAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Onshape.Api.ConsoleApp
{
    class Program
    {
        #region global context
        
        internal static Dictionary<String, List<String>> OptionValueCache { get; set; }

        internal static CommandExecutionContext BaseExecutionContext { get; set; }

        #endregion

        #region Command line utils

        private static ParsedCommandLine ParseCommand(ParsedCommandLine cmdLine)
        {
            StringBuilder commandName = new StringBuilder();
            int index = cmdLine.NextArg;
            while (index < cmdLine.Args.Length && Constants.commandTokens.ContainsKey(cmdLine.Args[index].ToUpperInvariant()))
            {
                if (commandName.Length > 0)
                {
                    commandName.Append("_");
                }
                commandName.Append(Constants.commandTokens[cmdLine.Args[index++].ToUpperInvariant()]);
                if (Commands.commands.ContainsKey(commandName.ToString()))
                {
                    cmdLine.Command = Commands.commands[commandName.ToString()];
                    cmdLine.NextArg = index;
                }
            }
            if (cmdLine.Command == null)
            {
                throw new CommandLineParseException(String.Format(@"Unrecognized command: {0}", cmdLine.Args[cmdLine.NextArg]));
            }
            return cmdLine;
        }

        private static ParsedCommandLine ParseOptions(ParsedCommandLine cmdLine)
        {
            Dictionary<String, List<String>> optionValues = new Dictionary<String, List<String>>();
            List<CommandOption> options = cmdLine.Command.Options;
            CommandOption currentOption = null;
            List<String> currentValues = null;
            int index = cmdLine.NextArg;
            while (index < cmdLine.Args.Length)
            {
                String token = cmdLine.Args[index];
                if (currentOption == null)
                {
                    if (Constants.optionTokens.ContainsKey(token))
                    {
                        currentOption = options.Where(o => String.Equals(Constants.optionTokens[token], o.Token, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        if (currentOption == null)
                        {
                            currentOption = Constants.globalOptions.Where(o => String.Equals(Constants.optionTokens[token], o.Token, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        }
                        if (currentOption != null)
                        {
                            if (optionValues.ContainsKey(Constants.optionTokens[token]))
                            {
                                currentValues = optionValues[currentOption.Token];
                            }
                            else
                            {
                                currentValues = new List<String>();
                                optionValues.Add(currentOption.Token, currentValues);
                                OptionValueCache[currentOption.Token] = currentValues;
                            }
                            ++index;
                            continue;
                        }
                        else
                        {
                            throw new CommandLineParseException(String.Format(@"Unexpected option: {0}", token));
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (Constants.optionTokens.ContainsKey(token))
                {
                    // New option
                    currentValues = null;
                    currentOption = null;
                    continue;
                }
                else {
                    // Option value
                    if (currentOption.MaxArgs == 0)
                    {
                        break;
                    }
                    currentValues.Add(token);
                    ++index;
                    if (currentOption.MaxArgs == currentValues.Count)
                    {
                        currentValues = null;
                        currentOption = null;
                        continue;
                    }
                }
            }
            foreach(CommandOption o in options) {
                if (o.Required && !optionValues.ContainsKey(o.Token) && o.MutuallyExclusive == null)
                {
                    if (OptionValueCache.ContainsKey(o.Token))
                    {
                        optionValues.Add(o.Token, OptionValueCache[o.Token]);
                    }
                    else
                    {
                        throw new CommandLineParseException(String.Format(@"{0} is required", o.Token));
                    }
                }
                if (optionValues.ContainsKey(o.Token))
                {
                    int argCount = optionValues[o.Token] == null ? 0 : optionValues[o.Token].Count;
                    if (argCount > o.MaxArgs || argCount < o.MinArgs)
                    {
                        throw new CommandLineParseException(String.Format(@"Invalid option {0}", o.Token));
                    }
                    if (o.MutuallyExclusive != null)
                    {
                        foreach (var name in o.MutuallyExclusive)
                        {
                            if (optionValues.ContainsKey(name))
                            {
                                List<String> mutuallyExclusiveOptions = new List<String> { o.Token };
                                mutuallyExclusiveOptions.AddRange(o.MutuallyExclusive);
                                throw new CommandLineParseException(String.Format(@"One of {0} is expected", String.Join(", ", mutuallyExclusiveOptions)));
                            }
                        }
                    }
                }
            }
            cmdLine.Options = optionValues;
            cmdLine.NextArg = index;
            return cmdLine;
        }

        private static ParsedCommandLine ParseValues(ParsedCommandLine cmdLine)
        {
            int index = cmdLine.NextArg;
            while (index < cmdLine.Args.Length)
            {
                String token = cmdLine.Args[index++];
                if (cmdLine.Values == null)
                {
                    cmdLine.Values = new List<string> ();
                }
                cmdLine.Values.Add(token);
            }
            return cmdLine;
        }
        
        private static ParsedCommandLine ParseCommandLine(string[] args)
        {
            // The command line is expected to have the following structure:
            // COMMAND [OPTION_FLAG_1 [OPTION_VALUE_1 ...]] [...] [VALUE_1] [...]
            ParsedCommandLine result = new ParsedCommandLine { Args = args };
            if (args.Length < 1)
            {
                throw new CommandLineParseException(@"Invalid arguments");
            }
            return ParseValues(ParseOptions(ParseCommand(result)));
        }

        private static void printUsage(string errorMessage, Dictionary<string, List<string>> options, List<string> values)
        {
            if (errorMessage != null)
            {
                Console.WriteLine(String.Format("Error: {0}", errorMessage));
            }
            Commands.PrintHelp(options, values);
        }

        #endregion

        #region Command execution context 

        private static CommandExecutionContext ConstructExecutionContext(String baseUri, String clientId, String clientSecret, String oauthToken, String oauthRefreshToken, Boolean interactiveMode)
        {
            if (oauthToken == null)
            {
                if (String.IsNullOrWhiteSpace(baseUri))
                {
                    throw new Exception("'baseUri' is not specified: Please, use -b command line option or set ONSHAPE_BASE_URI environment variable");
                }
                if (String.IsNullOrWhiteSpace(clientId))
                {
                    throw new Exception("'clientId' is not specified: Please, set ONSHAPE_CLIENT_ID environment variable");
                }
                if (String.IsNullOrWhiteSpace(clientSecret))
                {
                    throw new Exception("'clientSecret' is not specified: Please, set ONSHAPE_CLIENT_SECRET environment variable");
                }

                // Read refresh token from registry
                oauthRefreshToken = Utils.GetRegistry(Constants.REFRESH_TOKEN_KEY_NAME);

                if (String.IsNullOrEmpty(oauthRefreshToken))
                {
                    // Authenticate as Onshape Application
                    OnshapeOAuth onshapeOAuth = new OnshapeOAuth(baseUri, clientId, clientSecret);
                    Console.WriteLine("Opening browser window for Onshape authentication...");
                    onshapeOAuth.AuthenticateBlocking();
                    oauthToken = onshapeOAuth.AccessToken;
                    oauthRefreshToken = onshapeOAuth.RefreshToken;
                    Utils.SetRegistry(Constants.REFRESH_TOKEN_KEY_NAME, oauthRefreshToken);
                }
            }

            // Initialize Onshape client
            OnshapeClient client = new OnshapeClient { AccessToken = oauthToken, RefreshToken = oauthRefreshToken, BaseUri = baseUri, ClientId = clientId, ClientSecret = clientSecret };

            // Refresh token if needed
            if (oauthToken == null)
            {
                Task<string> t = client.GetRefreshedOAuthToken();
                try {
                    t.Wait(Constants.TOKEN_REFRESH_TIME_OUT);
                    client.AccessToken = t.Result;
                }
                catch (Exception) 
                {
                    Console.WriteLine("Error: token refresh failed");
                }
            }

            // Construct command execution context
            return new CommandExecutionContext { BaseURL = baseUri, InteractiveMode = interactiveMode, OAuthToken = oauthToken, OAuthRefreshToken = oauthRefreshToken, Client = client };
        }

        private static CommandExecutionContext InitExecutionContext(ParsedCommandLine cmdLine)
        {
            CommandExecutionContext result = null;

            String baseUri = cmdLine.Options.ContainsKey(Constants.BASE_URI) && cmdLine.Options[Constants.BASE_URI].Count == 1 ? cmdLine.Options[Constants.BASE_URI][0] : Environment.GetEnvironmentVariable(Constants.ONSHAPE_BASE_URI);
            String clientId = Environment.GetEnvironmentVariable(Constants.ONSHAPE_CLIENT_ID);
            String clientSecret = Environment.GetEnvironmentVariable(Constants.ONSHAPE_CLIENT_SECRET);
            String oauthToken = cmdLine.Options.ContainsKey(Constants.OAUTH_TOKEN) && cmdLine.Options[Constants.OAUTH_TOKEN].Count == 1 ? cmdLine.Options[Constants.OAUTH_TOKEN][0] : Environment.GetEnvironmentVariable(Constants.ONSHAPE_OAUTH_TOKEN);
            String oauthRefreshToken = cmdLine.Options.ContainsKey(Constants.OAUTH_REFRESH_TOKEN) && cmdLine.Options[Constants.OAUTH_REFRESH_TOKEN].Count == 1 ? cmdLine.Options[Constants.OAUTH_REFRESH_TOKEN][0] : Environment.GetEnvironmentVariable(Constants.ONSHAPE_OAUTH_REFRESH_TOKEN);
            Boolean interactiveMode = cmdLine.Options.ContainsKey(Constants.INTERACTIVE_MODE);

            if (BaseExecutionContext != null)
            {
                Boolean sameAsBaseContext = String.Equals(BaseExecutionContext.BaseURL, baseUri, StringComparison.InvariantCultureIgnoreCase);
                sameAsBaseContext = sameAsBaseContext && (oauthToken == null || String.Equals(BaseExecutionContext.OAuthToken, oauthToken, StringComparison.InvariantCultureIgnoreCase));
                sameAsBaseContext = sameAsBaseContext && (oauthRefreshToken == null || String.Equals(BaseExecutionContext.OAuthRefreshToken, oauthRefreshToken, StringComparison.InvariantCultureIgnoreCase));
                // Return base context if no changes
                result = sameAsBaseContext ? BaseExecutionContext : ConstructExecutionContext(baseUri, clientId, clientSecret, oauthToken, oauthRefreshToken, interactiveMode);
            }
            else
            {
                result = ConstructExecutionContext(baseUri, clientId, clientSecret, oauthToken, oauthRefreshToken, interactiveMode);
                BaseExecutionContext = result;
            }

            return result;
        }

        #endregion

        private static int exitCode = 0;

        private static async Task RunAsync(string[] args)
        {
            // Init console window size
            Console.WindowWidth = 98;

            // Parse command line
            ParsedCommandLine cmdLine = null;
            try
            {
                cmdLine = ParseCommandLine(args);
            }
            catch (CommandLineParseException e)
            {
                printUsage(e.Message, null, null);
                exitCode = 1;
            }

            if (cmdLine != null)
            {
                if (cmdLine.Command != null && String.Equals(Constants.HELP, cmdLine.Command.Name))
                {
                    printUsage(null, cmdLine.Options, cmdLine.Values);
                    return;
                }
                try
                {
                    // Initialize command execution context
                    BaseExecutionContext = InitExecutionContext(cmdLine);

                    // Execute command
                    if (cmdLine.Command != null)
                    {
                        await cmdLine.Command.Worker(BaseExecutionContext, cmdLine.Options, cmdLine.Values);
                    }

                    if (BaseExecutionContext.InteractiveMode)
                    {
                        CommandExecutionContext executionContext = BaseExecutionContext;
                        string pattern = @"\s*(""([^""])*""|'([^'])*'|([^""\s])*)";
                        do {
                            Console.Write(Constants.INTERACTIVE_PROMPT);
                            MatchCollection matches = Regex.Matches(Console.ReadLine(), pattern, RegexOptions.IgnorePatternWhitespace);
                            List<string> tokenList = new List<string>();
                            foreach (Match m in matches) {
                                String t = m.Value.Trim();
                                if (t.StartsWith("\"") || t.StartsWith("'") && t.Length > 1)
                                {
                                    t = t.Substring(1, t.Length - 2);
                                }
                                if (!String.IsNullOrEmpty(t))
                                {
                                    tokenList.Add(t);
                                }
                            }
                            string[] tokens = tokenList.ToArray();
                            if (tokens.Length > 0 && tokens[0].Length > 0)
                            {
                                try
                                {
                                    // Parse command
                                    cmdLine = ParseCommandLine(tokens);

                                    // Initialize execution context
                                    executionContext = InitExecutionContext(cmdLine);

                                    // Execute command
                                    await cmdLine.Command.Worker(BaseExecutionContext, cmdLine.Options, cmdLine.Values);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(String.Format("Error:\n\t{0}", e.Message));
                                }
                            }
                        } while (executionContext != null && executionContext.InteractiveMode);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(String.Format(@"Error: {0}", e.Message));
                    exitCode = 1;
                }
            }
        }

        static int Main(string[] args)
        {
            OptionValueCache = new Dictionary<String, List<String>>();
            RunAsync(args).Wait();
            return exitCode;
        }
    }
}
