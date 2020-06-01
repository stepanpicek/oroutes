using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBPostupy.Data;
using OBPostupy.Models.Races;
using OBPostupy.Models.Maps;
using Microsoft.AspNetCore.Hosting;
using OBPostupy.Extensions.Messages;
using System.Diagnostics;

namespace OBPostupy.Controllers
{
    public class MapsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public MapsController(ApplicationDbContext context, IWebHostEnvironment env):base(context)
        {
            _context = context;
            _env = env;
        }


        /// <summary>
        /// File upload action for map data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadMap(int? id, IFormFile file)
        {            
            if(id == null)
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
                var mapFilePath = Path.GetTempFileName();

                using (var stream = System.IO.File.Create(mapFilePath))
                {
                    await file.CopyToAsync(stream);
                }
                MapReader mapReader = new MapReader(_env.WebRootPath,mapFilePath, Path.GetExtension(file.FileName).ToLowerInvariant());
                Map map = mapReader.Read();
                if(map != null)
                {
                    map.Race = race;
                    _context.Maps.Add(map);
                    _context.SaveChanges();
                    return RedirectToAction("Setting", "Races", new { id = id }).Success("Mapa byla nahrána.");
                }

            }
            return RedirectToAction("Setting", "Races", new { id = id }).Danger("Mapa nebyla nahrána.");
        }

        /// <summary>
        /// Actions for showing the map image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Show(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var race = await _context.Races.Include(r => r.Map)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (race == null || race.Map == null)
            {
                return NotFound();
            }

            string pathToImage = Path.Combine(_env.WebRootPath, race.Map.PathToFile);
            var extension = Path.GetExtension(pathToImage);
            if (System.IO.File.Exists(pathToImage))
            {
                return Redirect("~/" + race.Map.PathToFile);
            }

            return NotFound();
        }

        /// <summary>
        /// Action for delete map
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var race = await _context.Races.Include(r => r.Map)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (race == null || race.Map == null)
            {
                return NotFound();
            }

            return View(race.Map);
        }

        /// <summary>
        /// Delete comfirmation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var race = await _context.Races.Include(r => r.Map).FirstOrDefaultAsync(m => m.ID == id);

            if (race == null || race.Map == null)
            {
                return NotFound();
            }

            string pathToImage = Path.Combine(_env.WebRootPath, race.Map.PathToFile);
            if (System.IO.File.Exists(pathToImage))
            {
                System.IO.File.Delete(pathToImage);
            }

            try
            {
                _context.Maps.Remove(race.Map);
                await _context.SaveChangesAsync();
                return RedirectToAction("Setting", "Races", new { id = race.ID}).Success("Mapa byla smazána.");
            }
            catch (Exception)
            {
                return RedirectToAction("Setting", "Races", new { id = race.ID }).Danger("Mapa NEbyla smazána, nastal nějaký problém.");
            }
        }
    }
}