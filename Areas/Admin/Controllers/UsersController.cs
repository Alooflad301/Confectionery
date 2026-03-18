using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Confectionery.Data;
using Confectionery.Models;

[Area("Admin")]
[Authorize(Roles = "Администратор")]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _context.Users
            .Include(u => u.Role)
            .OrderBy(u => u.Login)
            .ToListAsync();
        return View(users);
    }

    public IActionResult Create()
    {
        ViewBag.Roles = new SelectList(_context.Roles.ToList(), "Id_Role", "Name");
        return View(new User());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User user)
    {
        if (ModelState.IsValid)
        {
            if (_context.Users.Any(u => u.Login == user.Login))
            {
                ModelState.AddModelError("Login", "Логин уже существует");
                ViewBag.Roles = new SelectList(_context.Roles.ToList(), "Id_Role", "Name");
                return View(user);
            }

            user.Id_Role = user.Id_Role == 0 ? 1 : user.Id_Role;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Пользователь создан!";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Roles = new SelectList(_context.Roles.ToList(), "Id_Role", "Name");
        return View(user);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();
        ViewBag.Roles = new SelectList(_context.Roles.ToList(), "Id_Role", "Name", user.Id_Role);
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, User user)
    {
        if (id != user.Id_User) return NotFound();

        if (ModelState.IsValid)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return NotFound();

            existingUser.Login = user.Login;
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Id_Role = user.Id_Role;
            if (!string.IsNullOrEmpty(user.Password))
                existingUser.Password = user.Password;

            _context.Update(existingUser);
            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Пользователь обновлён!";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Roles = new SelectList(_context.Roles.ToList(), "Id_Role", "Name", user.Id_Role);
        return View(user);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        TempData["Success"] = "✅ Пользователь удалён!";
        return RedirectToAction(nameof(Index));
    }
}
