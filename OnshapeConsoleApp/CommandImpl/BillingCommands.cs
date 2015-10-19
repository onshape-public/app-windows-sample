using Newtonsoft.Json;
using Onshape.Api.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Onshape.Api.ConsoleApp.CommandImpl
{
    internal static class BillingCommands
    {
        internal const int PURCHASE_WORKFLOW_TIMEOUT = 60000; // 60 seconds
        internal const string PURCHASE_WORKFLOW_CALLBACK = @"http://localhost:9326";
        internal const string PURCHASE_WORKFLOW_URI_TEMPLATE = @"{0}/billing/purchase?redirectUri={1}&clientId={2}&sku={3}";

        internal static async Task GetPlans(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            if (values != null && values.Count > 0)
            {
                OnshapeBillingPlan plan = await context.Client.GetBillingPlan(values[0]);
                Console.WriteLine(JsonConvert.SerializeObject(plan));
            }
            else
            {
                List<OnshapeBillingPlan> plans = await context.Client.GetClientBillingPlans();
                plans.Print();
            }
        }

        internal static async Task GetPurchases(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            List<OnshapePurchase> purchases = await context.Client.GetPurchases();
            purchases.Print();
        }

        internal static async Task CancelPurchase(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            if (values != null && values.Count > 0)
            {
                await context.Client.CancelPurchase(values[0]);
            }
            else
            {
                Console.WriteLine("Error: 'sku' is required");
            }
        }

        internal static async Task ConsumePurchase(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            if (values != null && values.Count > 0)
            {
                await context.Client.ConsumePurchase(values[0]);
            }
            else
            {
                Console.WriteLine("Error: 'sku' is required");
            }
        }

        internal static async Task Purchase(CommandExecutionContext context, Dictionary<string, List<string>> options, List<string> values)
        {
            if (values != null && values.Count > 0)
            {
                string purchaseWorkflowUri = String.Format(PURCHASE_WORKFLOW_URI_TEMPLATE, context.BaseURL,
                    HttpUtility.UrlEncode(PURCHASE_WORKFLOW_CALLBACK),
                    HttpUtility.UrlEncode(context.Client.ClientId),
                    HttpUtility.UrlEncode(values[0]));
                HttpListenerRequest result = await Utils.ExecuteBrowserWorkflow(purchaseWorkflowUri, "Please, close this window.", PURCHASE_WORKFLOW_CALLBACK, PURCHASE_WORKFLOW_TIMEOUT);
                if (result != null && Boolean.Parse(result.QueryString["success"]))
                {
                    Console.WriteLine("Purchase succedded.");
                }
                else
                {
                    Console.WriteLine("Purchase failed.");
                }
            }
            else
            {
                Console.WriteLine("Error: 'sku' is required");
            }
        }
    }
}
