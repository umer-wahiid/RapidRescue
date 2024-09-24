using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RapidResuce.Context;
using RapidResuce.Models;

namespace RapidResuce.Controllers
{
    public class FirstAidsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public FirstAidsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        private bool FirstAidExists(int id)
        {
            return _context.FirstAids.Any(e => e.Id == id);
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Get()
        {
            try
            {
                var firstAid = await _context.FirstAids.ToListAsync();

                return Json(new { Success = true, Data = firstAid });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> GetById(int id)
        {
            var firstAid = await _context.FirstAids.FirstOrDefaultAsync(m => m.Id == id);
            if (firstAid is null)
                return Json(new { Success = false, Message = $"FirstAid with id {id} not found" });

            return Json(new { Success = true, Data = firstAid });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(FirstAid firstAid)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    firstAid.CreatedDate = DateTime.Now;
                    firstAid.UpdatedDate = DateTime.Now;
                    _context.Add(firstAid);
                    await _context.SaveChangesAsync();
                    return Json(new { Success = true, Message = "FirstAid Added Successfully" });
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

        [HttpPost]
        public async Task<IActionResult> Edit(FirstAid firstAid, IFormFile image)
        {
            try
            {

                if (firstAid.Id == 0)
                    return Json(new { Success = false, Message = $"FirstAid not found" });

                var existingFirstAid = _context.FirstAids.AsNoTracking().FirstOrDefault(m => m.Id == firstAid.Id);

                if (existingFirstAid is null)
                    return Json(new { Success = false, Message = $"FirstAid not found" });

                firstAid.UpdatedDate = DateTime.Now;
                firstAid.CreatedDate = existingFirstAid.CreatedDate;

                try
                {
                    _context.Update(firstAid);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FirstAidExists(firstAid.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Json(new { Success = true, Message = $"FirstAid Updated Succesfully" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = $"{ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var firstAid = await _context.FirstAids.FindAsync(id);
                if (firstAid is null)
                    return Json(new { Success = false, Message = $"FirstAid not found" });

                firstAid.IsDeleted = true;
                firstAid.UpdatedDate = DateTime.Now;
                _context.Update(firstAid);
                await _context.SaveChangesAsync();
                return Json(new { Success = true, Message = $"FirstAid Deleted Succesfully" });

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = $"{ex.Message}" });
            }

        }
    }
}
