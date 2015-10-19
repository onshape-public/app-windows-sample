using Onshape.Api.Client.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client
{
    public class OnshapeClient
    {
        public String BaseUri { get; set; }
        public String AccessToken { get; set; }
        public String RefreshToken { get; set; }
        public String ClientId { get; set; }
        public Action<HttpResponseMessage> OnResponse { get; set; }

        #region Utilities

        private String constructGetDocumentsUri(Nullable<OnshapeDocumentFilterType> filter = null, String owner = null, Nullable<OnshapeDocumentOwnerType> ownerType = null, String sortColumn = null, Nullable<OnshapeSortOrder> sortOrder = OnshapeSortOrder.ASC, int offset = Constants.USE_API_DEFAULT, int limit = Constants.USE_API_DEFAULT)
        {
            StringBuilder uri = new StringBuilder(Constants.DOCUMENTS_API_URI);
            String queryString = constructGetDocumentsQueryString(filter, owner, ownerType, sortColumn, sortOrder, offset, limit);
            if (queryString != null && queryString.Length > 0)
            {
                uri.Append("?");
                uri.Append(queryString.ToString());
            }
            return uri.ToString();
        }

        private String constructGetDocumentsQueryString(Nullable<OnshapeDocumentFilterType> filter = null, String owner = null, Nullable<OnshapeDocumentOwnerType> ownerType = null, String sortColumn = null, Nullable<OnshapeSortOrder> sortOrder = OnshapeSortOrder.ASC, int offset = Constants.USE_API_DEFAULT, int limit = Constants.USE_API_DEFAULT)
        {
            StringBuilder queryString = new StringBuilder();
            if (offset != Constants.USE_API_DEFAULT)
            {
                queryString.AppendQueryParam("offset", offset);
            }
            if (limit != Constants.USE_API_DEFAULT)
            {
                queryString.AppendQueryParam("limit", limit);
            }
            if (filter != null)
            {
                queryString.AppendQueryParam("filter", (int)filter.Value);
            }
            if (owner != null)
            {
                queryString.AppendQueryParam("owner", owner);
            }
            if (ownerType != null)
            {
                queryString.AppendQueryParam("ownerType", (int)ownerType.Value);
            }
            if (sortColumn != null)
            {
                queryString.AppendQueryParam("sortColumn", sortColumn);
            }
            if (sortOrder != null)
            {
                queryString.AppendQueryParam("sortOrder", sortOrder.Value == OnshapeSortOrder.ASC ? "asc" : "desc" );
            }
            return queryString.ToString();
        }

        #endregion

        #region Http utilities

        private HttpClient ConstructHttpClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(BaseUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
            return client;
        }

        public async Task<HttpResponseMessage> HttpGet(String uri)
        {
            using (var client = ConstructHttpClient())
            {
                return await client.GetAsync(uri);
            }
        }

        public async Task<T> HttpGet<T>(String uri)
        {
            using (var client = ConstructHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (OnResponse != null)
                {
                    OnResponse(response);
                }
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>();
                }
                throw new Exception(response.ToString());
            }
        }

        public async Task<HttpResponseMessage> HttpPostJson(String uri, String value)
        {
            using (var client = ConstructHttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(uri, value);
                if (OnResponse != null)
                {
                    OnResponse(response);
                }
                return response;
            }
        }

        public async Task<HttpResponseMessage> HttpPostMultipartFormData(String uri, Dictionary<string, string> fields, string fileName)
        {
            using (var client = ConstructHttpClient())
            {
                using(var content = new MultipartFormDataContent ()) 
                {
                    if (fields != null)
                    {
                        foreach (var v in fields)
                        {
                            content.Add(new StringContent(v.Value), v.Key);
                        }
                    }
                    var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(fileName));
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue ("form-data") { FileName = System.IO.Path.GetFileName(fileName) };
                    content.Add(fileContent);
                    HttpResponseMessage response = await client.PostAsync(uri, content);
                    if (OnResponse != null)
                    {
                        OnResponse(response);
                    }
                    return response;
                }
            }
        }

        public async Task<T> HttpPost<T>(String uri, T value)
        {
            using (var client = ConstructHttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(uri, value);
                if (OnResponse != null)
                {
                    OnResponse(response);
                }
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>();
                }
                throw new Exception(response.ToString());
            }
        }

        public async Task<T> HttpPost<T>(String uri)
        {
            using (var client = ConstructHttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(uri, null);
                if (OnResponse != null)
                {
                    OnResponse(response);
                }
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>();
                }
                throw new Exception(response.ToString());
            }
        }

        public async Task<HttpResponseMessage> HttpDelete(String uri)
        {
            using (var client = ConstructHttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync(uri);
                if (OnResponse != null)
                {
                    OnResponse(response);
                }
                return response;
            }
        }

        #endregion

        #region Onshape REST API

        #region Document

        public async Task<List<OnshapeDocument>> GetDocuments(int offset = Constants.USE_API_DEFAULT, int limit = Constants.USE_API_DEFAULT)
        {
            String uri = constructGetDocumentsUri(null, null, null, null, null, offset, limit);
            OnshapeDocuments documents = await HttpGet<OnshapeDocuments>(uri);
            return documents.items;
        }
        public async Task<List<OnshapeDocument>> GetDocuments(OnshapeDocumentFilterType filter, String sortColumn = null, OnshapeSortOrder sortOrder = OnshapeSortOrder.ASC, int offset = Constants.USE_API_DEFAULT, int limit = Constants.USE_API_DEFAULT)
        {
            String uri = constructGetDocumentsUri(filter, null, null, sortColumn, sortOrder, offset, limit);
            OnshapeDocuments documents = await HttpGet<OnshapeDocuments>(uri);
            return documents.items;
        }
        public async Task<List<OnshapeDocument>> GetDocuments(OnshapeDocumentFilterType filter, String owner, Nullable<OnshapeDocumentOwnerType> ownerType = null, String sortColumn = null, Nullable<OnshapeSortOrder> sortOrder = null, int offset = Constants.USE_API_DEFAULT, int limit = Constants.USE_API_DEFAULT)
        {
            String uri = constructGetDocumentsUri(filter, owner, ownerType, sortColumn, sortOrder, offset, limit);
            OnshapeDocuments documents = await HttpGet<OnshapeDocuments>(uri);
            return documents.items;
        }
        public async Task<OnshapeDocument> GetDocument(String documentId)
        {
            return await HttpGet<OnshapeDocument>(String.Format(Constants.DOCUMENT_API_URI, documentId));
        }
        public async Task<OnshapeDocument> CreateDocument(String name)
        {
            return await HttpPost<OnshapeDocument>(Constants.DOCUMENTS_API_URI, new OnshapeDocument
            {
                name = name
            });
        }
        public async Task DeleteDocument(String documentId)
        {
            await HttpDelete(String.Format(Constants.DOCUMENT_API_URI, documentId));
        }

        #endregion

        #region Version

        public async Task<List<OnshapeVersion>> GetVersions(String documentId) 
        {
            return await HttpGet<List<OnshapeVersion>>(String.Format(Constants.VERSIONS_API_URI, documentId));
        }
        public async Task<OnshapeVersion> GetVersion(String documentId, String versionId) 
        {
            return await HttpGet<OnshapeVersion>(String.Format(Constants.VERSION_API_URI, documentId, versionId)); 
        }
        public async Task DeleteVersion(String documentId, String versionId)
        {
            await HttpDelete(String.Format(Constants.VERSION_API_URI, documentId, versionId));
        }

        #endregion

        #region Workspace

        public async Task<List<OnshapeWorkspace>> GetWorkspaces(String documentId) 
        {
            return await HttpGet<List<OnshapeWorkspace>>(String.Format(Constants.WORKSPACES_API_URI, documentId));
        }
        public async Task<OnshapeWorkspace> GetWorkspace(String documentId, String workspaceId) 
        {
            return await HttpGet<OnshapeWorkspace>(String.Format(Constants.WORKSPACE_API_URI, documentId, workspaceId));
        }
        public async Task DeleteWorkspace(String documentId, String workspaceId)
        {
            await HttpDelete(String.Format(Constants.WORKSPACE_API_URI, documentId, workspaceId));
        }

        #endregion

        #region User

        public async Task<OnshapeUser> GetUser(String userId) 
        {
            return await HttpGet<OnshapeUser>(String.Format(Constants.USER_API_URI, userId));
        }

        #endregion

        #region Element

        public async Task<List<OnshapeElement>> GetWorkspaceElements(String documentId, String workspaceId) 
        {
            return await HttpGet<List<OnshapeElement>>(String.Format(Constants.ELEMENTS_API_URI, documentId, "w", workspaceId));
        }
        public async Task<List<OnshapeElement>> GetVersionElements(String documentId, String versionId) 
        { 
            return await HttpGet<List<OnshapeElement>>(String.Format(Constants.ELEMENTS_API_URI, documentId, "v", versionId));
        }
        public async Task<OnshapeElement> GetWorkspaceElement(String documentId, String workspaceId, String elementId) 
        {
            var elements = await HttpGet<List<OnshapeElement>>(String.Format(Constants.ELEMENT_API_URI, documentId, "w", workspaceId, elementId));
            return elements != null && elements.Count == 1 ? elements[0] : null;
        }
        public async Task<OnshapeElement> GetVersionElement(String documentId, String versionId, String elementId) 
        {
            var elements = await HttpGet<List<OnshapeElement>>(String.Format(Constants.ELEMENT_API_URI, documentId, "v", versionId, elementId));
            return elements != null && elements.Count == 1 ? elements[0] : null;
        }
        public async Task<OnshapeElement> UpdateWorkspaceElement(String documentId, String workspaceId, OnshapeElement value) 
        {
            return await HttpPost<OnshapeElement>(String.Format(Constants.ELEMENT_API_URI, documentId, "w", workspaceId, value.id), value); 
        }
        public async Task<OnshapeElement> UpdateVersionElement(String documentId, String versionId, OnshapeElement value) 
        {
            return await HttpPost<OnshapeElement>(String.Format(Constants.ELEMENT_API_URI, documentId, "v", versionId, value.id), value);
        }

        #endregion

        #region Partstudios

        public async Task<Stream> DownloadPartstudio(String documentId, String wmvSelector, String selectorId, String elementId, String format)
        {
            Stream result = null;
            var response = await HttpGet(String.Format(Constants.DOWNLOAD_PARTSTUDIO_API_URI, documentId, wmvSelector, selectorId, elementId, format));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                //TODO: Replace this with a generic retry logic
                response = await HttpGet(response.RequestMessage.RequestUri.ToString());
            }
            if (OnResponse != null)
            {
                OnResponse(response);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsStreamAsync();
            }
            return result;
        }

        #endregion

        #region Billing Account

        public async Task<List<OnshapePurchase>> GetPurchases()
        {
            return await HttpGet<List<OnshapePurchase>>(String.Format(Constants.PURCHASES_API_URI));
        }
        public async Task<OnshapePurchase> ConsumePurchase(String purchaseId)
        {
            return await HttpPost<OnshapePurchase>(String.Format(Constants.CONSUME_PURCHASE_API_URI, purchaseId));
        }
        public async Task CancelPurchase(String purchaseId)
        {
            await HttpDelete(String.Format(Constants.PURCHASE_API_URI, purchaseId));
        }
        
        #endregion

        #region Billing Plan

        public async Task<List<OnshapeBillingPlan>> GetClientBillingPlans()
        {
            return await HttpGet<List<OnshapeBillingPlan>>(String.Format(Constants.CLIENT_BILLING_PLANS_API_URI, ClientId));
        }
        public async Task<OnshapeBillingPlan> GetBillingPlan(String planId)
        {
            return await HttpGet<OnshapeBillingPlan>(String.Format(Constants.BILLING_PLAN_API_URI, planId));
        }

        #endregion

        #endregion
    }
}
