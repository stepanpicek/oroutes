using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OBPostupy.Models.Courses;
using OBPostupy.Models.Results;

namespace OBPostupy.ViewModels
{
    public class SplitResultsViewModel
    {
        public List<SplitTime> SplitTimes { get; set; }
        public Split Split { get; set; }
        public bool isForComparison { get; set; }
        public int PersonResult { get; set; }
    }
}
