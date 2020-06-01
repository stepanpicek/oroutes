using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using SharpKml.Engine;
using SharpKml.Dom;
using System.Diagnostics;

namespace OBPostupy.Models.Maps
{

    /// <summary>
    /// Class for reading map image
    /// </summary>
    public class MapReader
    {
        private Map map;
        private string path = null;
        private string completePath = null;
        private string rootPath = null;
        private string[] imgExtensions = { ".txt", ".pdf" };
        private string[] gisExtensions = { ".kmz" };
        private string extension = null;

        public MapReader(string rootPath, string tempPath, string extension)
        {
            this.path = tempPath;
            this.rootPath = GetUploadDirectory(rootPath);
            this.extension = extension;

        }

        /// <summary>
        /// Main method for reading data
        /// </summary>
        /// <returns></returns>
        public Map Read()
        {
            if (File.Exists(path))
            {
                if (string.IsNullOrEmpty(extension))
                    return null;
                if (imgExtensions.Contains(extension))
                {
                    ReadImageFile();
                }
                else if (gisExtensions.Contains(extension))
                {
                    ReadGISFile();
                }
            }

            return map;
        }

        private void ReadImageFile()
        {

        }

        /// Read file in KMZ and KML format
        private void ReadGISFile()
        {
            string pathToimage = null;
            using (FileStream reader = new FileStream(path, FileMode.Open))
            using (KmzFile kmzFile = KmzFile.Open(reader))
            {
                KmlFile kmlFile = kmzFile.GetDefaultKmlFile();
                var image = kmlFile.Root.Flatten().OfType<GroundOverlay>().FirstOrDefault();
                if (image != null)
                {
                    pathToimage = image.Icon.Href.ToString();
                }
                var latLonBox = kmlFile.Root.Flatten().OfType<LatLonBox>().FirstOrDefault();
                if (latLonBox != null)
                {
                    var coorners = GetMapCorners(latLonBox);
                     map = new Map()
                    {
                        MapCorners = coorners,
                        Rotation = latLonBox.Rotation,
                        Name = kmlFile.Root.Flatten().OfType<Folder>().FirstOrDefault().Name

                    };
                }
            }

            ReadArchiveAndExport(pathToimage);
        }

        // Read archive of KMZ file and save to upload folder
        private void ReadArchiveAndExport(string pathToimage)
        {
            using (ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read))
            {
                if (pathToimage != null && map != null)
                {
                    ZipArchiveEntry entry = archive.GetEntry(pathToimage);
                    string name = Path.GetRandomFileName() + "." + entry.Name;
                    while (File.Exists(Path.Combine(rootPath, name)))
                    {
                        name = Path.GetRandomFileName() + "." + entry.Name;
                    }
                    ZipFileExtensions.ExtractToFile(entry, Path.Combine(rootPath, name));
                    map.PathToFile = Path.Combine(completePath, name);
                }
            }
        }


        private string GetUploadDirectory(string root)
        {
            completePath = Path.Combine("uploads", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
            var uploadPath = Path.Combine(root, completePath);
            DirectoryInfo di = Directory.CreateDirectory(uploadPath);
            Debug.WriteLine("Get Upload Directory: " + uploadPath);
            return uploadPath;
        }

        private List<Tuple<double,double>> GetMapCorners(LatLonBox latLonBox)
        {
            if (latLonBox != null)
            {
                if (latLonBox.North != null &&
                    latLonBox.South != null &&
                    latLonBox.East != null &&
                    latLonBox.West != null)
                {
                    if (latLonBox.Rotation != null)
                    {
                        return GetCornersWithRotation(latLonBox);
                    }

                    return GetCornersWithoutRotation(latLonBox);
                }
            }
            return null;
        }   

        // Rotation of corners of map
        private List<Tuple<double, double>> GetCornersWithRotation(LatLonBox latLonBox)
        {
            double n = (double)latLonBox.North;
            double s = (double)latLonBox.South;
            double e = (double)latLonBox.East;
            double w = (double)latLonBox.West;
            double rotation = (double)latLonBox.Rotation;

            double a = (e + w) / 2.0;
            double b = (n + s) / 2.0;
            double squish = Math.Cos(DegToRad(b));
            double x = squish * (e - w) / 2.0;
            double y = (n - s) / 2.0;

            double X, Y;
            List<Tuple<double, double>> corners = new List<Tuple<double, double>>();
            X = b - (x * Math.Sin(DegToRad(rotation)) - y * Math.Cos(DegToRad(rotation)));
            Y = a - (x * Math.Cos(DegToRad(rotation)) + y * Math.Sin(DegToRad(rotation))) / squish;
            corners.Add(Tuple.Create(X, Y));
            X = b + (x * Math.Sin(DegToRad(rotation)) + y * Math.Cos(DegToRad(rotation)));
            Y = a + (x * Math.Cos(DegToRad(rotation)) - y * Math.Sin(DegToRad(rotation))) / squish;
            corners.Add(Tuple.Create(X, Y));
            X = b - (x * Math.Sin(DegToRad(rotation)) + y * Math.Cos(DegToRad(rotation)));
            Y = a - (x * Math.Cos(DegToRad(rotation)) - y * Math.Sin(DegToRad(rotation))) / squish;
            corners.Add(Tuple.Create(X, Y));
            X = b + (x * Math.Sin(DegToRad(rotation)) - y * Math.Cos(DegToRad(rotation)));
            Y = a + (x * Math.Cos(DegToRad(rotation)) + y * Math.Sin(DegToRad(rotation))) / squish;
            corners.Add(Tuple.Create(X, Y));
            X = b - (x * Math.Sin(DegToRad(rotation)) - y * Math.Cos(DegToRad(rotation)));
            Y = a - (x * Math.Cos(DegToRad(rotation)) + y * Math.Sin(DegToRad(rotation))) / squish;
            corners.Add(Tuple.Create(X, Y));;            
            return corners;
        }

        private List<Tuple<double, double>> GetCornersWithoutRotation(LatLonBox latLonBox)
        {            
            List<Tuple<double, double>> corners = new List<Tuple<double, double>> ();
            corners.Add(Tuple.Create((double)latLonBox.North, (double)latLonBox.West));
            corners.Add(Tuple.Create((double)latLonBox.North, (double)latLonBox.East));
            corners.Add(Tuple.Create((double)latLonBox.South, (double)latLonBox.East));
            corners.Add(Tuple.Create((double)latLonBox.South, (double)latLonBox.West));
            corners.Add(Tuple.Create((double)latLonBox.North, (double)latLonBox.West));
            return corners;
        }

        private double DegToRad(double angle)
        {
            return (Math.PI / 180) * angle;
        }
    }
}
