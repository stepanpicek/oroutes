using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OBPostupy.Models.People;
using OBPostupy.Models.Results;
using OBPostupy.Models.Races;
using OBPostupy.Models.GPS;

namespace OBPostupy.ViewModels
{
    public class PersonResultsDetailViewModel
    {
        public Person Person { get; set; }
        public Result Result { get; set; }
        public Category Category { get; set; }
        public List<SplitTime> splitTimes { get; set; }
        public PersonResult PersonResult { get; set; }
        public Path Path { get; set; }

    }
}
