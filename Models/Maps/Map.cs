using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupy.Models.Maps
{
    /// <summary>
    /// Data model of Map
    /// </summary>
    public class Map
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string PathToFile { get; set; }
        public int Scale { get; set; }
        public int? RaceID { get; set; }
        public Races.Race Race { get; set; }
        public string Corners { get; set; }
        public double? Rotation { get; set; }
        [NotMapped]
        public List<Tuple<double, double>> MapCorners {
            get {

                string[] data = Corners.Split(';');
                var coorners = new List<Tuple<double, double>>();
                foreach (var coordinates in data)
                {
                    string[] coordinate = coordinates.Split('|');
                    double lat;
                    double lon;
                    if (coordinate.Length == 2 && double.TryParse(coordinate[0], out lat) && double.TryParse(coordinate[1], out lon))
                    {
                        coorners.Add(Tuple.Create(lat, lon));
                    }
                }
                return coorners;
            }
            set {
                string data = "";
                foreach(var coord in value)
                {
                    data += coord.Item1 + "|" + coord.Item2+";";
                }
                Corners = data;
            } }
    }
}
