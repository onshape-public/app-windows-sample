using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client
{
    public static class Extensions
    {
        public static StringBuilder AppendQueryParam<T>(this System.Text.StringBuilder stringBuilder, String name, T value)
        {
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append("&");
            }
            return stringBuilder.Append(String.Format("{0}={1}", name, value));
        }
    }
}
