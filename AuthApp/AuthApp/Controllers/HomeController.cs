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
            var showWelcome = HttpContext.Session.GetString("ShowWelcome");
            var userName = HttpContext.Session.GetString("UserName");

            if (showWelcome == "true")
            {
                ViewBag.ShowWelcome = true;
                ViewBag.UserName = string.IsNullOrEmpty(userName)
                    ? "пользователь"
                    : userName;

                HttpContext.Session.Remove("ShowWelcome");
            }

            return View();
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        

    }
}
