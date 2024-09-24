using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RapidResuce.Context;
using RapidResuce.Models;

namespace RapidResuce.Controllers
{
    public class AmbulancesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AmbulancesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        private bool AmbulanceExists(int id)
        {
            return _context.Ambulances.Any(e => e.Id == id);
        }
        public IActionResult Index()
        {
            ViewData["Drivers"] = new SelectList(
                _context.Users
                .Where(dr => dr.Role == Enums.UserRole.Driver)
                .Select(dr => new { dr.Id, FullName = dr.FirstName + " " + dr.LastName }),
                "Id",
                "FullName"
            );
            ViewData["Emts"] = new SelectList(
                _context.Users
                .Where(dr => dr.Role == Enums.UserRole.EMT)
                .Select(dr => new { dr.Id, FullName = dr.FirstName + " " + dr.LastName }),
                "Id",
                "FullName"
            );

            //ViewData["Emts"] = new SelectList(_context.Users.Where(dr => dr.Role == Enums.UserRole.EMT), "Id", "FirstName" + " " + "LastName");
            return View();
        }
        public async Task<IActionResult> Get()
        {
            try
            {
                var ambulance = await _context.Ambulances.Include(a => a.Driver).AsNoTracking().Where(x => !x.IsDeleted).ToListAsync();

                return Json(new { Success = true, Data = ambulance });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IActionResult> GetById(int id)
        {
            var ambulance = await _context.Ambulances.Include(a => a.Driver).AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ambulance is null)
                return Json(new { Success = false, Message = $"Ambulance with id {id} not found" });
            ViewData["DriverId"] = new SelectList(_context.Users, "Id", "LastName", ambulance.DriverId);

            return Json(new { Success = true, Data = ambulance });
        }
        public IActionResult Create()
        {
            ViewData["DriverId"] = new SelectList(_context.Users, "Id", "Address");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Ambulance ambulance, IFormFile image)
        {
            try
            {
                ModelState.Remove("Image");
                ModelState.Remove("EmergencyRequests");
                ModelState.Remove("Driver");
                ModelState.Remove("Emt");

                if (ModelState.IsValid)
                {
                    if (image is null)
                        return Json(new { Success = false, Message = "Please Insert Image" });


                    if (!Common.Utility.IsImage(image.FileName))
                        return Json(new { Success = false, Message = "Wrong Picture Format" });

                    var path = await Common.Utility.SaveMedia(_environment, image, "Ambulances");

                    ambulance.Image = path;
                    ambulance.CreatedDate = DateTime.Now;
                    ambulance.UpdatedDate = DateTime.Now;
                    _context.Add(ambulance);
                    await _context.SaveChangesAsync();
                    return Json(new { Success = true, Message = "Ambulance Added Successfully" });
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
        public async Task<IActionResult> Edit(Ambulance ambulance, IFormFile image)
        {
            try
            {

                if (ambulance.Id == 0)
                    return Json(new { Success = false, Message = $"Ambulance not found" });

                var existingAmbulance = _context.Ambulances.AsNoTracking().FirstOrDefault(m => m.Id == ambulance.Id);
                if (existingAmbulance is null)
                    return Json(new { Success = false, Message = $"Ambulance not found" });


                if (image is null)
                {
                    ambulance.Image = existingAmbulance.Image;
                }
                else
                {
                    if (!Common.Utility.IsImage(image.FileName))
                        return Json(new { Success = false, Message = "Wrong Picture Format" });

                    ambulance.Image = await Common.Utility.SaveMedia(_environment, image, "Ambulances");
                }


                ambulance.UpdatedDate = DateTime.Now;
                ambulance.CreatedDate = existingAmbulance.CreatedDate;

                try
                {
                    _context.Update(ambulance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AmbulanceExists(ambulance.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Json(new { Success = true, Message = $"Ambulance Updated Succesfully" });
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
                var ambulance = await _context.Ambulances.FindAsync(id);
                if (ambulance is null)
                    return Json(new { Success = false, Message = $"Ambulance not found" });

                ambulance.IsDeleted = true;
                ambulance.UpdatedDate = DateTime.Now;
                _context.Update(ambulance);
                await _context.SaveChangesAsync();
                return Json(new { Success = true, Message = $"Ambulance Deleted Succesfully" });

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = $"{ex.Message}" });
            }

        }
    }
}
