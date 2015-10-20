using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapeThumbnails
    {
        public string href { get; set; }
        public List<OnshapeThumbnail> sizes { get; set; }
    }
}
