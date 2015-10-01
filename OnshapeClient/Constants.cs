using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client
{
    internal static class Constants
    {
        #region Onshape REST API URIs
 
        public const String DOCUMENTS_API_URI = @"/api/documents";
        public const String DOCUMENT_API_URI = @"/api/documents/{0}";

        public const String WORKSPACES_API_URI = @"/api/documents/d/{0}/workspaces";
        public const String WORKSPACE_API_URI = @"/api/documents/d/{0}/workspaces/{1}";

        public const String VERSIONS_API_URI = @"/api/documents/d/{0}/versions";
        public const String VERSION_API_URI = @"/api/documents/d/{0}/versions/{1}";

        public const String ELEMENTS_API_URI = @"/api/documents/d/{0}/{1}/{2}/elements";
        public const String ELEMENT_API_URI = @"/api/documents/d/{0}/{1}/{2}/elements?elementId={3}";

        public const String USER_API_URI = @"/api/users/{0}";

        #endregion

        #region Misc constants

        public const int USE_API_DEFAULT = -1;

        #endregion
    }
}
