using Microsoft.AspNetCore.Mvc.Rendering;
using OBPostupy.Models.Maps;
using OBPostupy.Models.Races;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupy.ViewModels
{
    public class RaceDetailViewModel
    {
        public Race Race { get; set; }
        public Map Map { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public bool HasStravaAccount { get; set; }
    }
}
