using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapePartstudioTranslationParameters : OnshapeTranslationParameters
    {
        public string[] partIds { get; set; }
    }
}
