using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.ConsoleApp
{
    internal static class Constants
    {
        #region Misc constants

        internal const string DEFAULT_LOGING_LEVEL = "Info";
        internal const string INTERACTIVE_PROMPT = "> ";

        #endregion

        #region Configuration environment variables

        internal const string ONSHAPE_OAUTH_REFRESH_TOKEN = @"ONSHAPE_OAUTH_REFRESH_TOKEN";
        internal const string ONSHAPE_CLIENT_SECRET = @"ONSHAPE_CLIENT_SECRET";
        internal const string ONSHAPE_OAUTH_TOKEN = @"ONSHAPE_OAUTH_TOKEN";
        internal const string ONSHAPE_CLIENT_ID = @"ONSHAPE_CLIENT_ID";
        internal const string ONSHAPE_BASE_URI = @"ONSHAPE_BASE_URI";

        #endregion

        #region Tokens

        internal const string UPLOAD = @"UPLOAD";
        internal const string HELP = @"HELP";
        internal const string GET = @"GET";
        internal const string POST = @"POST";
        internal const string DELETE = @"DELETE";
        internal const string DOCUMENT_ID = @"DOCUMENT_ID";
        internal const string WORKSPACE_ID = @"WORKSPACE_ID";
        internal const string VERSION_ID = @"VERSION_ID";
        internal const string USER_ID = @"USER_ID";
        internal const string ELEMENT_ID = @"ELEMENT_ID";
        internal const string VALUE = @"VALUE";
        internal const string DOCUMENTS = @"DOCUMENTS";
        internal const string WORKSPACES = @"WORKSPACES";
        internal const string VERSIONS = @"VERSIONS";
        internal const string USERS = @"USERS";
        internal const string ELEMENTS = @"ELEMENTS";
        internal const string FILE = @"FILE";
        internal const string BASE_URI = @"BASE_URI";
        internal const string OAUTH_TOKEN = @"TOKEN";
        internal const string OAUTH_REFRESH_TOKEN = @"REFRESH_TOKEN";
        internal const string LOGING_LEVEL = @"LOGING_LEVEL";
        internal const string INTERACTIVE_MODE = @"INTERACTIVE_MODE";
        internal const string EXIT = @"EXIT";

        #endregion

        #region Command line parser token maps

        internal static Dictionary<string, string> commandTokens = new Dictionary<string, string> {
            {@"HELP", HELP},
            {@"?", HELP},
            {@"/?", HELP},
            {@"--HELP", HELP},
            {@"-H", HELP},
            {@"EXIT", EXIT},
            {@"Q", EXIT},
            {@"UPLOAD", UPLOAD},
            {@"GET", GET},
            {@"POST", POST},
            {@"DELETE", DELETE},
            {@"DOCUMENTS", DOCUMENTS},
            {@"WORKSPACES", WORKSPACES},
            {@"VERSIONS", VERSIONS},
            {@"USERS", USERS},
            {@"ELEMENTS", ELEMENTS}
        };

        internal static Dictionary<string, string> optionTokens = new Dictionary<string, string> {
            {@"--documentId", DOCUMENT_ID},
            {@"-d", DOCUMENT_ID},
            {@"--workspaceId", WORKSPACE_ID},
            {@"-w", WORKSPACE_ID},
            {@"--versionId", VERSION_ID},
            {@"-v", VERSION_ID},
            {@"--elementId", ELEMENT_ID},
            {@"-e", ELEMENT_ID},
            {@"--userId", USER_ID},
            {@"-u", USER_ID},
            {@"--baseUri", BASE_URI},
            {@"-b", BASE_URI},
            {@"--token", OAUTH_TOKEN},
            {@"-t", OAUTH_TOKEN},
            {@"-r", OAUTH_REFRESH_TOKEN},
            {@"-i", INTERACTIVE_MODE},
            {@"-l", LOGING_LEVEL},
            {@"-f", FILE},
        };

        internal static List<CommandOption> globalOptions = new List<CommandOption>
        {
            new CommandOption {Reqiuired = false, Token = Constants.BASE_URI, MinArgs = 1, MaxArgs = 1},
            new CommandOption {Reqiuired = false, Token = Constants.OAUTH_TOKEN, MinArgs = 1, MaxArgs = 1},
            new CommandOption {Reqiuired = false, Token = Constants.OAUTH_REFRESH_TOKEN, MinArgs = 1, MaxArgs = 1},
            new CommandOption {Reqiuired = false, Token = Constants.INTERACTIVE_MODE, MinArgs = 0, MaxArgs = 0}
        };

        #endregion
    }
}
