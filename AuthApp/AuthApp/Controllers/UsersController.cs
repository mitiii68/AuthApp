using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Home");

            return View();
        }
    }
}