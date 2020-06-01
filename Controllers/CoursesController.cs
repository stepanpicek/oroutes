using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OBPostupy.Data;
using OBPostupy.ViewModels;
using OBPostupy.Extensions.Messages;
using OBPostupy.Models.Courses;
using OBPostupy.Models.Races;
using OBPostupy.Models.Results;
using System.Text.Json;
using System.Text.Json.Serialization;
using EFCore.BulkExtensions;

namespace OBPostupy.Controllers
{
    public class CoursesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public CoursesController(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        /// <summary>
        /// File upload action for course data
        /// </summary>
        /// <param name="id">ID of race</param>
        /// <param name="file">File data</param>
        /// <returns>Redirect to setting of race</returns>
        [HttpPost]
        public async Task<IActionResult> UploadCourses(int? id, IFormFile file)
        {
            if (id == null  || file == null)
            {
                return NotFound();
            }

            var race = await _context.Races.SingleOrDefaultAsync(r => r.ID == id);

            if (race == null)
            {
                return NotFound();
            }

            if (file.Length > 0)
            {
                var courseFilePath = Path.GetTempFileName();

                using (var stream = System.IO.File.Create(courseFilePath))
                {
                    await file.CopyToAsync(stream);
                }
                CourseReader courseReader = new CourseReader(courseFilePath);
                courseReader.Read();
                CourseData courseData = new CourseData {
                       Controls = courseReader.Controls,
                       Splits = courseReader.Splits,
                       Courses = courseReader.Courses,
                       Race = race
                };
                _context.CourseData.Add(courseData);
                _context.CourseControls.AddRange(courseReader.CourseControls);
                _context.CourseSplits.AddRange(courseReader.CourseSplits);
                await _context.SaveChangesAsync();
                return RedirectToAction("Setting", "Races", new { id = id }).Success("Tratě a kontroly nahrány.");

            }
            return RedirectToAction("Setting", "Races", new { id = id }).Danger("Tratě a kontroly NEbyly nahrány.");
        }

        /// <summary>
        /// Action for delete course data
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Page with comfirmation</returns>

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var race = await GetSingleRaceAsync(r => r.ID == id);
            if (race == null)
            {
                return NotFound();
            }
            
            return View();
        }

        /// <summary>
        /// Delete confirmation 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var race = await GetSingleRaceAsync(r => r.ID == id);
            if (race == null)
            {
                return NotFound();
            }

            try
            {
                var splits = _context.Splits.Where(c => c.CourseData == race.CourseData).ToList();
                _context.Splits.RemoveRange(splits);
                await _context.SaveChangesAsync();

                var controls = _context.Controls.Where(c => c.CourseData == race.CourseData).ToList();
                _context.Controls.RemoveRange(controls);
                await _context.SaveChangesAsync();

                var courses = _context.Courses.Where(c => c.CourseData == race.CourseData).ToList();
                _context.Courses.RemoveRange(courses);
                _context.CourseData.Remove(race.CourseData);
                await _context.SaveChangesAsync();
                return RedirectToAction("Setting", "Races", new { id = id }).Success("Tratě a kontroly byly smazány");

            }
            catch (Exception)
            {
                return RedirectToAction("Setting", "Races", new { id = id }).Danger("Tratě a kontroly NEbyly smazány, nastal nějaký problém.");
            }

        }

        /// <summary>
        /// Action for returning a serialized course in JSON
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Course in JSON</returns>
        [HttpGet]
        public async Task<IActionResult> GetCourse(int? id)
        {
            
            if(id == null)
            {
                return NotFound();
            }

            var course = await GetSingleCourseWithControlsAsync(c => c.ID == id);

            if (course == null)
            {
                return NotFound();
            }

            var options = new Newtonsoft.Json.JsonSerializerSettings
            {
                MaxDepth = 3,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };
            return Json(course, options);
        }

    }
}