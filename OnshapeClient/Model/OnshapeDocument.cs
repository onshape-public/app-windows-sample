using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapeDocument
    {
        public string name { get; set; }
        public string id { get; set; }
        public String createdAt { get; set; }
        public OnshapeUser createdBy { get; set; }
        public String modifiedAt { get; set; }
        public OnshapeUser modifiedBy { get; set; }
        public String trashedAt { get; set; }
        public List<String> tags { get; set; }
        public Boolean active { get; set; }
        public Boolean trash { get; set; }
        public Boolean canUnshare { get; set; }
        public String permission { get; set; }
        public int sizeBytes { get; set; }
    }
}
