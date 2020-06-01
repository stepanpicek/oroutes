using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OBPostupy.Models.Races;
using OBPostupy.Models.Results;


namespace OBPostupy.ViewModels
{
    public class RaceSettingViewModel
    {
        public Race Race { get; set; }
        public int OrisEventID { get; set; }

        public bool IsCategoriesUploaded { get; set; }
        public bool IsResultsUploaded { get; set; }
        public bool IsMapUploaded { get; set; }
        public bool IsCoursesUploaded { get; set; }

    }
}
