using Microsoft.AspNetCore.Mvc;
using RapidResuce.Context;
using RapidResuce.Models;

namespace RapidResuce.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminController
        public ActionResult Index()
        {
            int completedCount = _context.EmergencyRequests
                .Where(e => e.Status == Enums.RequestStatus.Completed)
                .Count();

            ViewBag.Completed = completedCount;

            int inprogressCount = _context.EmergencyRequests
                .Where(e => e.Status == Enums.RequestStatus.Dispatched
                            || e.Status == Enums.RequestStatus.EnRoute || e.Status == Enums.RequestStatus.Arrived)
                .Count();

            ViewBag.InProgress = inprogressCount;

            int pendingCount = _context.EmergencyRequests
                .Where(e => e.Status == Enums.RequestStatus.Pending)
                .Count();

            ViewBag.Pending = pendingCount;

            int cancelCount = _context.EmergencyRequests
                .Where(e => e.Status == Enums.RequestStatus.Canceled)
                .Count();

            ViewBag.Cancelled = cancelCount;

            return View();
        }

        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public IActionResult AdminProfile()
        {
            var profile = _context.Users
                .FirstOrDefault(e => e.Id == Session.UserId);

            return View(profile);
        }
    }
}
