using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OBPostupy.Models.Results;

namespace OBPostupy.Models.Courses
{
    /// <summary>
    /// Data model of course
    /// </summary>
    public class Course
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? CourseDataID { get; set; }
        public CourseData CourseData { get; set; }
        public List<CourseControl> CourseControl { get; }
        public List<CourseSplit> CourseSplits { get; }
        public List<Category> Categories { get; }
       
    }
}
