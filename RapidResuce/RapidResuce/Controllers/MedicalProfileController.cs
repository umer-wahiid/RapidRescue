using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapidResuce.Context;
using RapidResuce.Models;

namespace RapidResuce.Controllers
{
    public class MedicalProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicalProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.MedicalProfiles.AsNoTracking()
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user is null)
                user = new MedicalProfile();

            return Json(new { Success = true, Data = user });
        }

        [HttpPost]
        public async Task<IActionResult> Add(MedicalProfile medicalProfile)
        {
            try
            {
                ModelState.Remove("User");
                string message = string.Empty;
                if (ModelState.IsValid)
                {
                    var exists = _context.MedicalProfiles.AsNoTracking().FirstOrDefault(med => med.UserId == medicalProfile.UserId);
                    if (exists!=null)
                    {
                        medicalProfile.Id = exists.Id;
                        medicalProfile.UpdatedDate = DateTime.Now;
                        _context.Update(medicalProfile);
                        message = "Profile updated successfully!";
                    }
                    else
                    {
                        medicalProfile.CreatedDate = DateTime.Now;
                        medicalProfile.UpdatedDate = DateTime.Now;
                        _context.Add(medicalProfile);
                        message = "Profile added successfully!";
                    }
                    await _context.SaveChangesAsync();
                    return Json(new { Success = true, Message = message });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList();

                return Json(new { Success = false, Message = string.Join(", ", errors) });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = $"{ex.Message}" });
            }
        }
    }
}
