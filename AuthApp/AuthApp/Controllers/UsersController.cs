using Microsoft.AspNetCore.Mvc;
using AuthApp.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AuthApp.Models;

namespace AuthApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string search)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Home");

            var users = _context.Users
                .Include(u => u.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                users = users.Where(u =>
                    (u.FullName != null && u.FullName.Contains(search)) ||
                    (u.Email != null && u.Email.Contains(search)) ||
                    (u.Login != null && u.Login.Contains(search))
                );
            }

            return View(users.ToList());
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
            string deletedEmail = user.Email ?? "";

            _context.Users.Remove(user);
            _context.SaveChanges();

            _context.UserActionLog.Add(new UserActionLog
            {
                UserEmail = HttpContext.Session.GetString("user"),
                Action = $"Удалил пользователя {deletedEmail}",
                ActionTime = DateTime.Now
            });
            _context.SaveChanges();
            return RedirectToAction("Index");

        }

        public IActionResult ActionLogs()
        {
            var logs = _context.UserActionLog
                .OrderByDescending(x => x.ActionTime)
                .ToList();
            return View(logs);
        }

       
        
        
    }
}