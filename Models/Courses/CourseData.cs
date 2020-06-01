using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OBPostupy.Models.Races;

namespace OBPostupy.Models.Courses
{
    /// <summary>
    /// Data model of all course data
    /// </summary>
    public class CourseData
    {
        public int ID { get; set; }
        public int RaceID { get; set; }
        public Race Race { get; set; }
        public List<Control> Controls { get; set; }
        public List<Split> Splits { get; set; }
        public List<Course> Courses { get; set; }
    }
}
