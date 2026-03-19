using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Confectionery.Data;
using Confectionery.Models;

namespace Confectionery.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "✅ Категория добавлена!";
                    return RedirectToAction("Index", "Catalogs");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Ошибка: {ex.Message}";
                }
            }

            return View(category);
        }



    }
}
