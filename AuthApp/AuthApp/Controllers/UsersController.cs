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
        public IActionResult Index(string search, string sort)
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

            switch (sort)
            {
                case "date":
                    users = users.OrderByDescending(u => u.CreateAt);
                    break;
                case "login":
                    users = users.OrderBy(u => u.Login);
                    break;
                case "email":
                    users = users.OrderByDescending(u => u.LoginCount);
                    break;
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
        [HttpPost]
        public IActionResult BlockUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);

            if (user == null)
                return NotFound();

            user.IsBlocked = true;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UnblockUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);

            if (user == null)
                return NotFound();

            user.IsBlocked = false;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult EditProfile(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Home");

            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult EditProfile(int userId, string? district, string? ruralDistrict,
            string? settlement, string? street, string? house,
            double? latitude, double? longitude)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Home");

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return NotFound();

            user.District = district;
            user.RuralDistrict = ruralDistrict;
            user.Settlement = settlement;
            user.Street = street;
            user.House = house;
            user.Latitude = latitude;
            user.Longitude = longitude;

            _context.SaveChanges();

            _context.UserActionLog.Add(new UserActionLog
            {
                UserEmail = HttpContext.Session.GetString("user"),
                Action = $"Обновил профиль пользователя {user.Email}",
                ActionTime = DateTime.Now
            });
            _context.SaveChanges();

            return RedirectToAction("Index");
        }




    }
}