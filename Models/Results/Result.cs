using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupy.Models.Results
{
    /// <summary>
    /// Data model for Result
    /// </summary>
    public class Result
    {
        public int ID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public int Position { get; set; }
        public string Status { get; set; }
        public List<SplitTime> SplitTimes { get; set; }
        public int PersonResultID { get; set; }
        public PersonResult PersonResult { get; set; }
    }
}
