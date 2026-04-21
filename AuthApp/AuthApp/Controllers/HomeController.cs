using AuthApp.Data;
using AuthApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AuthApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("user") == null)
                return RedirectToAction("Login", "Account");
            var email = HttpContext.Session.GetString("user");
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            ViewBag.FullName = user?.FullName ?? "Пользователь";
            ViewBag.Login = user?.Login ?? "";
            return View();

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
