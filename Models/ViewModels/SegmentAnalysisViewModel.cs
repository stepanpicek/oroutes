using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OBPostupy.Models.People;

namespace OBPostupy.Models.GPS
{
    public class SegmentAnalysisViewModel:PathAnalysisViewModel
    {
        public Person Person { get; set; }
        public SegmentAnalysisViewModel()
        {

        }

        public SegmentAnalysisViewModel(PathAnalysisViewModel pathAnalysis) :base(pathAnalysis)
        {

        }
    }
}
