using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupy.Models.Courses
{
    /// <summary>
    /// Datový model postupu mezi dvema kontrolami
    /// </summary>
    public class Split
    {
        public int ID { get; set; }
        public int? FirstControlID { get; set; }
        public Control FirstControl { get; set; }
        public int? SecondControlID { get; set; }
        public Control SecondControl { get; set; }
        public int? CourseDataID { get; set; }
        public CourseData CourseData { get; set; }
        public List<CourseSplit> CourseSplits { get; set; }
        public List<Results.SplitTime> SplitTimes { get; }
    }
}
