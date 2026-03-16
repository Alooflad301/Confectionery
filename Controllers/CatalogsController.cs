using Confectionery.Data;
using Confectionery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Confectionery.Controllers
{
    public class CatalogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatalogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchString, int? categoryId, int page = 1)
        {
            int pageSize = 12;
            var query = _context.Catalog
                .Include(c => c.Category)
                .AsQueryable();

            // 🔍 Поиск по названию товара
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Product.Contains(searchString));
                ViewBag.SearchString = searchString;
            }

            // Фильтр по категории
            if (categoryId.HasValue)
            {
                query = query.Where(c => c.Id_Ctegory == categoryId.Value);
                ViewBag.CategoryId = categoryId;
            }

            var totalItems = await query.CountAsync();
            var catalogs = await query
                .OrderBy(c => c.Product)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(catalogs);
        }


        public async Task<IActionResult> Details(int id)
        {
            var catalog = await _context.Catalog
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.Id_Catalog == id);

            if (catalog == null) return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(catalog);
        }

        [HttpPost]
        public async Task<IActionResult> AddToBasket(int catalogId, int quantity = 1)
        {
            // Проверка авторизации
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ReturnUrl"] = $"/Catalog/Details/{catalogId}";
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            // Получаем корзину пользователя или создаем новую
            var basket = await _context.Baskets
                .Include(b => b.BasketCatalogs)
                .ThenInclude(bc => bc.Catalog)
                .FirstOrDefaultAsync(b => b.Id_User == userId);

            if (basket == null)
            {
                basket = new Basket
                {
                    Id_User = userId,
                    Total_Price = 0
                };
                _context.Baskets.Add(basket);
                await _context.SaveChangesAsync();
            }

            // Проверяем, есть ли уже товар в корзине
            var basketItem = basket.BasketCatalogs
                ?.FirstOrDefault(bc => bc.Id_Catalog == catalogId);

            var catalog = await _context.Catalog.FindAsync(catalogId);
            if (catalog == null)
            {
                TempData["Error"] = "Товар не найден!";
                return RedirectToAction("Index");
            }

            if (basketItem == null)
            {
                // Новый товар
                basketItem = new BasketCatalog
                {
                    Id_Basket = basket.Id_Basket,
                    Id_Catalog = catalogId
                };
                _context.BasketCatalogs.Add(basketItem);
            }
            else
            {
                // Увеличиваем количество
                basketItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
            await RecalculateBasketTotal(basket.Id_Basket);

            TempData["Success"] = $"✅ {catalog.Product} ({quantity} шт) добавлен в корзину!";
            return RedirectToAction("Index");
        }

        // Пересчет общей суммы корзины
        private async Task RecalculateBasketTotal(int basketId)
        {
            var basket = await _context.Baskets
                .Include(b => b.BasketCatalogs)
                .ThenInclude(bc => bc.Catalog)
                .FirstAsync(b => b.Id_Basket == basketId);

            basket.Total_Price = basket.BasketCatalogs!
                .Sum(bc => bc.Quantity * bc.Catalog.Price);

            await _context.SaveChangesAsync();
        }
        // GET: /Catalog/Basket
        public async Task<IActionResult> Basket()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var basket = await _context.Baskets
                .Include(b => b.BasketCatalogs)
                .ThenInclude(bc => bc.Catalog)
                .ThenInclude(c => c.Category)
                .FirstOrDefaultAsync(b => b.Id_User == userId);

            return View(basket?.BasketCatalogs ?? new List<BasketCatalog>());
        }

        // POST: Удалить из корзины
        [HttpPost]
        public async Task<IActionResult> RemoveFromBasket(int basketCatalogId)
        {
            var item = await _context.BasketCatalogs.FindAsync(basketCatalogId);
            if (item != null)
            {
                _context.BasketCatalogs.Remove(item);
                await _context.SaveChangesAsync();
                await RecalculateBasketTotal(item.Id_Basket);
            }
            return RedirectToAction("Basket");
        }


    }
}
