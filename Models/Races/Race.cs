using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OBPostupy.Models.Results;
using OBPostupy.Models.Maps;
using OBPostupy.Models.Courses;

namespace OBPostupy.Models.Races
{
    /// <summary>
    /// Data model for Race
    /// </summary>
    public class Race
    {
        public int ID { get; set; }
        [Display(Name="Název")]
        public string Name { get; set; }
        [Display(Name = "Start")]
        public DateTime StartTime { get; set; }
        public List<PersonResult> PersonResults { get; set; }
        public List<Category> Categories { get; set; }
        public Map Map { get; set; }
        public CourseData CourseData { get; set; }
    }
}
