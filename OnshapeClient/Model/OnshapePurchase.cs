using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapePurchase
    {
        public string id { get; set; }
        public List<string> userIds { get; set; }
        public List<string> consumedIds { get; set; }
        public Nullable<long> seats { get; set; }
        public string accountId { get; set; }
        public string planId { get; set; }
        public OnshapeBillingPlanType planType { get; set; }
        public string planName { get; set; }
        public string group { get; set; }
        public string applicationId { get; set; }
        public string clientId { get; set; }
        public int state { get; set; }
        public Nullable<DateTime> canceledAt { get; set; }
        public Nullable<DateTime> subscriptionEndAt { get; set; }
        public string currency { get; set; }
        public long amountCents { get; set; }
    }
}
