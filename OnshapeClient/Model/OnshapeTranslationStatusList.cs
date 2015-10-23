using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapeTranslationStatusList
    {
        public List<OnshapeTranslationStatus> items { get; set; }
        public string href { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
    }
}
