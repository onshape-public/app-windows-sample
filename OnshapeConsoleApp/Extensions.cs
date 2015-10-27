using Onshape.Api.Client.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.ConsoleApp
{
    public static class Extensions
    {
        #region String utils

        public static string AlignCenter(this string text, int width)
        {
            width = Math.Abs(width);
            return text.Shorten(width).PadCenter(width);
        }

        public static string Shorten(this string text, int width)
        {
            width = Math.Abs(width);
            string result = null;
            if (text != null && width > 3)
            {
                if (text.Length < width)
                {
                    result = text;
                }
                else
                {
                    result = String.Format("{0}...", text.Substring(0, width - 3));
                }
            }
            return result;
        }

        public static string PadCenter(this string text, int width)
        {
            string result = null;
            if (string.IsNullOrEmpty(text))
            {
                result = new string(' ', width);
            }
            else
            {
                result = text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
            return result;
        }

        #endregion

        #region Console output helpers

        public static void PrintAsTable(this Dictionary<String, String> map)
        {
            if (map != null)
            map.Select((e) => new object[] { e.Key, e.Value }).PrintAsTable(
                    new int[] { 25, 10 },
                    new int[] { 25, 120 },
                    new string[] { "", "" },
                    new string[] { "Name", "Value" }
                    );
        }

        public static void PrintAsTable(this IEnumerable<object[]> rows, int[] columnWidthMin, int[] columnWidthMax, string[] rowFormat, string[] headers)
        {
            int columnCount = columnWidthMin.Length;
            string[] paddedHeaders = new string[columnCount];
            int[] adjustedColumnWidth = new int[columnCount];
            double[] columnWidthRelativeVariability = new double[columnCount];
            object[] paddedRow = new object[columnCount];
            int totalAvailableWidth = Console.WindowWidth - columnCount - 1;
            double totalColumnWidthRelativeVariability = 0.0;
            int totalRequiredWidthMin = 0;
            for (int i = 0; i< columnCount; ++i)
            {
                columnWidthRelativeVariability[i] = (Math.Abs(columnWidthMax[i]) - Math.Abs(columnWidthMin[i])) / (double)columnWidthMin[i];
                totalColumnWidthRelativeVariability += Math.Abs(columnWidthRelativeVariability[i]);
                totalRequiredWidthMin += Math.Abs(columnWidthMin[i]);
            }
            int totalDynamicWidth = totalAvailableWidth - totalRequiredWidthMin;
            StringBuilder headerFormatBuilder = new StringBuilder();
            StringBuilder rowFormatBuilder = new StringBuilder();
            for (int i = 0; i < columnCount; ++i)
            {
                double adjustmentRatio = (totalDynamicWidth > 0) ? columnWidthRelativeVariability[i] / totalColumnWidthRelativeVariability : columnWidthMin[i] / totalRequiredWidthMin;
                adjustedColumnWidth[i] = (int)(columnWidthMin[i] + adjustmentRatio * totalDynamicWidth);
                if (i > 0)
                {
                    headerFormatBuilder.Append(' ');
                    rowFormatBuilder.Append(' ');
                }
                headerFormatBuilder.Append(String.Format("{{{0} , {1}}}", i, adjustedColumnWidth[i]));
                if (!String.IsNullOrEmpty(rowFormat[i]))
                {
                    rowFormatBuilder.Append(String.Format("{{{0} , {1}:{2}}}", i, adjustedColumnWidth[i], rowFormat[i]));
                }
                else
                {
                    rowFormatBuilder.Append(String.Format("{{{0} , {1}}}", i, adjustedColumnWidth[i]));
                }
                paddedHeaders[i] = headers[i].AlignCenter(adjustedColumnWidth[i]);
            }
            Console.WriteLine(headerFormatBuilder.ToString(), paddedHeaders);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            foreach (var row in rows)
            {
                for (int i = 0; i < row.Length; ++i)
                {
                    paddedRow[i] = row[i];
                    if (String.IsNullOrEmpty(rowFormat[i])) {
                        // TODO: need to be smarted here 
                        paddedRow[i] = row[i].ToString().Shorten(adjustedColumnWidth[i]);
                    }
                }
                Console.WriteLine(rowFormatBuilder.ToString(), paddedRow);
            }
            Console.ResetColor();
        }

        #endregion

        public static void Print(this List<OnshapePurchase> purchases)
        {
            if (purchases != null && purchases.Count > 0)
                purchases.Select((p) => new object[] { p.planName, p.id, p.planType.ToString(), p.state.ToString(), p.amountCents / 100.0, p.subscriptionEndAt.ToString() }).PrintAsTable(
                    new int[] { -12, 26, 10, 10, 10, 24 },
                    new int[] { -30, 26, 10, 15, 15, 50 },
                    new string[] { "", "", "", "", "C2", "" },
                    new string[] { "Name", "Id", "Type", "State", "Amount", "Subscription End Date" }
                    );
        }

        public static void Print(this List<OnshapeBillingPlan> plans)
        {
            if (plans != null && plans.Count > 0) 
            plans.Select((p) => new object[] { p.name, p.id, p.description, p.planType.ToString(), p.amountCents / 100.0 }).PrintAsTable(
                new int[] { -15, 26, -26, 15, 10 },
                new int[] { -30, 30, -32, 20, 12 },
                new string[] { "", "", "", "", "C2" },
                new string[] { "Name", "Id", "Description", "Type", "Amount" }
                );
        }

        public static void Print(this List<OnshapeDocument> documents)
        {
            if (documents != null && documents.Count > 0) 
            documents.Select((d) => new object[] {d.name, d.createdAt.ToString(),  d.id, d.permission, d.sizeBytes}).PrintAsTable(
                new int[] { -15, 26, 25, 12, 15},
                new int[] { -21, 31, 31, 15, 20 },
                new string[] { "", "", "", "", "" },
                new string[] { "Name", "Created At", "Id", "Permissions", "Size(bytes)" }
                );
        }

        public static string GetOptionValue(this Dictionary<string, List<string>> options, string name)
        {
            return options != null && options.ContainsKey(name) ? options[name][0] : null;
        }
    }
}
