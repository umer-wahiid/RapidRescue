using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RapidResuce.Context;
using RapidResuce.Models;
using RapidResuce.Interfaces;

namespace RapidResuce.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _environment;

        public UsersController(ApplicationDbContext context, IWebHostEnvironment environment, IEmailService emailService)
        {
            _context = context;
            _environment = environment;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Get()
        {
            try
            {
                var users = await _context.Users.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync();
                return Json(new { Success = true, Data = users });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
       
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user is null)
                return Json(new { Success = false, Message = $"User with id {id} not found" });

            return Json(new { Success = true, Data = user });
        }

        [HttpPost]
        public async Task<IActionResult> Add(User user, IFormFile image)
        {
            try
            {
                ModelState.Remove("Image");

                if (ModelState.IsValid)
                {

                    if (image is null)
                        return Json(new { Success = false, Message = "Please Insert Image" });

                    if (!IsEmailUnique(user.Email))
                        return Json(new { Success = false, Message = "This Email Address Already Exists" });


                    if (!Common.Utility.IsImage(image.FileName))
                        return Json(new { Success = false, Message = "Wrong Picture Format" });

                    var path = await Common.Utility.SaveMedia(_environment, image, "Users");

                    user.Image = path;
                    user.CreatedDate = DateTime.Now;
                    user.UpdatedDate = DateTime.Now;
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    //_emailService.SendConfirmationEmail(user.Email, user.FirstName);
                    return Json(new { Success = true, Message = "User Added Successfully" });
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
        public async Task<IActionResult> Edit(User user, IFormFile image)
        {
            try
            {

                if (user.Id == 0)
                    return Json(new { Success = false, Message = $"User not found" });


                var existingUser = _context.Users.AsNoTracking().FirstOrDefault(m => m.Id == user.Id);
                if (existingUser is null)
                    return Json(new { Success = false, Message = $"User not found" });

                if (user.Email!=existingUser.Email && !IsEmailUnique(user.Email))
                    return Json(new { Success = false, Message = "This Email Address Already Exists" });

                if (string.IsNullOrEmpty(user.Password))
                    user.Password = existingUser.Password;
                if (image is null)
                {
                    user.Image = existingUser.Image;
                }
                else
                {
                    if (!Common.Utility.IsImage(image.FileName))
                        return Json(new { Success = false, Message = "Wrong Picture Format" });

                    user.Image = await Common.Utility.SaveMedia(_environment, image, "Users");
                }


                user.UpdatedDate = DateTime.Now;
                user.CreatedDate = existingUser.CreatedDate;

                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Json(new { Success = true, Message = $"User Updated Succesfully" });
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
                var user = await _context.Users.FindAsync(id);
                if (user is null)
                    return Json(new { Success = false, Message = $"User not found" });

                user.IsDeleted = true;
                user.UpdatedDate = DateTime.Now;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return Json(new { Success = true, Message = $"User Deleted Succesfully" });

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = $"{ex.Message}" });
            }

        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        public bool IsEmailUnique(string email)
        {
            return !_context.Users.Any(u => u.Email == email);
        }
    }
}
