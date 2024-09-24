using Microsoft.AspNetCore.Mvc;

namespace RapidResuce.Controllers
{
    public class UserAuthController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult EditProfile()
        {
            return View();
        }
    }
}
