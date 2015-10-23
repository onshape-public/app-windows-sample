using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapeTranslationParameters
    {
        public string elementId { get; set; }
        public string partIds { get; set; }
        public string formatName { get; set; }
        public Boolean flattenAssemblies { get; set; }
        public Boolean yAxisIsUp { get; set; }
        public Boolean storeInDocument { get; set; }
        public string versionString { get; set; }
        public Boolean grouping { get; set; }
    }
}
