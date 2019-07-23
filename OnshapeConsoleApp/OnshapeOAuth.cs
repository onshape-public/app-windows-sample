using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Web.Script.Serialization;

namespace OnshapeAPI
{
   public enum OnshapeOAuthStatus {Authenticated, Error, Initialized, IsAuthenticating};

   public class JSONTokenResponse
   {
       public string access_token { get; set; }
       public string token_type { get; set; }
       public string refresh_token { get; set; }
       public int expires_in { get; set; }
       public string scope { get; set; }
   }

   class OnshapeOAuth
   {
       #region OAuth Constants

       private const string AUTH_URI_TEMPLATE = @"{0}/oauth/authorize?response_type=code&client_id={1}&grant_type=authorization_code&redirect_uri={2}";
       private const string AUTH_FORM_TEMPLATE = "code={0}&client_id={1}&client_secret={2}&grant_type=authorization_code&redirect_uri={3}";
       private const string TOKEN_URI_TEMPLATE = @"{0}/oauth/token";
       private const string BROWSER_URI = @"urn:ietf:wg:oauth:2.0:oob";
       private const string SERVER_URI = @"http://localhost:9326";

       #endregion

       #region Variables

       private String clientSecret;
       private String clientId;
       private String baseUri;

       private string strAuthCode;
       private string strAccToken;
       private string strRefToken;
       private Action<String> logWriter;
       private OnshapeOAuthStatus ooasStatus;
       private bool IsLogging;
       private Thread tWorker;
       private object oLocker;

       #endregion

       #region Properties

       private string AuthPath { get { return String.Format(AUTH_URI_TEMPLATE, baseUri, HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(SERVER_URI)); } }

       private string FormAuthReq { get { return String.Format(AUTH_FORM_TEMPLATE, HttpUtility.UrlEncode(AuthCode), HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret), HttpUtility.UrlEncode(SERVER_URI)); } }

       public string AuthCode
       {
           get { lock (oLocker) { return strAuthCode; } }
           private set { lock (oLocker) { strAuthCode = value; } }
       }

       public string AccessToken
       {
           get { lock (oLocker) { return strAccToken; } }
           private set { lock (oLocker) { strAccToken = value; } }
       }

       public string RefreshToken
       {
           get { lock (oLocker) { return strRefToken; } }
           private set { lock (oLocker) { strRefToken = value; } }
       }

       public OnshapeOAuthStatus Status
       {
           get { lock(oLocker) { return ooasStatus; } }
           private set {lock(oLocker) { ooasStatus = value; } }
       }

       #endregion



       #region Constructors

       public OnshapeOAuth(String baseUri, String clientId, String clientSecret, Action<string> logMethod = null)
       {
           IsLogging = logMethod != null;
           logWriter = logMethod;
           this.baseUri = baseUri;
           this.clientId = clientId;
           this.clientSecret = clientSecret;

           oLocker = new object();
           AuthCode = "";
           AccessToken = "";
           RefreshToken = "";
           Status = OnshapeOAuthStatus.Initialized;
       }

       #endregion

       #region Public Methods

       public void AuthenticateThreading()
       {
           tWorker = new Thread(new ThreadStart(DoAuthentication));
           
           tWorker.Start();

       }

       public void AuthenticateBlocking()
       {
           DoAuthentication();

       }

       public void Abort()
       {
           if (tWorker != null)
           {
               AddLogEntry("Aborting thread");
               tWorker.Abort();
           }
           else
               AddLogEntry("No active thread to abort");
       }

       #endregion


       #region Private Methods

       private void DoAuthentication()
       {
           Status = OnshapeOAuthStatus.IsAuthenticating;
           Process pWeb = new Process();
           pWeb.StartInfo.FileName = AuthPath;
           pWeb.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
           pWeb.Start();
           AddLogEntry("Browser opened...");
           Listen();
           pWeb.Close();
           GetTokens();
           if (Status != OnshapeOAuthStatus.Error)
           {
               Status = OnshapeOAuthStatus.Authenticated;

               AddLogEntry("Authentication completed...");
           }
       }

       //Listener for callback
       private void Listen()
       {
           try
           {
               AddLogEntry("Starting listener...");
               HttpListener hlListener = new HttpListener();
               hlListener.Prefixes.Add(SERVER_URI + "/");
               hlListener.Start();

               AddLogEntry("Listener started...");
               HttpListenerContext hlcContext = hlListener.GetContext();

               AddLogEntry("Listener received request...sending response...");
               HttpListenerRequest hlrqRequest = hlcContext.Request;
               HttpListenerResponse hlrpResponse = hlcContext.Response;
               string strResponse = "<HTML><HEAD><TITLE>Authentication Successful</TITLE></HEAD><BODY>Please close the window.</BODY></HTML>";//This is what people will see after authenticating
               byte[] bBuffer = System.Text.Encoding.UTF8.GetBytes(strResponse);
               // Get a response stream and write the response to it.
               hlrpResponse.ContentLength64 = bBuffer.Length;
               System.IO.Stream smOutput = hlrpResponse.OutputStream;
               smOutput.Write(bBuffer, 0, bBuffer.Length);

               AddLogEntry("Response sent...");
               // You must close the output stream.
               smOutput.Close();
               hlListener.Stop();
               AddLogEntry("Listener closed...");

               AuthCode = hlrqRequest.QueryString["code"];

               AddLogEntry("Authentication code: " + AuthCode);
           }
           catch(Exception e)
           {
               AddLogEntry("Error occured in Listen: " + e.ToString());
               Status = OnshapeOAuthStatus.Error;

           }

       }

       //After getting callback request token
       private void GetTokens()
       {

           try
           {
               string strResponse;

               AddLogEntry("Sending request for tokens...");

               System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
               WebRequest wrRequest = WebRequest.Create(String.Format(TOKEN_URI_TEMPLATE, baseUri));
               wrRequest.Method = "POST";
               wrRequest.ContentType = "application/x-www-form-urlencoded";

               StreamWriter swPost = new StreamWriter(wrRequest.GetRequestStream());
               swPost.Write(FormAuthReq);
               swPost.Flush();
               swPost.Close();

               AddLogEntry("Request sent...waiting for response...");

               WebResponse wrResponse = wrRequest.GetResponse();

               AddLogEntry("Processing response...");
               StreamReader srResponse = new StreamReader(wrResponse.GetResponseStream());
               strResponse = srResponse.ReadToEnd();
               srResponse.Close();

               JavaScriptSerializer jss = new JavaScriptSerializer();
               JSONTokenResponse jtrResponse = (JSONTokenResponse)jss.Deserialize<JSONTokenResponse>(strResponse);

               AccessToken = jtrResponse.access_token;
               RefreshToken = jtrResponse.refresh_token;
               AddLogEntry("Access Token: " + AccessToken);
               AddLogEntry("Refresh Token: " + RefreshToken);


           }
           catch (Exception e)
           {
               Status = OnshapeOAuthStatus.Error;
               AddLogEntry("Error occured in GetTokens: " + e.ToString());
               
           }

       }

       private void AddLogEntry(string strEntry)
       {
           if (IsLogging)
               logWriter(strEntry);
       }

       #endregion
   }
}
