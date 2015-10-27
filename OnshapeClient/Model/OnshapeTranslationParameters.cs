using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapeTranslationParameters
    {
        public string formatName { get; set; }
        public string versionString { get; set; }
        public Nullable<Boolean> storeInDocument { get; set; }
    }
}
