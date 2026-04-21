using Microsoft.AspNetCore.Mvc;
using AuthApp.Data;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace AuthApp.Controllers
{
    public class RolesController : Controller
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Home");

            var roles = _context.Roles.ToList();

            return View(roles);
        }
    }
}