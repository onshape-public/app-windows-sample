using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public enum OnshapePurchaseStateType
    {
        UNKNOWN = 0,
        DELETED,
        CANCELED,
        UNPAID,
        PAST_DUE,
        TRIALING,
        ACTIVE,
        CONSUMED
    }
}
