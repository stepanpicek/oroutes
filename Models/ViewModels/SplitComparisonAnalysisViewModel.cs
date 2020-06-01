using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json;

namespace OBPostupy.Models.GPS
{
    public class SplitComaprisonAnalysisViewModel
    {
        public List<Location> Locations { get; set; }
        public List<SegmentAnalysisViewModel> SegmentAnalyses { get; set; }
        public double PathDistance { get; set; } = 0;
        public double Elevation { get; set; } = 0;
        public double Descent { get; set; } = 0;

        [JsonIgnore]
        public Color Color { get; set; }
        public string HexColor
        {
            get{
                if (Color == null) return "#000000";
                return "#" + Color.R.ToString("X2") + Color.G.ToString("X2") + Color.B.ToString("X2");
            }
        }
    }
}
