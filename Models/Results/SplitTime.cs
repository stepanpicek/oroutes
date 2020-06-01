using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OBPostupy.Models.Courses;
using OBPostupy.Models.GPS;

namespace OBPostupy.Models.Results
{
    /// <summary>
    /// Data model for SplitTime
    /// </summary>
    public class SplitTime
    {
        public int ID { get; set; }
        public int? SplitID { get; set; }
        public Split Split { get; set; }
        public DateTime Time { get; set; }
        public int TimeSpan { get; set; }
        public string Code { get; set; }
        public int ResultID { get; set; }
        public Result Result { get; set; }
    }
}
