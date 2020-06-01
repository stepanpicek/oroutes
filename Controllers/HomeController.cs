using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OBPostupy.Models;
using System.IO;
using System.Globalization;
using OBPostupy.Models.GPS;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OBPostupy.Data;


namespace OBPostupy.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Main page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Races.ToListAsync());
        }                

    }
}
