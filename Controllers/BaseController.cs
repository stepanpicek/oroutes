using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OBPostupy.Data;
using OBPostupy.Models.Courses;
using OBPostupy.Models.GPS;
using OBPostupy.Models.Maps;
using OBPostupy.Models.People;
using OBPostupy.Models.Races;
using OBPostupy.Models.Results;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace OBPostupy.Controllers
{
    /// <summary>
    /// Base abstract class
    /// Contains methods for retrieving data using EF
    /// </summary>
    public abstract class BaseController : Controller
    {
        private readonly ApplicationDbContext _context;
        protected readonly PathAnalysis pathAnalysis = new PathAnalysis();
        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a single race object with related categories and courses
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>Task with Race object</returns>
        protected async Task<Race> GetSingleRaceAsync(Expression<Func<Race, bool>> predicate)
        {
            var race = await _context.Races.Where(predicate)
                            .Include(r => r.Categories)
                                .ThenInclude(c => c.Course)
                            .SingleOrDefaultAsync();
            return race;
        }

        /// <summary>
        /// Get a single PersonResults object with related results and split tímes
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>Task with PersonResult object</returns>
        protected async Task<PersonResult> GetSinglePersonResultAsync(Expression<Func<PersonResult, bool>> predicate)
        {
            var personResult = await _context.PersonResults.Where(predicate)
                                              .Include(pr => pr.Result)
                                                .ThenInclude(r => r.SplitTimes)
                                               .SingleOrDefaultAsync();
            return personResult;
        }

        /// <summary>
        /// Get a single Path object with related locations
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>Task with Path object</returns>
        protected async Task<Path> GetSinglePathAsync(Expression<Func<Path, bool>> predicate)
        {
            var path = await _context.Paths.Where(predicate)
                            .Include(p => p.Locations)
                            .SingleOrDefaultAsync();
            return path;
        }

        /// <summary>
        /// Get a signle categorii with related race, course data and results
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        protected async Task<Category> GetSingleCategoryAsync(Expression<Func<Category, bool>> predicate)
        {
            var category = await _context.Categories.Where(predicate)
                            .Include(c => c.Race)
                                .ThenInclude(r => r.CourseData)
                                    .ThenInclude(cd => cd.Courses)
                            .Include(c => c.PersonResults)
                                .ThenInclude(pr => pr.Result)
                                    .ThenInclude(r => r.SplitTimes)
                            .SingleOrDefaultAsync();
            return category;
        }

        /// <summary>
        /// Get all categories with related race and results
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>Task with List<Category> </returns>
        protected async Task<List<Category>> GetAllCategoriesAsync(Expression<Func<Category, bool>> predicate)
        {
            var categories = await _context.Categories.Where(predicate)
                            .Include(c => c.Race)
                            .Include(c => c.PersonResults)
                                .ThenInclude(pr => pr.Result)
                                    .ThenInclude(r => r.SplitTimes)
                            .ToListAsync();
            return categories;
        }

        /// <summary>
        /// Get a single course with related controls
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>Task with course</returns>
        protected async Task<Course> GetSingleCourseWithControlsAsync(Expression<Func<Course, bool>> predicate)
        {
            var course = await _context.Courses.Where(predicate)
                                 .Include(c => c.CourseControl)
                                     .ThenInclude(cc => cc.Control)
                                 .SingleOrDefaultAsync();
            return course;
        }


        /// <summary>
        /// Get a path trimmed by results
        /// </summary>
        /// <param name="path"></param>
        /// <param name="splitTime"></param>
        /// <returns>List with locations</returns>
        protected List<Location> GetSplitPath(Path path, SplitTime splitTime)
        {
            int startIndex = pathAnalysis.GetLocationIndex(path, splitTime.Time.AddSeconds(-splitTime.TimeSpan));
            int finishIndex = pathAnalysis.GetLocationIndex(path, splitTime.Time);
            var interpolationByTime = pathAnalysis.InterpolationByTime(path.Locations, 1);

            List<Location> splitPath = null;
            if (startIndex < 0) 
                startIndex = 0;

            if (finishIndex < 0) 
                finishIndex = interpolationByTime.Count - 1;

            if (startIndex < finishIndex)
            {
                splitPath = interpolationByTime.GetRange(startIndex, finishIndex - startIndex);
            }

            return splitPath;
        }

        /// <summary>
        /// Get a single split time with related results and split controls
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>Task with SplitTime object</returns>
        protected async Task<SplitTime> GetSingleSplitTimeAsync(Expression<Func<SplitTime, bool>> predicate)
        {
            var splitTime = await _context.SplitTimes.Where(predicate)
                                .Include(st => st.Result)
                                    .ThenInclude(r => r.PersonResult)
                                .Include(st => st.Split)
                                    .ThenInclude(s => s.FirstControl)
                                .Include(st => st.Split)
                                    .ThenInclude(s => s.SecondControl)
                                .SingleOrDefaultAsync();
            return splitTime;
        }

        /// <summary>
        /// Get all locations for split
        /// </summary>
        /// <param name="personResult"></param>
        /// <param name="id"></param>
        /// <returns>Task with list of locations</returns>
        protected async Task<List<Location>> GetSplitLocationsForComparison(PersonResult personResult, int? id)
        {
            SplitTime splitTime = personResult.Result.SplitTimes.Where(st => st.SplitID == id).SingleOrDefault();
            if (splitTime == null)
            {
                return null;
            }

            Path path = await GetSinglePathAsync(p => p.ID == personResult.PathID);
            if (path == null)
            {
                return null;
            }

            List<Location> SplitSegment = pathAnalysis.InterpolationByDistance(GetSplitPath(path, splitTime), 5);
            if (SplitSegment == null)
            {
                return null;
            }

            return SplitSegment;
        }
    }
}
