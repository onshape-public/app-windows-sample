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
 
        public const string DOCUMENTS_API_URI = @"/api/documents";
        public const string DOCUMENT_API_URI = @"/api/documents/{0}";

        public const string WORKSPACES_API_URI = @"/api/documents/d/{0}/workspaces";
        public const string WORKSPACE_API_URI = @"/api/documents/d/{0}/workspaces/{1}";

        public const string VERSIONS_API_URI = @"/api/documents/d/{0}/versions";
        public const string VERSION_API_URI = @"/api/documents/d/{0}/versions/{1}";

        public const string ELEMENTS_API_URI = @"/api/documents/d/{0}/{1}/{2}/elements";
        public const string ELEMENT_API_URI = @"/api/documents/d/{0}/{1}/{2}/elements?elementId={3}";

        public const string USER_API_URI = @"/api/users/{0}";

        public const string PURCHASE_API_URI = @"/api/accounts/purchasses/{0}";
        public const string CONSUME_PURCHASE_API_URI = @"/api/accounts/purchasses/{0}/consume";
        public const string BILLING_PLAN_API_URI = @"/api/billing/plans/{0}";
        public const string CLIENT_BILLING_PLANS_API_URI = @"/api/billing/plans/client/{0}";

        #endregion

        #region Misc constants

        public const int USE_API_DEFAULT = -1;

        #endregion
    }
}
