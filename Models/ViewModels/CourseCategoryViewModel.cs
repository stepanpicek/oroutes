using Microsoft.AspNetCore.Mvc.Rendering;
using OBPostupy.Models.Races;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupy.ViewModels
{
    public class CourseCategoryViewModel
    {
        public List<SelectListItem> Courses { get; set; }
        public string CourseID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
    }
}
