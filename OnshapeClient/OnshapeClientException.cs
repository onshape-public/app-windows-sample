using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client
{
    public class OnshapeClientException : Exception
    {
        public OnshapeClientException()
        {
        }

        public OnshapeClientException(string message)
            : base(message)
        {
        }

        public OnshapeClientException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
