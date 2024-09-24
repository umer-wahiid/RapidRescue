using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapidResuce.Context;
using RapidResuce.Models;

namespace RapidResuce.Controllers
{
    public class FeebackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeebackController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var feedback = _context.Feedbacks.ToList();

            ViewBag.EmergencyRequest = _context.EmergencyRequests
                .Where(e => e.UserId == Session.UserId)
                .ToList();

            //ViewBag.Ratings = Enum.GetValues()
                            /*!_context.Feedbacks.Any(f => f.EmergencyRequestId == e.Id)*/


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Feedback feedback)
        {
            ModelState.Remove("EmergencyRequest");
            if (ModelState.IsValid)
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await _context.Feedbacks.AddAsync(feedback);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Json(new { Success = true, Message = "Feedback Sent Successfully" });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();

                        return Json(new { Success = false, Message = "Error: " + ex.Message });
                    }
                }
            }

            return Json(new { Success = false, Message = "Modal State Invalid" });
        }

    }
}
