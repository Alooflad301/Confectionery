using Confectionery.Data;
using Confectionery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Area("Admin")]
[Authorize(Roles = "Администратор")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var stats = new
        {
            UsersCount = _context.Users.Count(),
            OrdersCount = _context.Orders.Count(),
            TotalRevenue = _context.Orders.Sum(o => o.Price),
            ProductsCount = _context.Catalog.Count()
        };
        ViewBag.Stats = stats;
        return View();
    }

    // 👥 ПОЛЬЗОВАТЕЛИ
    public async Task<IActionResult> Users()
    {
        var users = await _context.Users.Include(u => u.Role).ToListAsync();
        return View(users);
    }

    [HttpGet]
    public IActionResult EditUser(int id)
    {
        var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Id_User == id);
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(int id, User user)
    {
        if (id != user.Id_User) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Пользователь обновлен!";
            return RedirectToAction("Users");
        }
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Пользователь удален!";
        }
        return RedirectToAction("Users");
    }

    // 🛒 ТОВАРЫ
    public async Task<IActionResult> Products()
    {
        var products = await _context.Catalog.Include(c => c.Category).ToListAsync();
        return View(products);
    }

    [HttpGet]
    public IActionResult EditProduct(int id)
    {
        var product = _context.Catalog.Include(c => c.Category).FirstOrDefault(c => c.Id_Catalog == id);
        ViewBag.Categories = _context.Categories.ToList();
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> EditProduct(int id, Catalog product)
    {
        if (id != product.Id_Catalog) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Catalog.Update(product);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Товар обновлен!";
            return RedirectToAction("Products");
        }
        ViewBag.Categories = _context.Categories.ToList();
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Catalog.FindAsync(id);
        if (product != null)
        {
            _context.Catalog.Remove(product);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Товар удален!";
        }
        return RedirectToAction("Products");
    }

    // 📋 ЗАКАЗЫ
    public async Task<IActionResult> Orders()
    {
        var orders = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.StatusOrder)
            .ToListAsync();
        return View(orders);
    }

    [HttpGet]
    public IActionResult EditOrder(int id)
    {
        var order = _context.Orders
            .Include(o => o.StatusOrder)
            .Include(o => o.User)
            .FirstOrDefault(o => o.Id_Order == id);
        ViewBag.Statuses = _context.StatusOrders.ToList();
        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> EditOrder(int id, Order order)
    {
        if (id != order.Id_Order) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Заказ обновлен!";
            return RedirectToAction("Orders");
        }
        ViewBag.Statuses = _context.StatusOrders.ToList();
        return View(order);
    }
}


