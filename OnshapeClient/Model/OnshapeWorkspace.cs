using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapeWorkspace
    {
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string parent { get; set; }
        public List<string> parents { get; set; }
        public Boolean canDelete { get; set; }
        public Boolean isReadOnly { get; set; }
        public string microversion { get; set; }
        public OnshapeUser creator { get; set; }
        public Nullable<DateTime> createdAt { get; set; }
        public OnshapeUser lastModifier { get; set; }
        public Nullable<DateTime> modifiedAt { get; set; }
    }
}
