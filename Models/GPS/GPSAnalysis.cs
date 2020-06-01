using OBPostupy.Models.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupy.Models.GPS
{
    public class GPSAnalysis
    {
        /// <summary>
        /// Harvesin's formula for calculating the distance between two coordinates
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public double GetDistanceInMeters(Tuple<double, double> first, Tuple<double, double> second)
        {
            if (first == null || second == null) return 0;

            var earthRadius = 6371000;

            var dLat = DegreesToRadians(second.Item1 - first.Item1);
            var dLon = DegreesToRadians(second.Item2 - first.Item2);

            var lat1 = DegreesToRadians(first.Item1);
            var lat2 = DegreesToRadians(second.Item1);
            var a = Math.Pow(Math.Sin(dLat / 2),2) + Math.Pow(Math.Sin(dLon / 2), 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var completeDist = earthRadius * c;
            return completeDist;
        }

        private double DegreesToRadians(double degree)
        {
            return degree * Math.PI / 180;
        }
        
    }
}
