using OBPostupy.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupy.Models.GPS
{
    public class PathAnalysis:GPSAnalysis
    {

        /// <summary>
        /// Create interpolation in Path data by distance between each location
        /// </summary>
        /// <param name="locations"></param>
        /// <param name="distanceInMeters"></param>
        /// <returns></returns>
        public List<Location> InterpolationByDistance(List<Location> locations, double distanceInMeters)
        {
            if (locations == null || locations.Count < 2) 
                return null;

            var sortedLocations = locations.OrderBy(l => l.Time).ToList();
            List<Location> finalLocations = new List<Location>();
            Location actualLocation = sortedLocations[0];
            finalLocations.Add(actualLocation);

            foreach (var location in sortedLocations)
            {
                if (actualLocation == location) continue;
                if (location.Time != null && actualLocation.Time != null)
                {
                    var distance = GetDistanceInMeters(actualLocation.Position, location.Position);
                    if (distance < distanceInMeters) continue;

                    while (distance > distanceInMeters)
                    {
                        Location interpolated = GetInterpolatedLocationByDistance(actualLocation, location, distanceInMeters);
                        if (interpolated != null)
                        {
                            finalLocations.Add(interpolated);
                        }
                        actualLocation = interpolated;
                        distance = GetDistanceInMeters(actualLocation.Position, location.Position);
                    }

                }
            }
            if (sortedLocations.Last() != null)
                finalLocations.Add(sortedLocations.Last());
            return finalLocations;
        }

        //Create and return location with interpolated data
        private Location GetInterpolatedLocationByDistance(Location firstPoint, Location secondPoint, double distance)
        {
            if (firstPoint.Position != null && secondPoint.Position != null)
            {
                double diffDistance = GetDistanceInMeters(firstPoint.Position, secondPoint.Position);
                double diffLat = (secondPoint.Position.Item1 - firstPoint.Position.Item1) / diffDistance;
                double diffLon = (secondPoint.Position.Item2 - firstPoint.Position.Item2) / diffDistance;
                double newLat = firstPoint.Position.Item1 + (diffLat * distance);
                double newLon = firstPoint.Position.Item2 + (diffLon * distance);
                Location newLocation = new Location
                {
                    Position = Tuple.Create(newLat, newLon),
                };

                if (firstPoint.Elevation != null && secondPoint.Elevation != null)
                {
                    double diffEle = (double)(secondPoint.Elevation - firstPoint.Elevation) / diffDistance;
                    double newEle = (double)firstPoint.Elevation + (distance * diffEle);
                    newLocation.Elevation = newEle;
                }
                if (firstPoint.Time != null && secondPoint.Time != null)
                {
                    TimeSpan diffTime = (TimeSpan)(secondPoint.Time - firstPoint.Time);
                    double diffTimeOne = (diffTime.TotalSeconds / diffDistance) * distance;
                    DateTime firstPointTime = (DateTime)firstPoint.Time;
                    DateTime newTime = firstPointTime.AddSeconds(diffTimeOne);
                    newLocation.Time = newTime;
                }
                return newLocation;
            }
            return null;
        }

        /// <summary>
        ///  Create interpolation in Path data by time between each location
        /// </summary>
        /// <param name="locations"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public List<Location> InterpolationByTime(List<Location> locations, int seconds)
        {
            if (locations == null || locations.Count < 2) 
                return null;

            var sortedLocations = locations.OrderBy(l => l.Time).ToList();
            List<Location> finalLocations = new List<Location>();
            Location actualLocation = sortedLocations[0];
            finalLocations.Add(actualLocation);

            foreach (var location in sortedLocations)
            {
                if (actualLocation == location) continue;
                if (location.Time != null && actualLocation.Time != null)
                {
                    TimeSpan diff = (TimeSpan)(location.Time - actualLocation.Time);
                    if (diff.TotalSeconds < seconds) continue;
                    if (diff.TotalSeconds == seconds)
                    {
                        finalLocations.Add(location);
                        actualLocation = location;
                        continue;
                    }

                    while (diff.TotalSeconds >= seconds)
                    {
                        Location interpolated = GetInterpolatedLocationByTime(actualLocation, location, seconds);
                        if (interpolated != null)
                        {
                            finalLocations.Add(interpolated);
                            actualLocation = interpolated;
                            diff = (TimeSpan)(location.Time - actualLocation.Time);
                        }
                    }



                }
            }
            return finalLocations;
        }

        //Create and return location with interpolated data by time
        private Location GetInterpolatedLocationByTime(Location firstPoint, Location secondPoint, int seconds)
        {
            if (firstPoint.Time != null && secondPoint.Time != null)
            {
                TimeSpan diff = (TimeSpan)(secondPoint.Time - firstPoint.Time);
                DateTime firstPointTime = (DateTime)firstPoint.Time;
                double diffLat = (secondPoint.Position.Item1 - firstPoint.Position.Item1) / diff.TotalSeconds;
                double diffLon = (secondPoint.Position.Item2 - firstPoint.Position.Item2) / diff.TotalSeconds;
                double newLat = firstPoint.Position.Item1 + (diffLat * seconds);
                double newLon = firstPoint.Position.Item2 + (diffLon * seconds);
                Location newLocation = new Location
                {
                    Position = Tuple.Create(newLat, newLon),
                    Time = firstPointTime.AddSeconds(seconds)
                };

                if (firstPoint.Elevation != null && secondPoint.Elevation != null)
                {
                    double diffEle = (double)(secondPoint.Elevation - firstPoint.Elevation) / diff.TotalSeconds;
                    double newEle = (double)firstPoint.Elevation + (seconds * diffEle);
                    newLocation.Elevation = newEle;
                }

                return newLocation;
            }
            return null;
        }

        /// <summary>
        /// Set time ofset to all locations
        /// </summary>
        /// <param name="locations"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public List<Location> SetOffset(List<Location> locations, double seconds)
        {
            if (locations == null) return null;
            foreach (var location in locations)
            {
                if (location.Time != null)
                {
                    DateTime time = (DateTime)location.Time;
                    location.Time = time.AddSeconds(seconds);
                }
            }
            return locations;
        }

        /// <summary>
        /// Algorithm for determining path controls
        /// </summary>
        /// <param name="splitTimes"></param>
        /// <param name="locations"></param>
        /// <returns></returns>
        public double GetOffsetFromSplits(List<SplitTime> splitTimes, List<Location> locations)
        {
            if (splitTimes == null || locations == null) return 0;
            var orderedSplitTimes = splitTimes.OrderBy(st => st.Time).ToList();
            List<int> controlIndexes = new List<int>();
            var firstControl = orderedSplitTimes.First();
            controlIndexes.Add(0);
            if (firstControl != null)
            {
                for (int i = 1; i < orderedSplitTimes.Count; i++)
                {
                    controlIndexes.Add((int)(orderedSplitTimes[i].Time - firstControl.Time).TotalSeconds);
                }
            }

            int closerIndex = 0;
            double averageDistance = Int32.MaxValue;

            for (int i = 0; i < locations.Count; i++)
            {
                double distance = 0;
                int controlsCount = 0;
                bool isEnd = false;
                for (int j = 0; j < controlIndexes.Count; j++)
                {
                    if (controlIndexes[j] + i >= locations.Count)
                    {
                        isEnd = true;
                        break;
                    }

                    distance += GetDistanceInMeters(orderedSplitTimes[j].Split.SecondControl.Coordinates, locations[controlIndexes[j] + i].Position);
                    controlsCount++;
                }
                if (isEnd) break;
                if (controlsCount > 0 && distance / controlsCount < averageDistance)
                {
                    closerIndex = i;
                    averageDistance = distance / controlsCount;
                }
            }

            return ((TimeSpan)(firstControl.Time - locations[closerIndex].Time)).TotalSeconds;
        }

        /// <summary>
        /// Return location with same time
        /// </summary>
        /// <param name="path"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public int GetLocationIndex(Path path, DateTime time)
        {
            if (path == null || time == null) return -1;
            for (int i = 0; i < path.Locations.Count; i++)
            {
                if (Math.Abs(((TimeSpan)(path.Locations[i].Time - time)).TotalSeconds) < 1)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
