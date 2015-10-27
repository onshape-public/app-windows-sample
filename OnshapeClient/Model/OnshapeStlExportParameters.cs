using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onshape.Api.Client.Model
{
    public class OnshapeStlExportParameters
    {
        public Nullable<Boolean> grouping { get; set; }
        public Nullable<double> scale { get; set; }
        public string units { get; set; }
        public Nullable<double> angleTolerance { get; set; }
        public Nullable<double> chordTolerance { get; set; }
        public Nullable<double> maxFacetWidth { get; set; }
        public Nullable<double> minFacetWidth { get; set; }
        public string mode { get; set; }
    }
}
