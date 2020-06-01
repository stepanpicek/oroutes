using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupy.Models.GPS
{
    public class PathAnalysisViewModel
    {
        public double AverageSpeed { get; set; } = 0;
        public double AverageTempo { get; set; } = 0;
        public double PathDistance { get; set; } = 0;
        public double Elevation { get; set; } = 0;
        public double Descent { get; set; } = 0;
        public TimeSpan Time { get; set; }

        public PathAnalysisViewModel()
        {

        }
        protected PathAnalysisViewModel(PathAnalysisViewModel pathAnalysis)
        {
            if(pathAnalysis != null)
            {
                AverageSpeed = pathAnalysis.AverageSpeed;
                AverageTempo = pathAnalysis.AverageTempo;
                PathDistance = pathAnalysis.PathDistance;
                Elevation = pathAnalysis.Elevation;
                Descent = pathAnalysis.Descent;
                Time = pathAnalysis.Time;
            }
        }
    }
}
