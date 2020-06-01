using OBPostupy.Models.Courses;
using OBPostupy.Models.Results;
using OBPostupy.Models.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupy.ViewModels
{
    public class SplitComparisonViewModel
    {
        public List<PersonResult> People { get; set; }
        public Split Split{ get; set; }
        public double SplitDistance { get; set; }

    }
}
