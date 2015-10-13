using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapeElement
    {
        public string microversionId { get; set; }
        public string dataType { get; set; }
        public string elementType { get; set; }
        public string lengthUnits { get; set; }
        public string angleUnits { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string foreignDataId { get; set; }
        public string filename { get; set; }
    }
}
