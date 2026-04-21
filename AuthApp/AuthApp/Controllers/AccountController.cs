using Microsoft.AspNetCore.Mvc;
using AuthApp.Models;
using AuthApp.Data;
using AuthApp.Services;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public AccountController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
           
                return View(model);

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError("Email", "Введите email");
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Password) || !IsValidPassword(model.Password))
            {
                ModelState.AddModelError("Password",
                    "Пароль должен быть не менее 8 символов и содержать заглавные, строчные буквы, цифры и спецсимвол.");
                return View(model);
            }

            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Такой email уже существует.");
                return View(model);
            }

            
            string code = new Random().Next(100000, 999999).ToString();
            string login = model.Email!.Split('@')[0];
            string token = Guid.NewGuid().ToString();

            
            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = HashPassword(model.Password!),
                Login = login,
                ConfirmationCode = code,
                ConfirmationToken = token,
                RoleId = 1,
                IsConfirmed = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _emailService.SendEmailAsync(
                model.Email!,
                "Код подтверждения регистрации",
                $@"<h2>Добро пожаловать, {model.FullName}!</h2>
                   <p>Ваш код подтверждения:</p>
                   <h1 style='color:#4CAF50; letter-spacing:4px;'>{code}</h1>
                   <p>Введите этот код на странице подтверждения.</p>"
            );

            TempData["Email"] = model.Email;
            return RedirectToAction("ConfirmCode");
           
        }

        [HttpGet]
        public IActionResult ConfirmCode()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ConfirmCode(string email, string code)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null || user.ConfirmationCode != code)
            {
                ModelState.AddModelError(string.Empty, "Неверный код подтверждения.");
                return View();
            }

            user.IsConfirmed = true;
            user.ConfirmationCode = null;
            _context.SaveChanges();

            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("Login", user.Login ?? "");
            HttpContext.Session.SetString("user", user.Email ?? "");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ConfirmEmail(string token)
        {
            var user = _context.Users.FirstOrDefault(u => u.ConfirmationToken == token);

            if (user == null)
                return Content("Ссылка недействительна");

            user.IsConfirmed = true;
            user.ConfirmationToken = null;
            _context.SaveChanges();

            return Content("Почта подтверждена");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == email && u.IsConfirmed);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !VerifyPassword(password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Неверный email, пароль или почта не подтверждена.");
                return View();
            }

            HttpContext.Session.SetString("user", user.Email!);
            HttpContext.Session.SetString("UserRole", user.Role?.RoleName ?? "");
            HttpContext.Session.SetString("FullName", user.FullName ?? ""); 
            HttpContext.Session.SetString("Login", user.Login ?? "");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
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

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}
