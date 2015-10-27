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
        internal static async Task ExportPartstudio(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            string documentId = options[Constants.DOCUMENT_ID][0];
            string wmvSelector = (options.ContainsKey(Constants.WORKSPACE_ID)) ? "w" : "v";
            string selectorId = (options.ContainsKey(Constants.WORKSPACE_ID)) ? options[Constants.WORKSPACE_ID][0] : options[Constants.VERSION_ID][0];
            string elementId = options[Constants.ELEMENT_ID][0];
            string format = options[Constants.FORMAT][0];
            switch (format.ToUpperInvariant())
            {
                case "STL":
                    using (Stream contentStream = await context.Client.ExportPartstudioToStl(documentId, wmvSelector, selectorId, elementId,
                        Utils.createStlExportParams(options), values != null ? values.ToArray() : null))
                    {
                        await Utils.ProcessContentStream(contentStream, options);
                    }
                    break;
                case "PARASOLID":
                    string formatVersion = options.GetOptionValue(Constants.FORMAT_VERSION);
                    using (Stream contentStream = await context.Client.ExportPartstudioToParasolid(documentId, wmvSelector, selectorId, elementId, formatVersion, values != null?values.ToArray():null))
                    {
                        await Utils.ProcessContentStream(contentStream, options);
                    }
                    break;
                default:
                    Console.WriteLine("Invalid format");
                    break;
            }
        }
        internal static async Task GetPartstudioTranslationFormats(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            List<OnshapeTranslationFormat> formats = await context.Client.GetPartstudioTranslationFormats(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0], options[Constants.ELEMENT_ID][0]);
            Console.WriteLine(JsonConvert.SerializeObject(formats, Formatting.Indented));
        }
        internal static async Task CreatePartStudioTranslation(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            OnshapePartstudioTranslationParameters translationParameters = new OnshapePartstudioTranslationParameters() { 
                formatName = options.GetOptionValue(Constants.FORMAT),
                versionString = options.GetOptionValue(Constants.FORMAT_VERSION),
                partIds = values != null ? values.ToArray() : null
            };
            string storeInDocumentStr = options.GetOptionValue(Constants.STORE_IN_DOCUMENT);
            if (!String.IsNullOrEmpty(storeInDocumentStr))
            {
                translationParameters.storeInDocument = Boolean.Parse(storeInDocumentStr);
            }
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
            OnshapeAssemblyTranslationParameters translationParameters = new OnshapeAssemblyTranslationParameters()
            {
                formatName = options.GetOptionValue(Constants.FORMAT),
                versionString = options.GetOptionValue(Constants.FORMAT_VERSION)
            };
            string flattenAssembliesStr = options.GetOptionValue(Constants.FLATTEN_ASSEMBLIES);
            string storeInDocumentStr = options.GetOptionValue(Constants.STORE_IN_DOCUMENT);
            string yAxisIsUpStr = options.GetOptionValue(Constants.Y_AXIS_IS_UP);
            if (!String.IsNullOrEmpty(flattenAssembliesStr))
            {
                translationParameters.flattenAssemblies = Boolean.Parse(flattenAssembliesStr);
            }
            if (!String.IsNullOrEmpty(storeInDocumentStr))
            {
                translationParameters.storeInDocument = Boolean.Parse(storeInDocumentStr);
            }
            if (!String.IsNullOrEmpty(yAxisIsUpStr))
            {
                translationParameters.yAxisIsUp = Boolean.Parse(yAxisIsUpStr);
            }
            OnshapeTranslationStatus status = await context.Client.CreateAssemblyTranslation(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0], options[Constants.ELEMENT_ID][0], translationParameters);
            Console.WriteLine(JsonConvert.SerializeObject(status, Formatting.Indented));
        }
        internal static async Task UploadBlobelement(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            string flattenAssembliesStr = options.GetOptionValue(Constants.FLATTEN_ASSEMBLIES);
            string yAxisIsUpStr = options.GetOptionValue(Constants.Y_AXIS_IS_UP);
            Dictionary<String, String> formFields = new Dictionary<String, String> {
                {"encodedFileName", HttpUtility.UrlEncode(System.IO.Path.GetFileName(options[Constants.FILE][0]))}
            };
            if (flattenAssembliesStr != null)
            {
                formFields["flattenAssemblies"] = flattenAssembliesStr;
            }
            if (yAxisIsUpStr != null)
            {
                formFields["yAxisIsUpStr"] = yAxisIsUpStr;
            }
            var response = await context.Client.CreateBlobelement(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0], formFields, options[Constants.FILE][0]);
            Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        }
        internal static async Task UpdateBlobelement(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            string flattenAssembliesStr = options.GetOptionValue(Constants.FLATTEN_ASSEMBLIES);
            string yAxisIsUpStr = options.GetOptionValue(Constants.Y_AXIS_IS_UP);
            Dictionary<String, String> formFields = new Dictionary<String, String> {
                {"encodedFileName", HttpUtility.UrlEncode(System.IO.Path.GetFileName(options[Constants.FILE][0]))}
            };
            if (flattenAssembliesStr != null)
            {
                formFields["flattenAssemblies"] = flattenAssembliesStr;
            }
            if (yAxisIsUpStr != null)
            {
                formFields["yAxisIsUpStr"] = yAxisIsUpStr;
            }
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
        internal static async Task ExportPart(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            string documentId = options[Constants.DOCUMENT_ID][0];
            string wmvSelector = (options.ContainsKey(Constants.WORKSPACE_ID)) ? "w" : "v";
            string selectorId = (options.ContainsKey(Constants.WORKSPACE_ID)) ? options[Constants.WORKSPACE_ID][0] : options[Constants.VERSION_ID][0];
            string elementId = options[Constants.ELEMENT_ID][0];
            string partId = options[Constants.PART_ID][0];
            string format = options[Constants.FORMAT][0];
            switch (format.ToUpperInvariant())
            {
                case "STL":
                    using (Stream contentStream = await context.Client.ExportPartToStl(documentId, wmvSelector, selectorId, elementId, partId,
                        Utils.createStlExportParams(options)))
                    {
                        await Utils.ProcessContentStream(contentStream, options);
                    }
                    break;
                case "PARASOLID":
                    string formatVersion = options.GetOptionValue(Constants.FORMAT_VERSION);
                    using (Stream contentStream = await context.Client.ExportPartToParasolid(documentId, wmvSelector, selectorId, elementId, partId, formatVersion))
                    {
                        await Utils.ProcessContentStream(contentStream, options);
                    }
                    break;
                default:
                    Console.WriteLine("Invalid format");
                    break;
            }
        }
        internal static async Task GetBlobElementTranslationFormats(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            List<OnshapeTranslationFormat> formats = await context.Client.GetBlobelementTranslationFormats(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0], options[Constants.ELEMENT_ID][0]);
            Console.WriteLine(JsonConvert.SerializeObject(formats, Formatting.Indented));
        }
        internal static async Task CreateBlobElementTranslation(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            OnshapeBlobTranslationParameters translationParameters = new OnshapeBlobTranslationParameters()
            {
                formatName = options.ContainsKey(Constants.FORMAT) ? options[Constants.FORMAT][0] : null,
                versionString = options.ContainsKey(Constants.FORMAT_VERSION) ? options[Constants.FORMAT_VERSION][0] : null
            };
            string flattenAssembliesStr = options.GetOptionValue(Constants.FLATTEN_ASSEMBLIES);
            string storeInDocumentStr = options.GetOptionValue(Constants.STORE_IN_DOCUMENT);
            string yAxisIsUpStr = options.GetOptionValue(Constants.Y_AXIS_IS_UP);
            if (!String.IsNullOrEmpty(flattenAssembliesStr))
            {
                translationParameters.flattenAssemblies = Boolean.Parse(flattenAssembliesStr);
            }
            if (!String.IsNullOrEmpty(storeInDocumentStr))
            {
                translationParameters.storeInDocument = Boolean.Parse(storeInDocumentStr);
            }
            if (!String.IsNullOrEmpty(yAxisIsUpStr))
            {
                translationParameters.yAxisIsUp = Boolean.Parse(yAxisIsUpStr);
            }
            OnshapeTranslationStatus status = await context.Client.CreateBlobelementTranslation(options[Constants.DOCUMENT_ID][0], options[Constants.WORKSPACE_ID][0], options[Constants.ELEMENT_ID][0], translationParameters);
            Console.WriteLine(JsonConvert.SerializeObject(status, Formatting.Indented));
        }
        internal static async Task CreateTranslation(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            Dictionary<String, String> formFields = new Dictionary<String, String> {
                {"encodedFileName", HttpUtility.UrlEncode(System.IO.Path.GetFileName(options[Constants.FILE][0]))},
                {"formatName", options[Constants.FORMAT][0].ToUpperInvariant()},
                {"name", HttpUtility.UrlEncode(System.IO.Path.GetFileNameWithoutExtension(options[Constants.FILE][0]))}
            };
            string flattenAssembliesStr = options.GetOptionValue(Constants.FLATTEN_ASSEMBLIES);
            string yAxisIsUpStr = options.GetOptionValue(Constants.Y_AXIS_IS_UP);
            string fileExtension = System.IO.Path.GetExtension(options[Constants.FILE][0]);
            if (flattenAssembliesStr != null)
            {
                formFields["flattenAssemblies"] = flattenAssembliesStr;
            }
            if (yAxisIsUpStr != null)
            {
                formFields["yAxisIsUpStr"] = yAxisIsUpStr;
            }
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
