using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapeBillingPlan
    {
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public int amountCents { get; set; }
        public string interval { get; set; }
        public string description { get; set; }
        public string group { get; set; }
        public string applicationId { get; set; }
        public OnshapeBillingPlanType planType { get; set; }
        public Nullable<int> trialPeriodDays { get; set; }
    }
}
