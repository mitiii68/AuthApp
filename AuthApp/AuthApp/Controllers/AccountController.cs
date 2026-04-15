using Microsoft.AspNetCore.Mvc;
using AuthApp.Models;
using AuthApp.Data;
using System.Linq;

namespace AuthApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (string.IsNullOrWhiteSpace(model.Password) || !IsValidPassword(model.Password))
            {
                ModelState.AddModelError("Password",
                    "Пароль должен быть не менее 8 символов и содержать заглавные, строчные буквы, цифры и спецсимвол.");
                return View(model);
            }

            string login = model.Email!.Split('@')[0];

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password,
                Login = login
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Content($"Регистрация работает. Логин: {login}");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        private bool IsValidPassword(string password)
        {
            if (password.Length < 8) return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }
    }
}