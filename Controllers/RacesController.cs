using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OBPostupy.Data;
using OBPostupy.Models.Races;
using OBPostupy.Extensions.Messages;
using OBPostupy.ViewModels;

namespace OBPostupy.Controllers
{
    public class RacesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public RacesController(ApplicationDbContext context) :base(context)
        {
            _context = context;
        }


        /// <summary>
        /// Action for detail of race
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
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

            RaceDetailViewModel raceDetail = new RaceDetailViewModel
            {
                Race = race,
                Map = race.Map,
                Categories = race.Categories.OrderBy(c => c.Name).Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name,
                }).ToList(),
                HasStravaAccount = false

            };

            raceDetail.Categories.Insert(0, new SelectListItem { Value = "0", Text = "Vyberte kategorii", Selected = true});

            return View(raceDetail);
        }

        /// <summary>
        /// Action for race creating
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create race comfirmation
        /// </summary>
        /// <param name="race"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,StartTime")] Race race)
        {
            if (ModelState.IsValid)
            {
                _context.Add(race);
                await _context.SaveChangesAsync();
                return RedirectToAction("Setting", "Races", new { id = race.ID}).Success("Závod "+race.Name+" byl vytvořen.");
            }
            return View(race);
        }


        /// <summary>
        /// Action that returns the race settings page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Setting(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var race = await _context.Races.SingleOrDefaultAsync(r => r.ID == id);
            if (race == null)
            {
                return NotFound();
            }

            return View(GetSettingViewModel(race));
        }

        /// <summary>
        /// Edit race comfirmation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="race"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setting(int id, [Bind("ID,Name,StartTime")] Race race)
        {
            if (id != race.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(race);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RaceExists(race.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return View(GetSettingViewModel(race)).Success("Nastavení bylo uloženo.");
            }
            return View(GetSettingViewModel(race)).Danger("Data nejsou validní");
        }               

        
        /// <summary>
        /// Action for delete race
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var race = await _context.Races
                .FirstOrDefaultAsync(m => m.ID == id);
            if (race == null)
            {
                return NotFound();
            }

            return View(race);
        }

        /// <summary>
        /// Delete race confirmation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var race = await _context.Races.FindAsync(id);
            try
            {
                _context.Races.Remove(race);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home").Success("Závod " + race.Name + " byl odstraněn.");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home").Danger("Závod " + race.Name + " NEBYL odstraněn. Odstraňte prosím nejdříve všechna data(Tratě, Mapu a Výsledky).");
            }
        }

        private bool RaceExists(int id)
        {
            return _context.Races.Any(e => e.ID == id);
        }

        private RaceSettingViewModel GetSettingViewModel(Race race)
        {
            RaceSettingViewModel raceSetting = new RaceSettingViewModel
            {
                Race = race,
                IsCategoriesUploaded = _context.Categories.Any(c => c.RaceID == race.ID),
                IsCoursesUploaded = _context.CourseData.Any(c => c.RaceID == race.ID),
                IsMapUploaded = _context.Maps.Any(m => m.RaceID == race.ID),
                IsResultsUploaded = _context.PersonResults.Any(pr => pr.RaceID == race.ID)
            };
            return raceSetting;
        }
    }
}
