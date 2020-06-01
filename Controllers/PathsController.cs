using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OBPostupy.Data;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;
using OBPostupy.Extensions.Messages;
using System.IO;
using OBPostupy.Models;
using OBPostupy.Models.GPS;
using System.Diagnostics;
using OBPostupy.Models.Results;
using Microsoft.AspNetCore.Identity;
namespace OBPostupy.Controllers
{
    public class PathsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public PathsController(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(int? raceID, int? competitorID, IFormFile file)
        {
            if (raceID == null || competitorID == null)
            {
                return NotFound();
            }
            var race = await _context.Races.SingleOrDefaultAsync(r => r.ID == raceID);
            var competitor = await _context.Person.SingleOrDefaultAsync(r => r.ID == competitorID);

            if (race == null || competitor == null)
            {
                return NotFound();
            }

            var personResult = await _context.PersonResults.Where(r => r.Person == competitor && r.Race == race).Include(pr => pr.Result).SingleOrDefaultAsync();
            if (personResult == null)
            {
                return NotFound();
            }

            if (file.Length > 0)
            {
                var gpxFilePath = System.IO.Path.GetTempFileName();

                using (var stream = System.IO.File.Create(gpxFilePath))
                {
                    await file.CopyToAsync(stream);
                }

                GpxReader gpxReader = new GpxReader(gpxFilePath);
                Models.GPS.Path path = gpxReader.Read();

                if (path != null)
                {                    
                    await GPSManipulationAsync(path, personResult);
                    return Ok(new { id = path.ID }); 
                }

            }
            return BadRequest();
        }
        
        private async Task GPSManipulationAsync(Models.GPS.Path path, PersonResult personResult)
        {
            var interpolated = pathAnalysis.InterpolationByTime(path.Locations, 1);
            var splits = await _context.SplitTimes.Where(st => st.Result == personResult.Result)
                                .Include(st => st.Split)
                                .ThenInclude(s => s.SecondControl)
                                .ToListAsync();

            double ofsset = pathAnalysis.GetOffsetFromSplits(splits, interpolated);
            var completeLocations = pathAnalysis.SetOffset(interpolated, ofsset);
           
            path.Locations = null;
            path.PersonResult = personResult;
            _context.Add(path);
            _context.SaveChanges();

            foreach (var cl in completeLocations)
            {
                cl.PathID = path.ID;
            }
            _context.BulkInsert(completeLocations);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var path = await _context.Paths.Include(p => p.PersonResult)
                                           .SingleOrDefaultAsync(r => r.ID == id);

            if (path == null)
            {
                return NotFound();
            }

            _context.Paths.Remove(path);
            _context.SaveChanges();

            return RedirectToAction("Details", "Races", new { id = path.PersonResult.RaceID });
        }

        [HttpGet]
        public async Task<IActionResult> GetPath(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var path = await GetSinglePathAsync(p => p.ID == id);

            if (path == null)
            {
                return NotFound();
            }

            var personResult = await GetSinglePersonResultAsync(pr => pr.PathID == path.ID);
            if (personResult == null)
            {
                return NotFound();
            }

            var splitTimeOfAllPath = new SplitTime { Time = path.PersonResult.Result.FinishTime, TimeSpan = (path.PersonResult.Result.FinishTime  - path.PersonResult.Result.StartTime).Seconds };
            var locationsTruncatedByResults = GetSplitPath(path, splitTimeOfAllPath);
            if (locationsTruncatedByResults != null)
            {
                path.Locations = locationsTruncatedByResults;
            }

            var options = new Newtonsoft.Json.JsonSerializerSettings
            {
                MaxDepth = 1,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore
            };
            return Json(path, options);
        }

        [HttpGet]
        public async Task<IActionResult> GetCompletePath(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var path = await GetSinglePathAsync(r => r.ID == id);

            if (path == null)
            {
                return NotFound();
            }

            var interpolationByDistance = pathAnalysis.InterpolationByTime(path.Locations, 1);
            path.Locations = interpolationByDistance;

            var options = new Newtonsoft.Json.JsonSerializerSettings
            {
                MaxDepth = 1,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore
            };
            return Json(path, options);
        }
        
       

    }
}