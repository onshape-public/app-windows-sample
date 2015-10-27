using Onshape.Api.Client.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Onshape.Api.Client
{
    public class OnshapeClient
    {
        public String BaseUri { get; set; }
        public String AccessToken { get; set; }
        public String RefreshToken { get; set; }
        public String ClientId { get; set; }
        public String ClientSecret { get; set; }

        #region Utilities

        private static string constructGetDocumentsUri(Nullable<OnshapeDocumentFilterType> filter = null, String owner = null, Nullable<OnshapeDocumentOwnerType> ownerType = null, String sortColumn = null, Nullable<OnshapeSortOrder> sortOrder = OnshapeSortOrder.ASC, int offset = Constants.USE_API_DEFAULT, int limit = Constants.USE_API_DEFAULT)
        {
            return appendQueryString(Constants.DOCUMENTS_API_URI, constructGetDocumentsQueryString(filter, owner, ownerType, sortColumn, sortOrder, offset, limit));
        }

        private static string constructGetDocumentsQueryString(Nullable<OnshapeDocumentFilterType> filter = null, String owner = null, Nullable<OnshapeDocumentOwnerType> ownerType = null, String sortColumn = null, Nullable<OnshapeSortOrder> sortOrder = OnshapeSortOrder.ASC, int offset = Constants.USE_API_DEFAULT, int limit = Constants.USE_API_DEFAULT)
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

        private static string appendQueryString(string uri, string query)
        {
            string result = uri;
            if (!String.IsNullOrEmpty(query)){
                StringBuilder builder = new StringBuilder(uri);
                builder.Append("?");
                builder.Append(query.ToString());
                result =  builder.ToString();
            }
            return result;
        }

        private static string constructExportToParasolidQueryString(String version, string[] partIds = null)
        {
            StringBuilder queryString = new StringBuilder();
            if (!String.IsNullOrEmpty(version))
            {
                queryString.AppendQueryParam("version", version);
            }
            if (partIds != null && partIds.Length > 0)
            {
                queryString.AppendQueryParam("partIds", WebUtility.UrlEncode(String.Join(",", partIds)));
            }
            return queryString.ToString();
        }

        private static string constructExportToStlQueryString(OnshapeStlExportParameters parameters, string[] partIds = null)
        {
            StringBuilder queryString = new StringBuilder();
            if (parameters != null)
            {
                if (parameters.angleTolerance != null && parameters.angleTolerance.HasValue)
                {
                    queryString.AppendQueryParam("angleTolerance", parameters.angleTolerance.Value);
                }
                if (parameters.chordTolerance != null && parameters.chordTolerance.HasValue)
                {
                    queryString.AppendQueryParam("chordTolerance", parameters.chordTolerance.Value);
                }
                if (parameters.grouping != null && parameters.grouping.HasValue)
                {
                    queryString.AppendQueryParam("grouping", parameters.grouping.Value);
                }
                if (parameters.maxFacetWidth != null && parameters.maxFacetWidth.HasValue)
                {
                    queryString.AppendQueryParam("maxFacetWidth", parameters.maxFacetWidth.Value);
                }
                if (parameters.minFacetWidth != null && parameters.minFacetWidth.HasValue)
                {
                    queryString.AppendQueryParam("minFacetWidth", parameters.minFacetWidth.Value);
                }
                if (!String.IsNullOrEmpty(parameters.mode))
                {
                    queryString.AppendQueryParam("mode", parameters.mode);
                }
                if (parameters.scale != null && parameters.scale.HasValue)
                {
                    queryString.AppendQueryParam("scale", parameters.scale.Value);
                }
                if (!String.IsNullOrEmpty(parameters.units))
                {
                    queryString.AppendQueryParam("units", parameters.units);
                }
            }
            if (partIds != null && partIds.Length > 0)
            {
                queryString.AppendQueryParam("partIds", WebUtility.UrlEncode(String.Join(",", partIds)));
            }
            return queryString.ToString();
        }

        #endregion

        #region Http utilities

        private void InitDefaultHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
        }

        private class OnshapeRefreshTokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string refresh_token { get; set; }
            public int expires_in { get; set; }
            public string scope { get; set; }
        }

        private async void RefreshOAuthTokens(HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, String.Format(Constants.TOKEN_URI_TEMPLATE, BaseUri));
            request.Content = new FormUrlEncodedContent(new Dictionary<String, String>() {
                    {"grant_type", "refresh_token"},
                    {"refresh_token", RefreshToken},
                    {"client_id", ClientId},
                    {"client_secret", ClientSecret}
                });
            HttpResponseMessage response = null;
            try
            {
                using (response = await client.SendAsync(request))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        OnshapeRefreshTokenResponse refreshToken = await response.Content.ReadAsAsync<OnshapeRefreshTokenResponse>();
                        AccessToken = refreshToken.access_token;
                        RefreshToken = refreshToken.refresh_token;
                        InitDefaultHeaders(client);
                    }
                }
            }
            catch (Exception e)
            {
                throw new OnshapeClientException("Authorization token refresh failed", e);
            }
        }

        private class RedirectMessageHandler : DelegatingHandler
        {
            public RedirectMessageHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var tcs = new TaskCompletionSource<HttpResponseMessage>();
                base.SendAsync(request, cancellationToken)
                    .ContinueWith(t =>
                    {
                        HttpResponseMessage response;
                        try
                        {
                            response = t.Result;
                        }
                        catch (Exception e)
                        {
                            response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                            response.ReasonPhrase = e.Message;
                        }
                        if (response.StatusCode == HttpStatusCode.MovedPermanently
                            || response.StatusCode == HttpStatusCode.Moved
                            || response.StatusCode == HttpStatusCode.Redirect
                            || response.StatusCode == HttpStatusCode.Found
                            || response.StatusCode == HttpStatusCode.SeeOther
                            || response.StatusCode == HttpStatusCode.RedirectKeepVerb
                            || response.StatusCode == HttpStatusCode.TemporaryRedirect
                            || (int)response.StatusCode == 308)
                        {
                            var newRequest = new HttpRequestMessage(response.RequestMessage.Method, response.RequestMessage.RequestUri);
                            foreach (var header in response.RequestMessage.Headers)
                            {
                                newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                            }
                            foreach (var property in response.RequestMessage.Properties)
                            {
                                newRequest.Properties.Add(property);
                            }
                            if (response.RequestMessage.Content != null)
                            {
                                newRequest.Content = new StreamContent(response.RequestMessage.Content.ReadAsStreamAsync().Result);
                            }
                            if (response.StatusCode == HttpStatusCode.Redirect
                                || response.StatusCode == HttpStatusCode.Found
                                || response.StatusCode == HttpStatusCode.SeeOther)
                            {
                                newRequest.Content = null;
                                newRequest.Method = HttpMethod.Get;
                            }
                            newRequest.RequestUri = response.Headers.Location;
                            base.SendAsync(newRequest, cancellationToken)
                                .ContinueWith(t2 => tcs.SetResult(t2.Result));
                        }
                        else
                        {
                            tcs.SetResult(response);
                        }
                    });
                return tcs.Task;
            }
        }

        private HttpClient ConstructHttpClient(Boolean initDefaultHeaders = true)
        {
            var handler = new RedirectMessageHandler(new HttpClientHandler() { AllowAutoRedirect = false });
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri(BaseUri);
            if (initDefaultHeaders)
            {
                InitDefaultHeaders(client);
            }
            return client;
        }

        private async Task<T> doWithTokenRefresh<T> (HttpClient client, Func<Task<T>> operation) where T : HttpResponseMessage {
            var response = await operation();
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Refresh token and retry
                RefreshOAuthTokens(client);
                response = await operation();
            }
            return response;
        }

        public async Task<string> GetRefreshedOAuthToken()
        {
            string result = null;
            using (HttpClient client = ConstructHttpClient(false))
            {
                var request = new HttpRequestMessage(HttpMethod.Post, String.Format(Constants.TOKEN_URI_TEMPLATE, BaseUri));
                request.Content = new FormUrlEncodedContent(new Dictionary<String, String>() {
                    {"grant_type", "refresh_token"},
                    {"refresh_token", RefreshToken},
                    {"client_id", ClientId},
                    {"client_secret", ClientSecret}
                });
                HttpResponseMessage response = null;
                try
                {
                    using (response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            OnshapeRefreshTokenResponse refreshToken = await response.Content.ReadAsAsync<OnshapeRefreshTokenResponse>();
                            result = refreshToken.access_token;
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new OnshapeClientException("Authorization token refresh failed", e);
                }
            }
            return result;
        }

        public async Task<HttpResponseMessage> HttpGet(String uri)
        {
            using (var client = ConstructHttpClient())
            {
                return await doWithTokenRefresh(client, ()=>client.GetAsync(uri));
            }
        }

        public async Task<T> HttpGet<T>(String uri)
        {
            using (var client = ConstructHttpClient())
            {
                HttpResponseMessage response = await doWithTokenRefresh(client, ()=>client.GetAsync(uri));
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
                return await doWithTokenRefresh(client, () => client.PostAsJsonAsync(uri, value));
            }
        }

        public async Task<HttpResponseMessage> HttpPostMultipartFormData(String uri, Dictionary<string, string> fields, string fileName, byte[] fileData = null)
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
                    var fileContent = new ByteArrayContent(fileData == null ? System.IO.File.ReadAllBytes(fileName) : fileData);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { FileName = System.IO.Path.GetFileName(fileName), Name = "file" };
                    content.Add(fileContent);
                    HttpResponseMessage response = await doWithTokenRefresh(client, ()=>client.PostAsync(uri, content));
                    return response;
                }
            }
        }

        public async Task<T> HttpPost<T>(String uri, T value)
        {
            using (var client = ConstructHttpClient())
            {
                HttpResponseMessage response = await doWithTokenRefresh(client, ()=>client.PostAsJsonAsync(uri, value));
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>();
                }
                throw new Exception(response.ToString());
            }
        }

        public async Task<R> HttpPost<T, R>(String uri, T value)
        {
            using (var client = ConstructHttpClient())
            {
                HttpResponseMessage response = await doWithTokenRefresh(client, () => client.PostAsJsonAsync(uri, value));
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<R>();
                }
                throw new Exception(response.ToString());
            }
        }

        public async Task<T> HttpPost<T>(String uri)
        {
            using (var client = ConstructHttpClient())
            {
                HttpResponseMessage response = await doWithTokenRefresh(client, ()=>client.PostAsync(uri, null));
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
                HttpResponseMessage response = await doWithTokenRefresh(client, ()=>client.DeleteAsync(uri));
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

        public async Task<Stream> ExportPartstudioToStl(String documentId, String wvmSelector, String selectorId, String elementId, OnshapeStlExportParameters paramters, string[] partIds)
        {
            Stream result = null;
            var response = await HttpGet(appendQueryString(String.Format(Constants.EXPORT_PARTSTUDIO_API_URI, documentId, wvmSelector, selectorId, elementId, Constants.STL_FORMAT_NAME),
                constructExportToStlQueryString(paramters, partIds)));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsStreamAsync();
            }
            return result;
        }
        public async Task<Stream> ExportPartstudioToParasolid(String documentId, String wvmSelector, String selectorId, String elementId, String formatVersion, string[] partIds)
        {
            Stream result = null;
            var response = await HttpGet(appendQueryString(String.Format(Constants.EXPORT_PARTSTUDIO_API_URI, documentId, wvmSelector, selectorId, elementId, Constants.PARASOLID_FORMAT_NAME),
                constructExportToParasolidQueryString(formatVersion, partIds)));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsStreamAsync();
            }
            return result;
        }
        public async Task<List<OnshapeTranslationFormat>> GetPartstudioTranslationFormats(String documentId, String workspaceId, String elementId)
        {
            List<OnshapeTranslationFormat> result = null;
            var response = await HttpGet(String.Format(Constants.ELEMENT_TRANSLATION_FORMATS_API_URI, Constants.PARTSTUDIOS_PATH_NAME, documentId, workspaceId, elementId));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsAsync<List<OnshapeTranslationFormat>>();
            }
            return result;
        }
        public async Task<OnshapeTranslationStatus> CreatePartstudioTranslation(String documentId, String workspaceId, String elementId, OnshapePartstudioTranslationParameters parameters)
        {
            return await HttpPost<OnshapeTranslationParameters, OnshapeTranslationStatus>(String.Format(Constants.ELEMENT_TRANSLATIONS_API_URI, Constants.PARTSTUDIOS_PATH_NAME, documentId, workspaceId, elementId), parameters);
        }

        #endregion

        #region Assemblies

        public async Task<List<OnshapeTranslationFormat>> GetAssemblyTranslationFormats(String documentId, String workspaceId, String elementId)
        {
            List<OnshapeTranslationFormat> result = null;
            var response = await HttpGet(String.Format(Constants.ELEMENT_TRANSLATION_FORMATS_API_URI, Constants.ASSEMBLIES_PATH_NAME, documentId, workspaceId, elementId));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsAsync<List<OnshapeTranslationFormat>>();
            }
            return result;
        }
        public async Task<OnshapeTranslationStatus> CreateAssemblyTranslation(String documentId, String workspaceId, String elementId, OnshapeTranslationParameters parameters)
        {
            return await HttpPost<OnshapeTranslationParameters, OnshapeTranslationStatus>(String.Format(Constants.ELEMENT_TRANSLATIONS_API_URI, Constants.ASSEMBLIES_PATH_NAME, documentId, workspaceId, elementId), parameters);
        }

        #endregion

        #region Parts

        public async Task<Stream> ExportPartToStl(String documentId, String wvmSelector, String selectorId, String elementId, String partId, OnshapeStlExportParameters parameters)
        {
            Stream result = null;
            var response = await HttpGet(appendQueryString(String.Format(Constants.EXPORT_PART_API_URI, documentId, wvmSelector, selectorId, elementId, partId, Constants.STL_FORMAT_NAME),
                constructExportToStlQueryString(parameters)));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsStreamAsync();
            }
            return result;
        }

        public async Task<Stream> ExportPartToParasolid(String documentId, String wvmSelector, String selectorId, String elementId, String partId, string formatVersion)
        {
            Stream result = null;
            var response = await HttpGet(appendQueryString(String.Format(Constants.EXPORT_PART_API_URI, documentId, wvmSelector, selectorId, elementId, partId, Constants.PARASOLID_FORMAT_NAME),
                constructExportToParasolidQueryString(formatVersion)));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsStreamAsync();
            }
            return result;
        }

        #endregion

        #region Blobelementds

        public async Task<Stream> DownloadBlobelement(String documentId, String wvmSelector, String selectorId, String elementId)
        {
            Stream result = null;
            var response = await HttpGet(String.Format(Constants.BLOB_ELEMENT_API_URI, documentId, wvmSelector, selectorId, elementId));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsStreamAsync();
            }
            return result;
        }
        public async Task<OnshapeElementTranslation> CreateBlobelement(String documentId, String workspaceId, Dictionary<String, String> fields, String fileName, byte[] fileData = null)
        {
            OnshapeElementTranslation result = null;
            var response = await HttpPostMultipartFormData(String.Format(Constants.BLOB_ELEMENTS_API_URI, documentId, "w", workspaceId), fields, fileName, fileData);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsAsync<OnshapeElementTranslation>();
            }
            return result;
        }
        public async Task<OnshapeElementTranslation> UpdateBlobelement(String documentId, String workspaceId, String elementId, Dictionary<String, String> fields, String fileName, byte[] fileData = null)
        {
            OnshapeElementTranslation result = null;
            var response = await HttpPostMultipartFormData(String.Format(Constants.BLOB_ELEMENT_API_URI, documentId, "w", workspaceId, elementId), fields, fileName, fileData);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsAsync<OnshapeElementTranslation>();
            }
            return result;
        }
        public async Task<List<OnshapeTranslationFormat>> GetBlobelementTranslationFormats(String documentId, String workspaceId, String elementId)
        {
            List<OnshapeTranslationFormat> result = null;
            var response = await HttpGet(String.Format(Constants.ELEMENT_TRANSLATION_FORMATS_API_URI, Constants.BLOBELEMENTS_PATH_NAME, documentId, workspaceId, elementId));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsAsync<List<OnshapeTranslationFormat>>();
            }
            return result;
        }
        public async Task<OnshapeTranslationStatus> CreateBlobelementTranslation(String documentId, String workspaceId, String elementId, OnshapeBlobTranslationParameters parameters)
        {
            return await HttpPost<OnshapeTranslationParameters, OnshapeTranslationStatus>(String.Format(Constants.ELEMENT_TRANSLATIONS_API_URI, Constants.BLOBELEMENTS_PATH_NAME, documentId, workspaceId, elementId), parameters);
        }

        #endregion

        #region Translations

        public async Task<OnshapeTranslationStatus> CreateTranslation(String documentId, String workspaceId, Dictionary<string, string> fields, String fileName, byte[] fileData = null)
        {
            OnshapeTranslationStatus result = null;
            var response = await HttpPostMultipartFormData(String.Format(Constants.CREATE_TRANSLATION_API_URI, documentId, workspaceId), fields, fileName, fileData);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsAsync<OnshapeTranslationStatus>();
            }
            return result;
        }
        public async Task<OnshapeTranslationStatus> GetTranslationStatus(String translationId)
        {
            OnshapeTranslationStatus result = null;
            var response = await HttpGet(String.Format(Constants.TRANSLATION_API_URI, translationId));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsAsync<OnshapeTranslationStatus>();
            }
            return result;
        }
        public async Task<OnshapeTranslationStatusList> GetDocumentTranslationStatus(String documentId)
        {
            OnshapeTranslationStatusList result = null;
            var response = await HttpGet(String.Format(Constants.DOCUMENT_TRANSLATIONS_API_URI, documentId));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsAsync<OnshapeTranslationStatusList>();
            }
            return result;
        }
        public async Task DeleteTranslationStatusEntry(String translationId)
        {
            await HttpDelete(String.Format(Constants.TRANSLATION_API_URI, translationId));
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
