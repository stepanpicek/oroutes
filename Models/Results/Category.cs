using System;
using System.Collections.Generic;
using OBPostupy.Models.Races;
using OBPostupy.Models.Courses;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupy.Models.Results
{
    /// <summary>
    /// Data model for category
    /// </summary>
    public class Category
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }

        public int? CourseID { get; set; }
        public Course Course { get; set; }
        public int? RaceID { get; set; }
        public Race Race { get; set; }
        public List<PersonResult> PersonResults { get; set; }
    }
}
