using Newtonsoft.Json;
using Onshape.Api.Client.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Onshape.Api.ConsoleApp.CommandImpl
{
    internal static class ExportImportCommands
    {
        internal static async Task DownloadPartstudio(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            string documentId = options[Constants.DOCUMENT_ID][0];
            string wmvSelector = (options.ContainsKey(Constants.WORKSPACE_ID)) ? "w" : "v";
            string selectorId = (options.ContainsKey(Constants.WORKSPACE_ID)) ? options[Constants.WORKSPACE_ID][0] : options[Constants.VERSION_ID][0];
            string elementId = options[Constants.ELEMENT_ID][0];
            string format = (values != null && values.Count == 1) ? values[0] : "stl";
            using (Stream contentStream = await context.Client.DownloadPartstudio(documentId, wmvSelector, selectorId, elementId, format))
            {
                await Utils.ProcessContentStream(contentStream, options);
            }
        }
        internal static async Task GetPartstudioTranslationFormats(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            List<OnshapeTranslationFormat> formats = await context.Client.GetPartstudioTranslationFormats(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0], options[Constants.ELEMENT_ID][0]);
            Console.WriteLine(JsonConvert.SerializeObject(formats, Formatting.Indented));
        }
        internal static async Task CreatePartStudioTranslation(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            OnshapeTranslationParameters translationParameters = new OnshapeTranslationParameters() { elementId = options[Constants.ELEMENT_ID][0],
                formatName = options.ContainsKey(Constants.FORMAT) ? options[Constants.FORMAT][0] : null,
                versionString = options.ContainsKey(Constants.FORMAT_VERSION) ? options[Constants.FORMAT_VERSION][0] : null,
                storeInDocument = true
            };
            OnshapeTranslationStatus status = await context.Client.CreatePartstudioTranslation(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0], options[Constants.ELEMENT_ID][0], translationParameters);
            Console.WriteLine(JsonConvert.SerializeObject(status, Formatting.Indented));
        }
        internal static async Task GetAssemblyTranslationFormats(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            List<OnshapeTranslationFormat> formats = await context.Client.GetAssemblyTranslationFormats(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0], options[Constants.ELEMENT_ID][0]);
            Console.WriteLine(JsonConvert.SerializeObject(formats, Formatting.Indented));
        }
        internal static async Task CreateAssemblyTranslation(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            OnshapeTranslationParameters translationParameters = new OnshapeTranslationParameters()
            {
                elementId = options[Constants.ELEMENT_ID][0],
                formatName = options.ContainsKey(Constants.FORMAT) ? options[Constants.FORMAT][0] : null,
                versionString = options.ContainsKey(Constants.FORMAT_VERSION) ? options[Constants.FORMAT_VERSION][0] : null,
                storeInDocument = true
            };
            OnshapeTranslationStatus status = await context.Client.CreateAssemblyTranslation(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0], options[Constants.ELEMENT_ID][0], translationParameters);
            Console.WriteLine(JsonConvert.SerializeObject(status, Formatting.Indented));
        }
        internal static async Task UploadBlobelement(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            Dictionary<String, String> formFields = new Dictionary<String, String> {
                {"flattenAssemblies", "false"},
                {"yAcisIsUp", "false"},
                {"ownerId", "undefined"},
                {"encodedFileName", HttpUtility.UrlEncode(System.IO.Path.GetFileName(options[Constants.FILE][0]))}
            };
            var response = await context.Client.CreateBlobelement(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0], formFields, options[Constants.FILE][0]);
            Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        }
        internal static async Task UpdateBlobelement(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            Dictionary<String, String> formFields = new Dictionary<String, String> {
                {"flattenAssemblies", "false"},
                {"yAcisIsUp", "false"},
                {"ownerId", "undefined"},
                {"encodedFileName", HttpUtility.UrlEncode(System.IO.Path.GetFileName(options[Constants.FILE][0]))}
            };
            var response = await context.Client.UpdateBlobelement(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0], options[Constants.ELEMENT_ID][0], formFields, options[Constants.FILE][0]);
            Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        }
        internal static async Task DownloadBlobelement(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            string documentId = options[Constants.DOCUMENT_ID][0];
            string wmvSelector = (options.ContainsKey(Constants.WORKSPACE_ID)) ? "w" : "v";
            string selectorId = (options.ContainsKey(Constants.WORKSPACE_ID)) ? options[Constants.WORKSPACE_ID][0] : options[Constants.VERSION_ID][0];
            string elementId = options[Constants.ELEMENT_ID][0];
            using (Stream contentStream = await context.Client.DownloadBlobelement(documentId, wmvSelector, selectorId, elementId))
            {
                await Utils.ProcessContentStream(contentStream, options);
            }
        }
        internal static async Task DownloadPart(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            string documentId = options[Constants.DOCUMENT_ID][0];
            string wmvSelector = (options.ContainsKey(Constants.WORKSPACE_ID)) ? "w" : "v";
            string selectorId = (options.ContainsKey(Constants.WORKSPACE_ID)) ? options[Constants.WORKSPACE_ID][0] : options[Constants.VERSION_ID][0];
            string elementId = options[Constants.ELEMENT_ID][0];
            string partId = options[Constants.PART_ID][0];
            string format = (values != null && values.Count == 1) ? values[0] : "stl";
            using (Stream contentStream = await context.Client.DownloadPart(documentId, wmvSelector, selectorId, elementId, partId, format))
            {
                await Utils.ProcessContentStream(contentStream, options);
            }
        }
        internal static async Task GetBlobElementTranslationFormats(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            List<OnshapeTranslationFormat> formats = await context.Client.GetBlobelementTranslationFormats(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0], options[Constants.ELEMENT_ID][0]);
            Console.WriteLine(JsonConvert.SerializeObject(formats, Formatting.Indented));
        }
        internal static async Task CreateBlobElementTranslation(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            OnshapeTranslationParameters translationParameters = new OnshapeTranslationParameters()
            {
                elementId = options[Constants.ELEMENT_ID][0],
                formatName = options.ContainsKey(Constants.FORMAT) ? options[Constants.FORMAT][0] : null,
                versionString = options.ContainsKey(Constants.FORMAT_VERSION) ? options[Constants.FORMAT_VERSION][0] : null,
                storeInDocument = true
            };
            OnshapeTranslationStatus status = await context.Client.CreateBlobelementTranslation(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0], options[Constants.ELEMENT_ID][0], translationParameters);
            Console.WriteLine(JsonConvert.SerializeObject(status, Formatting.Indented));
        }
        internal static async Task CreateTranslation(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            Dictionary<String, String> formFields = new Dictionary<String, String> {
                {"flattenAssemblies", "false"},
                {"yAcisIsUp", "false"},
                {"ownerId", "undefined"},
                {"encodedFileName", HttpUtility.UrlEncode(System.IO.Path.GetFileName(options[Constants.FILE][0]))}
            };
            OnshapeTranslationStatus status = await context.Client.CreateTranslation(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0], formFields, options[Constants.FILE][0]);
            Console.WriteLine(JsonConvert.SerializeObject(status, Formatting.Indented));
        }
        internal static async Task GetTranslationStatus(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            OnshapeTranslationStatus status = await context.Client.GetTranslationStatus(options[Constants.TRANSLATION_ID][0]);
            Console.WriteLine(JsonConvert.SerializeObject(status, Formatting.Indented));
        }
        internal static async Task GetDocumentTranslationStatus(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            OnshapeTranslationStatusList status = await context.Client.GetDocumentTranslationStatus(options[Constants.DOCUMENT_ID][0]);
            Console.WriteLine(JsonConvert.SerializeObject(status, Formatting.Indented));
        }
        internal static async Task DeleteTranslationStatus(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            await context.Client.DeleteTranslationStatusEntry(options[Constants.TRANSLATION_ID][0]);
        }
    }
}
