using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupy.Models.Courses
{
    /// <summary>
    /// Association class between control and course in data model
    /// </summary>
    public class CourseControl
    {
        public int CourseID { get; set; }
        public Course Course { get; set; }
        public int ControlID { get; set; }
        public Control Control { get; set; }
        public int Order { get; set; }
        public string Type { get; set; }
    }
}
