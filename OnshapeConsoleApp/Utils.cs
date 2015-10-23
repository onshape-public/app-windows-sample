using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.ConsoleApp
{
    internal static class Utils
    {
        #region Browser workflow helpers

        internal static async Task<HttpListenerRequest> ExecuteBrowserWorkflow(string uri, string message, string callBackUri, int timeOut)
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

        #region I/o utils

        internal static async Task ProcessContentStream(Stream contentStream, Dictionary<string, List<string>> options)
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
                    long bufferLength = (contentStream.Length > Constants.MAX_FILE_LENGTH_TO_PRINT_OUT) ? Constants.MAX_FILE_LENGTH_TO_PRINT_OUT : contentStream.Length;
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

        #endregion

        #region Debug utils

        internal static void PrintContext(CommandExecutionContext executionContext)
        {
            Console.WriteLine("Execution context:");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            try
            {
                Console.WriteLine(JsonConvert.SerializeObject(executionContext, Formatting.Indented));
            }
            finally
            {
                Console.ResetColor();
            }
            Console.WriteLine("Cached options:");
            try
            {
                Console.WriteLine(JsonConvert.SerializeObject(Program.OptionValueCache, Formatting.Indented));
            }
            finally
            {
                Console.ResetColor();
            }
        }

        internal static void ClearContext()
        {
            SetRegistry(Constants.REFRESH_TOKEN_KEY_NAME, "");
            Program.BaseExecutionContext = null;
            Program.OptionValueCache.Clear();
        }
        #endregion

        #region Registry

        internal static void SetRegistry(string name, string value)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                Assembly a = Assembly.GetExecutingAssembly();
                string appVersion = a.GetName().Version.ToString();
                string appName = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(a, typeof(AssemblyTitleAttribute), false)).Title;
                key.CreateSubKey(appName);
                key = key.OpenSubKey(appName, true);
                key.CreateSubKey(appVersion);
                key = key.OpenSubKey(appVersion, true);
                key.SetValue(name, value);
            }
            catch (Exception)
            {
                Console.WriteLine("Error: registry write failed");
            }
        }

        internal static string GetRegistry(string name)
        {
            string result = null;
            try
            {
                Assembly a = Assembly.GetExecutingAssembly();
                string appVersion = a.GetName().Version.ToString();
                string appName = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(a, typeof(AssemblyTitleAttribute), false)).Title;
                string keyName = String.Format(Constants.REGISTRY_KEY_FORMAT, appName, appVersion);
                result = (string)Registry.GetValue(keyName, name, null);
            }
            catch (Exception)
            {
                Console.WriteLine("Error: registry read failed");
            }
            return result;
        }

        #endregion
    }
}
