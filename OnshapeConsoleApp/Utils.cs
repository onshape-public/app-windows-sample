using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.ConsoleApp
{
    internal static class Utils
    {
        #region Browser workflow helpers

        public static async Task<HttpListenerRequest> ExecuteBrowserWorkflow(string uri, string message, string callBackUri, int timeOut)
        {
            Process pWeb = new Process();
            HttpListenerRequest result = null;
            try
            {
                using (HttpListener httpListener = new HttpListener())
                {
                    Console.WriteLine("Opening browser for a browser based workflow...");
                    httpListener.Prefixes.Add(callBackUri + "/");
                    httpListener.Start();
                    pWeb.StartInfo.FileName = uri;
                    pWeb.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    // Start browser
                    pWeb.Start();
                    // Wait for call back
                    var task = httpListener.GetContextAsync();
                    if (await Task.WhenAny(task, Task.Delay(timeOut)) == task)
                    {
                        HttpListenerContext hlcContext = task.Result;
                        HttpListenerResponse hlrpResponse = hlcContext.Response;
                        result = hlcContext.Request;
                        // Write the response back to browser
                        string strResponse = String.Format(@"<HTML><HEAD><TITLE>{0}</TITLE></HEAD><BODY>Please close the window.</BODY></HTML>", message);
                        byte[] bBuffer = System.Text.Encoding.UTF8.GetBytes(strResponse);
                        hlrpResponse.ContentLength64 = bBuffer.Length;
                        System.IO.Stream smOutput = hlrpResponse.OutputStream;
                        smOutput.Write(bBuffer, 0, bBuffer.Length);
                        smOutput.Close();
                        httpListener.Stop();
                    }
                    else
                    {
                        Console.WriteLine(@"Operation canceled - timeout");
                    }
                }
            }
            finally
            {
                pWeb.Close();
            }
            return result;
        }

        #endregion
    }
}
