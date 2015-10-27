using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapeAssemblyTranslationParameters : OnshapeTranslationParameters
    {
        public Nullable<Boolean> flattenAssemblies { get; set; }
        public Nullable<Boolean> yAxisIsUp { get; set; }
    }
}
