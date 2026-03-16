using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Confectionery.Data;
using Confectionery.Models;

namespace Confectionery.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Очищаем предыдущие ошибки валидации
            ModelState.Clear();

            if (string.IsNullOrEmpty(model.Login) || string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("", "Введите логин и пароль.");
                return View(model);
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Login == model.Login);

            if (user == null || user.Password != model.Password)
            {
                ModelState.AddModelError("", "Неверный логин или пароль.");
                return View(model);
            }

            // Создаем claims для авторизации
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id_User.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "Пользователь")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(14)
                    : DateTimeOffset.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Очищаем предыдущие ошибки валидации
            ModelState.Clear();

            if (string.IsNullOrEmpty(model.Login) || string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("", "Заполните обязательные поля.");
                return View(model);
            }

            // Проверка на существующего пользователя
            if (await _context.Users.AnyAsync(u => u.Login == model.Login))
            {
                ModelState.AddModelError("", "Пользователь с таким логином уже существует.");
                return View(model);
            }

            // Создаем нового пользователя с ролью "Пользователь" (Id_Role = 1)
            var user = new User
            {
                Login = model.Login,
                Name = model.Name ?? "",
                Password = model.Password,
                Email = model.Email ?? "",
                Id_Role = 1 // Роль "Пользователь" по умолчанию
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Автоматическая авторизация после регистрации
            var newUser = await _context.Users
                .Include(u => u.Role)
                .FirstAsync(u => u.Id_User == user.Id_User);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.Id_User.ToString()),
                new Claim(ClaimTypes.Name, newUser.Login),
                new Claim(ClaimTypes.Role, "Пользователь")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }

    // ViewModel для формы логина
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Логин обязателен")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    // ViewModel для формы регистрации
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Логин обязателен")]
        [StringLength(50)]
        public string Login { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(50)]
        public string Password { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(50)]
        public string? Email { get; set; }
    }
}
