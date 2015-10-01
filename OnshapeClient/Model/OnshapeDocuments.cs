using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapeDocuments
    {
        public String href { get; set; }
        public List<OnshapeDocument> items { get; set; }
    }
}
