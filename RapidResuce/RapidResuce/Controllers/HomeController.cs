using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapidResuce.Context;
using RapidResuce.Enums;
using RapidResuce.Models;
using System.Diagnostics;

namespace RapidResuce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var feedbacks = _context.Feedbacks.Include(f => f.EmergencyRequest).Include(f => f.EmergencyRequest.User).ToList();
            return View(feedbacks);
        }
        public IActionResult Team()
        {
            var ourTeam = _context.Users.Where(x=>x.Role.Equals(UserRole.EMT) || x.Role.Equals(UserRole.Driver)).ToList();
            return View(ourTeam);
        }

        public IActionResult About()
        {
            var feedbacks = _context.Feedbacks.Include(f => f.EmergencyRequest).Include(f => f.EmergencyRequest.User).ToList();
            return View(feedbacks);
        }
        public IActionResult Gallery()
        {
            var ambulances = _context.Ambulances.Include(a => a.Driver).Include(a => a.Emt).AsNoTracking().ToList();
            return View(ambulances);
        }
        public IActionResult AmbulanceDetail(int id)
        {
            var ambulance =  _context.Ambulances.Include(a => a.Driver).Include(a => a.Emt).AsNoTracking().FirstOrDefault(x => x.Id.Equals(id));
            if (ambulance is null)
                return NotFound();
            return View(ambulance);
        }
        public IActionResult Driver()
        {
            return View();
        }
        public IActionResult MedicalProfile()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
