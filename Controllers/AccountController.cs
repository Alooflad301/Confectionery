using Confectionery.Data;
using Confectionery.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

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

            return RedirectToAction("Index", "Catalogs");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]  // ✅ Добавь для защиты
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // ✅ Очищаем старые ошибки
            ModelState.Clear();

            // Простая проверка обязательных полей
            if (string.IsNullOrEmpty(model.Login) || string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("", "Заполните логин и пароль");
                return View(model);
            }

            // ✅ Атрибуты [Compare], [Required] уже проверили пароли
            if (!ModelState.IsValid)
            {
                return View(model);  // ✅ Показываем ВСЕ ошибки под полями
            }

            // Проверки БД (уникальность)
            if (await _context.Users.AnyAsync(u => u.Login == model.Login))
            {
                ModelState.AddModelError("Login", "Логин уже занят!");
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.Email) &&
                await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email уже используется!");
                return View(model);
            }

            // ✅ Создаем пользователя
            var user = new User
            {
                Login = model.Login,
                Name = model.Name ?? "",
                Password = model.Password,  // 🎉 Без хеширования
                Email = model.Email ?? "",
                Id_Role = 1  // Пользователь (было 1)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Автологин
            var newUser = await _context.Users
                .Include(u => u.Role)
                .FirstAsync(u => u.Id_User == user.Id_User);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, newUser.Id_User.ToString()),
        new Claim(ClaimTypes.Name, newUser.Login),
        new Claim(ClaimTypes.Role, newUser.Role?.Name ?? "Пользователь")
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            TempData["Success"] = "✅ Регистрация успешна!";
            return RedirectToAction("Index", "Catalogs");
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Baskets)
                .ThenInclude(b => b.BasketCatalogs)
                .ThenInclude(bc => bc.Catalog)
                .FirstOrDefaultAsync(u => u.Id_User == userId);

            if (user == null) return NotFound();

            // ✅ СТАТИСТИКА
            ViewBag.OrderCount = await _context.Orders.CountAsync(o => o.Id_User == userId);
            ViewBag.BasketItems = user.Baskets?.SelectMany(b => b.BasketCatalogs).Sum(bc => bc.Quantity) ?? 0;
            ViewBag.BasketTotal = user.Baskets?.SelectMany(b => b.BasketCatalogs)
                .Sum(bc => bc.Quantity * bc.Catalog.Price) ?? 0;

            return View(user);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Orders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.StatusOrder)
                .Include(o => o.OrderCatalogs)
                .ThenInclude(oc => oc.Catalog)
                .ThenInclude(c => c.Category)
                .Where(o => o.Id_User == userId)
                .OrderByDescending(o => o.Date)
                .ToListAsync();

            return View(orders);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.StatusOrder)
                .Include(o => o.OrderCatalogs)!
                .ThenInclude(oc => oc.Catalog)
                .ThenInclude(c => c.Category)
                .FirstOrDefaultAsync(o => o.Id_Order == id && o.Id_User == userId);

            if (order == null) return NotFound();
            return View(order);
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
        // 🔥 СООБЩЕНИЯ ОШИБОК (статические константы)
        public static class ValidationMessages
        {
            public const string Required = "Поле обязательно для заполнения";
            public const string InvalidLogin = "Логин: латинские буквы, цифры, ._- (3-50 символов)";
            public const string InvalidName = "Имя: только буквы и пробелы (2-50 символов)";
            public const string InvalidPassword = "Пароль: 6-16 символов без пробелов";
            public const string InvalidEmail = "Введите корректный email";
            public const string PasswordsDoNotMatch = "Пароли не совпадают";
            public const string LoginExists = "Логин уже используется";
            public const string EmailExists = "Email уже зарегистрирован";
        }

        // 🔥 ПАТТЕРНЫ (статические константы)
        public static class ValidationPatterns
        {
            public const string LoginPattern = @"^[a-zA-Z0-9._-]+$";
            public const string NamePattern = @"^[а-яА-ЯёЁa-zA-Z\s]+$";
            public const string PasswordPattern = @"^(?=.{6,16}$)[^\s]+$";
        }

        // 🔥 ПОЛЯ МОДЕЛИ с валидацией
        [Required(ErrorMessage = ValidationMessages.Required)]
        [StringLength(50, MinimumLength = 3, ErrorMessage = ValidationMessages.InvalidLogin)]
        [RegularExpression(ValidationPatterns.LoginPattern, ErrorMessage = ValidationMessages.InvalidLogin)]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessages.Required)]
        [StringLength(50, MinimumLength = 2, ErrorMessage = ValidationMessages.InvalidName)]
        [RegularExpression(ValidationPatterns.NamePattern, ErrorMessage = ValidationMessages.InvalidName)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessages.Required)]
        [EmailAddress(ErrorMessage = ValidationMessages.InvalidEmail)]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessages.Required)]
        [StringLength(16, MinimumLength = 6, ErrorMessage = ValidationMessages.InvalidPassword)]
        [RegularExpression(ValidationPatterns.PasswordPattern, ErrorMessage = ValidationMessages.InvalidPassword)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessages.Required)]
        [Compare(nameof(Password), ErrorMessage = ValidationMessages.PasswordsDoNotMatch)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }


}
