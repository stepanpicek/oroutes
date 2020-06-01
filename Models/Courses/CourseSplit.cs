using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupy.Models.Courses
{
    /// <summary>
    /// Association class between course and split
    /// </summary>
    public class CourseSplit
    {
        public int CourseID { get; set; }
        public Course Course { get; set; }
        public int SplitID { get; set; }
        public Split Split { get; set; }
        public int Order { get; set; }
    }
}
