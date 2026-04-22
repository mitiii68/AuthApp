using Microsoft.AspNetCore.Mvc;
using AuthApp.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Controllers
{
    public class LoginHistoryController : Controller
    {
        private readonly AppDbContext _context;

        public LoginHistoryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Home");

            var history = _context.LoginHistories
                .OrderByDescending(h => h.LoginTime)
                .ToList();

            return View(history);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Home");

            var entry = _context.LoginHistories.FirstOrDefault(h => h.Id == id);

            if (entry != null)
            {
                _context.LoginHistories.Remove(entry);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteAll()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Home");

            _context.LoginHistories.RemoveRange(_context.LoginHistories);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
