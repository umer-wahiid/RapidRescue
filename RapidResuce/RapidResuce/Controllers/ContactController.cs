using Microsoft.AspNetCore.Mvc;
using RapidResuce.Context;
using RapidResuce.Models;

namespace RapidResuce.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Contact contact)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await _context.Contacts.AddAsync(contact);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Json(new { Success = true, Message = "Message Sent Successfully" });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return Json(new { Success = false, Message = "Error: " + ex.Message });
                    }
                }
            }

            return Json(new { Success = false, Message = "Model state is invalid" });
        }

    }
}
