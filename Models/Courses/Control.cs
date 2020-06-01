using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupy.Models.Courses
{
    /// <summary>
    /// Data model of Control
    /// </summary>
    public class Control
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string CoordsString { get; set; }
        public int? CourseDataID { get; set; }
        public CourseData CourseData { get; set; }
        public List<CourseControl> CourseControl { get;}
        public List<Split> SplitsFirstControl { get; }
        public List<Split> SplitsSecondControl { get; }

        [NotMapped]
        public Tuple<double, double> Coordinates
        {
            get
            {
                string[] tab = CoordsString.Split(';');
                return Tuple.Create(double.Parse(tab[0]), double.Parse(tab[1]));
            }
            set => CoordsString = string.Format("{0};{1}", value.Item1, value.Item2);
        }
    }
}
