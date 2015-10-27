using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapeElementTranslation : OnshapeElement
    {
        public String translationId { get; set; }
        public String translationEventKey { get; set; }
    }
}
