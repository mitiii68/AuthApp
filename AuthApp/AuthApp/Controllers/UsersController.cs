using Microsoft.AspNetCore.Mvc;
using AuthApp.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Home");
            var users = _context.Users
                .Include(u => u.Role)
                .ToList();

            return View(users);
        }
        [HttpPost]
        public IActionResult ChangeRole(int userId, int roleId)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Home");

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                return RedirectToAction("Index");

            user.RoleId = roleId;
            _context.SaveChanges();

            return RedirectToAction("Index");

          
        }
        [HttpPost]
        public IActionResult DeleteUser(int userId)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Home");

            var user  = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                return RedirectToAction("Index");

            var currentUserEmail = HttpContext.Session.GetString("user");

            if (user .Email == currentUserEmail)
                return RedirectToAction("Index");

            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}