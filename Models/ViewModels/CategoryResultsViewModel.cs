using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OBPostupy.Models.Races;
using OBPostupy.Models.Results;

namespace OBPostupy.ViewModels
{
    public class CategoryResultsViewModel
    {
        public Category Category { get; set; }
        public List<PersonResult> SortedResults { get; set; }        
    }
}
