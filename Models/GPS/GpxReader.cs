using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Xml.Linq;
using System.Globalization;
using System.Xml;

namespace OBPostupy.Models.GPS
{
    /// <summary>
    /// Class for reading a GPX file by LINQ
    /// </summary>
    public class GpxReader
    {
        private Path path = null;
        private string filePath;
        public GpxReader(string filePath)
        {
            this.filePath = filePath;
        }

        /// <summary>
        ///  The main method of reading a file
        /// </summary>
        /// <returns>An instance of the Path class that contains the data of the read GPX file</returns>
        public Path Read()
        {
            DeseralizerByLINQ();
            return path;            
        }

        // Use LINQ to read a GPX file
        private void DeseralizerByLINQ()
        {
            XElement doc;
            try
            {
                doc = XElement.Load(filePath);
            }
            catch (FileNotFoundException) { return; }

            path = new Path
            {
                Locations = new List<Location>()
            };

            var trackpoints = from trkpt in doc.Descendants() where trkpt.Name.LocalName == "trkpt" select trkpt;

            double latOld = 0, lonOld = 0;
            foreach (var trkpt in trackpoints)
            {
                
                double lat, lon;
                bool checkLat = double.TryParse(trkpt.Attributes().Where(n => n.Name.LocalName == "lat").FirstOrDefault().Value, NumberStyles.Any, CultureInfo.InvariantCulture, out lat);
                bool checkLon = double.TryParse(trkpt.Attributes().Where(n => n.Name.LocalName == "lon").FirstOrDefault().Value, NumberStyles.Any, CultureInfo.InvariantCulture, out lon);
                if (checkLat && checkLon)
                {
                    if (lat == latOld && lon == lonOld)
                    {
                        continue;
                    }
                    Location location = new Location();
                    latOld = lat;
                    lonOld = lon;
                    location.Position = Tuple.Create(lat,lon);
                    location.Time = GetTime(trkpt);
                    location.Elevation = GetElevation(trkpt);
                    location.Path = path;
                    path.Locations.Add(location);
                }
            }
        }

        //Determining the time from the trackpoint
        private DateTime? GetTime(XElement trkpt)
        {
            var time = trkpt.Elements().Where(n => n.Name.LocalName == "time").FirstOrDefault();
            if(time != null)
            {
                try
                {
                    DateTime timespan = DateTime.Parse(time.Value);
                    return timespan;
                }
                catch(ArgumentNullException) { }
                catch (FormatException) { }
            }

            return null;
        }

        //Determined altitude from the trackpoint
        private double? GetElevation(XElement trkpt)
        {
            var elevation = trkpt.Elements().Where(n => n.Name.LocalName == "ele").FirstOrDefault();
            if (elevation != null)
            {
                double elevNum;
                bool isEleNum = Double.TryParse(elevation.Value, out elevNum);
                if (isEleNum)
                {
                    return elevNum;
                }
                
            }

            return null;
        }

    }
}
