using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using OBPostupy.Models.Results;
using System.Drawing;
using Newtonsoft.Json;

namespace OBPostupy.Models.GPS
{
    /// <summary>
    /// Data model for Path
    /// </summary>
    public class Path
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Location> Locations { get; set; }
        [JsonIgnore]
        public PersonResult PersonResult { get; set; }

        [NotMapped]
        public List<Path> MatchWith { get; } = new List<Path>();
        [NotMapped, JsonIgnore]
        public bool hasAnalysis { get; set; } = false;
    }
}
