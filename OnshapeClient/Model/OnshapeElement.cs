using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapeElement
    {
        public String microversionId { get; set; }
        public String dataType { get; set; }
        public String elementType { get; set; }
        public String lengthUnits { get; set; }
        public String angleUnits { get; set; }
        public String id { get; set; }
        public String name { get; set; }
        public String foreignDataId { get; set; }
        public String filename { get; set; }
    }
}
