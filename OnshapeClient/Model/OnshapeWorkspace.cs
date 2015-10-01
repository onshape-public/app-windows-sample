using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapeWorkspace
    {
        public String href { get; set; }
        public String id { get; set; }
        public String name { get; set; }
        public String description { get; set; }
        public String type { get; set; }
        public String parent { get; set; }
        public List<String> parents { get; set; }
        public Boolean canDelete { get; set; }
        public Boolean isReadOnly { get; set; }
        public String microversion { get; set; }
        public OnshapeUser creator { get; set; }
        public String createdAt { get; set; }
        public OnshapeUser lastModifier { get; set; }
        public String modifiedAt { get; set; }
    }
}
