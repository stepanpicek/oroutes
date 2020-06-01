using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OBPostupy.Models.Courses;

namespace OBPostupy.Models.GPS
{
    public class SplitAnalysisViewModel:PathAnalysisViewModel
    {
        public TimeSpan FindControlTime { get; set; }
        public double SplitDistance { get; set; }
        public string FirstControlCode { get; set; }
        public string SecondControlCode { get; set; }
        public SplitAnalysisViewModel(PathAnalysisViewModel pathAnalysis) : base(pathAnalysis)
        {

        }
    }
}
