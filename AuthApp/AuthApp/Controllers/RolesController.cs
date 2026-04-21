using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Controllers
{
    public class RolesController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Home");
            return View();
        }
    }
}
