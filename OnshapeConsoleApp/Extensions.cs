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
            map.Select((e) => new string[] { e.Key, e.Value }).PrintAsTable(
                    new double[] { 0.25, 0.75 },
                    new string[] { "", "" },
                    new string[] { "Name", "Value" }
                    );
        }

        public static void PrintAsTable (this IEnumerable<string[]> rows, double[] columnWidth, string[] rowFormat, string[] headers) {
            int columnCount = columnWidth.Length;
            string[] paddedHeaders = new string[columnCount];
            int[] adjustedColumnWidth = new int[columnCount];
            string[] paddedRow = new string[columnCount];
            int totalWidth = Console.WindowWidth - columnCount - 1;
            StringBuilder headerFormatBuilder = new StringBuilder();
            StringBuilder rowFormatBuilder = new StringBuilder();
            for (int i = 0; i < columnWidth.Length; ++i)
            {
                adjustedColumnWidth[i] = (int)(columnWidth[i] * totalWidth);
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
            foreach(var row in rows) {
                for (int i = 0; i < row.Length; ++i)
                {
                    paddedRow[i] = row[i].Shorten(adjustedColumnWidth[i]);
                }
                Console.WriteLine(rowFormatBuilder.ToString(), paddedRow);
            }
            Console.ResetColor();
        }
        
        #endregion

        public static void PrintPlans(this List<OnshapeBillingPlan> plans)
        {
            if (plans != null && plans.Count > 0) 
            plans.Select((p) => new string[] { p.name, p.id, p.planType.ToString(), (p.amountCents / 100.0).ToString() }).PrintAsTable(
                new double[] { -0.35, 0.35, 0.15, 0.15 },
                new string[] { "", "", "", "N2" },
                new string[] { "Name", "Id", "Type", "Amount" }
                );
        }

        public static void PrintDocuments(this List<OnshapeDocument> documents)
        {
            if (documents != null && documents.Count > 0) 
            documents.Select((d) => new string[] {d.name, d.id, d.permission, d.sizeBytes.ToString()}).PrintAsTable(
                new double[] { -0.35, 0.3, 0.15, 0.15},
                new string[] { "", "", "", ""},
                new string[] { "Name", "Id", "Permissions", "Size(bytes)"}
                );
        }
    }
}
