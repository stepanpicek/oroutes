using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;


namespace OBPostupy.Models.Courses
{
    /// <summary>
    /// Class for reading course data
    /// </summary>
    public class CourseReader
    {
          
        public CourseData CourseData { get; set; }
        public List<Course> Courses { get; set; }
        public List<Control> Controls { get; set; }
        public List<CourseControl> CourseControls { get; set; }
        public List<Split> Splits { get; set; }
        public List<CourseSplit> CourseSplits { get; set; }

        private string pathToDocument;

        public CourseReader(string pathToDocument)
        {
            Courses = new List<Course>();
            Controls = new List<Control>();
            CourseControls = new List<CourseControl>();
            Splits = new List<Split>();
            CourseSplits = new List<CourseSplit>();
            this.pathToDocument = pathToDocument;
        }

        /// <summary>
        /// Main method for reading
        /// </summary>
        public void Read()
        {
            if (File.Exists(pathToDocument))
            {
                var courseData = parseCourseData(pathToDocument);
                parseRaceCourseData(courseData.RaceCourseData);
                setSplits();
            }

        }

        //Deserialization of data into an object of type CourseData determined by the schema IOF XML v. 3.0
        private IOFstandard.CourseData parseCourseData(string data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(IOFstandard.CourseData));
            object courseData = null;
            using (TextReader reader = new StreamReader(data))
            {
                courseData = serializer.Deserialize(reader);
            }

            return (IOFstandard.CourseData)courseData;
        }

        //Prevedeni deserializovanych objektu do nových instanci typu datoveho modelu
        private void parseRaceCourseData(IOFstandard.RaceCourseData[] coursesData)
        {
            foreach(var courseData in coursesData)
            {
                foreach(var control in courseData.Control)
                {
                    var coords = control.Position != null ? Tuple.Create(control.Position.lat, control.Position.lng) : null;
                    Control cnt = new Control
                    {
                        Code = control.Id.Value,
                        Coordinates = coords,
                    };
                    Controls.Add(cnt);
                }

                CreateCourses(courseData.Course);
               
            }
        }

        //Create all courses from data
        private void CreateCourses(IOFstandard.Course[] courses)
        {
            foreach (var course in courses)
            {
                Course crs = new Course
                {
                    Name = course.Name,
                };
                Courses.Add(crs);
                int i = 0;
                foreach (var courseControl in course.CourseControl)
                {
                    Control control = Controls.FirstOrDefault(c => c.Code == courseControl.Control.FirstOrDefault());
                    if (control != null)
                    {
                        CourseControl cc = new CourseControl
                        {
                            Control = control,
                            Course = crs,
                            Order = i,
                            Type = courseControl.type.ToString()
                        };
                        CourseControls.Add(cc);
                        i++;
                    }

                }
            }
        }

        //Determining the splits
        private void setSplits()
        {
            foreach (var course in Courses)
            {
                var courseControls = CourseControls.FindAll(cc => cc.Course == course).OrderBy(cc => cc.Order).ToList();
                for (int i = 0; i < courseControls.Count() - 1; i++)
                {
                    Control c1 = courseControls[i].Control;
                    Control c2 = courseControls[i + 1].Control;
                    var split = Splits.FirstOrDefault(s => s.FirstControl == c1 && s.SecondControl == c2);
                    if (split == null)
                    {
                        split = new Split
                        {
                            FirstControl = c1,
                            SecondControl = c2
                        };
                        Splits.Add(split);
                    }
                    CourseSplit courseSplit = new CourseSplit 
                    {
                        Course = course,
                        Split = split,
                        Order = i
                    };
                    CourseSplits.Add(courseSplit);
                }
            }
        }
    }
}
