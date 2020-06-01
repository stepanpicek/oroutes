using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OBPostupy.Models.GPS
{
    /// <summary>
    /// Data model for Location
    /// </summary>
    public class Location
    {
        public long ID { get; set; }
        [JsonIgnore]
        public string CoordsString { get; set; }
        public DateTime? Time { get; set; }
        public double? Elevation { get; set; }
        [JsonIgnore]
        public int PathID { get; set; }
        [JsonIgnore]
        public Path Path { get; set; }

        [NotMapped]
        public Tuple<double, double> Position
        {
            get
            {
                string[] tab = CoordsString.Split(';');
                return Tuple.Create(double.Parse(tab[0]), double.Parse(tab[1]));
            }
            set
            {
                CoordsString = string.Format("{0};{1}", value.Item1, value.Item2);
            }
        }
    }
}
