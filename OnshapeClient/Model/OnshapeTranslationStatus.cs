using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapeTranslationStatus
    {
        public string id { get; set; }
        public string documentId { get; set; }
        public string workspaceId { get; set; }
        public string requestElementId { get; set; }
        public OnshapeTranslationStateType requestState { get; set; }
        public List<string> resultElementIds { get; set; }
        public List<string> resultExternalDataIds { get; set; }
        public string failureReason { get; set; }
    }
}
