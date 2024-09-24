using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using RapidResuce.Context;
using RapidResuce.Enums;
using RapidResuce.Models;

namespace RapidResuce.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.error = "";


            if (Session.UserId != 0 && Session.Role == UserRole.Patient)
                return RedirectToAction("Index", "Home");

            if (Session.UserId != 0 && Session.Role != UserRole.Patient)
                return RedirectToAction("Index", "admin");

            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            try
            {
                if (Session.UserId != 0)
                    return RedirectToAction("Index", "admin");

                var userByPassword = _context.Users.Where(x => x.Email.Equals(user.Email) && x.Password.Equals(user.Password)).FirstOrDefault();
                if (userByPassword is null)
                {
                    ViewBag.error = "Incorrect Password or Username";
                    return View("Index");
                }
                Session.UserId = userByPassword.Id;
                Session.Name = $"{userByPassword.FirstName} {userByPassword.LastName}";
                Session.Role = userByPassword.Role;
                Session.Email = userByPassword.Email;
                Session.Image = userByPassword.Image;

                if (userByPassword.Role == UserRole.Patient)
                    return RedirectToAction("Index", "Home");

                return RedirectToAction("Index", "admin");

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
     

    }
}
