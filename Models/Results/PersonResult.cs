using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OBPostupy.Models.Races;
using OBPostupy.Models.People;
using OBPostupy.Models.GPS;

namespace OBPostupy.Models.Results
{
    /// <summary>
    /// Data model for PersonResult
    /// </summary>
    public class PersonResult
    {
        public int ID { get; set; }
        public int PersonID { get; set; }
        public Person Person { get; set; }
        public int RaceID { get; set; }
        public Races.Race Race { get; set; }
        public int CategoryID { get; set; }
        public Category Category { get; set; }
        public int? PathID { get; set; }
        public Path Path { get; set; }
        public Result Result { get; set; }    
    }
}
