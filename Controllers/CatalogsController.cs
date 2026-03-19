using Confectionery.Data;
using Confectionery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization; 

namespace Confectionery.Controllers
{
    public class CatalogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatalogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string sortOrder,
    int? categoryId, decimal? minPrice, decimal? maxPrice, int? page)
        {

            // 🔥 СОРТИРОВКА
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_asc" : "";
            ViewBag.PriceSortParm = sortOrder == "price_asc" ? "price_desc" : "price_asc";

            // Базовый запрос
            var catalogs = _context.Catalog.Include(c => c.Category).AsQueryable();

            // Поиск
            if (!string.IsNullOrEmpty(searchString))
            {
                catalogs = catalogs.Where(c => c.Product.Contains(searchString));
                ViewBag.SearchString = searchString;
            }

            // Категория
            if (categoryId.HasValue)
            {
                catalogs = catalogs.Where(c => c.Id_Ctegory == categoryId);
                ViewBag.CategoryId = categoryId;
            }

            // Цена
            if (minPrice.HasValue)
            {
                catalogs = catalogs.Where(c => c.Price >= minPrice.Value);
                ViewBag.MinPrice = minPrice;
            }
            if (maxPrice.HasValue)
            {
                catalogs = catalogs.Where(c => c.Price <= maxPrice.Value);
                ViewBag.MaxPrice = maxPrice;
            }

            // 🔥 СОРТИРОВКА
            switch (sortOrder)
            {
                case "name_asc":
                    catalogs = catalogs.OrderBy(c => c.Product);
                    break;
                case "name_desc":
                    catalogs = catalogs.OrderByDescending(c => c.Product);
                    break;
                case "price_asc":
                    catalogs = catalogs.OrderBy(c => c.Price);
                    break;
                case "price_desc":
                    catalogs = catalogs.OrderByDescending(c => c.Price);
                    break;
                default:
                    catalogs = catalogs.OrderBy(c => c.Id_Catalog);
                    break;
            }

            // Пагинация
            int pageSize = 12;
            ViewBag.CurrentPage = page ?? 1;
            var totalItems = await catalogs.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            int currentPage = page ?? 1;
            catalogs = catalogs.Skip((currentPage - 1) * pageSize).Take(pageSize);


            var model = await catalogs.ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View(model);
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
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ReturnUrl"] = $"/Catalog/Details/{catalogId}";
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var basket = await _context.Baskets
                .Include(b => b.BasketCatalogs)
                .ThenInclude(bc => bc.Catalog)
                .FirstOrDefaultAsync(b => b.Id_User == userId);

            if (basket == null)
            {
                basket = new Basket { Id_User = userId, Total_Price = 0 };
                _context.Baskets.Add(basket);
                await _context.SaveChangesAsync();
            }

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
                // ✅ НОВЫЙ ТОВАР - устанавливаем quantity
                basketItem = new BasketCatalog
                {
                    Id_Basket = basket.Id_Basket,
                    Id_Catalog = catalogId,
                    Quantity = quantity  // ← ГЛАВНОЕ ИСПРАВЛЕНИЕ!
                };
                _context.BasketCatalogs.Add(basketItem);
            }
            else
            {
                // ✅ СУЩЕСТВУЮЩИЙ - добавляем quantity
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
        [HttpPost]
        public async Task<IActionResult> UpdateBasketItem(int basketCatalogId, int quantity)
        {
            if (quantity <= 0)
            {
                // Удаляем если quantity = 0
                var item = await _context.BasketCatalogs.FindAsync(basketCatalogId);
                if (item != null)
                {
                    _context.BasketCatalogs.Remove(item);
                    await _context.SaveChangesAsync();
                    await RecalculateBasketTotal(item.Id_Basket);
                }
            }
            else
            {
                // Обновляем количество
                var item = await _context.BasketCatalogs.FindAsync(basketCatalogId);
                if (item != null)
                {
                    item.Quantity = quantity;
                    await _context.SaveChangesAsync();
                    await RecalculateBasketTotal(item.Id_Basket);
                }
            }

            return RedirectToAction("Basket");
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                TempData["Error"] = "Ошибка авторизации";
                return RedirectToAction("Basket");
            }

            var basket = await _context.Baskets
                .Include(b => b.BasketCatalogs)
                .ThenInclude(bc => bc.Catalog)
                .FirstOrDefaultAsync(b => b.Id_User == userId);

            if (basket?.BasketCatalogs == null || !basket.BasketCatalogs.Any())
            {
                TempData["Error"] = "Корзина пуста";
                return RedirectToAction("Basket");
            }

            // ✅ Создаем заказ
            var order = new Order
            {
                Id_User = userId,
                Id_StatusOrder = 1, // Новый
                Date = DateTime.Now,
                Price = basket.BasketCatalogs.Sum(bc => bc.Quantity * bc.Catalog.Price)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // ✅ СОХРАНЯЕМ ТОВАРЫ В OrderCatalogs
            foreach (var item in basket.BasketCatalogs)
            {
                var orderCatalog = new OrderCatalog
                {
                    Id_Order = order.Id_Order,
                    Id_Catalog = item.Id_Catalog,
                    Quantity = item.Quantity
                };
                _context.OrderCatalogs.Add(orderCatalog);
            }

            // ✅ Очищаем корзину
            _context.BasketCatalogs.RemoveRange(basket.BasketCatalogs);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Заказ #{order.Id_Order} успешно создан!";
            return RedirectToAction("Basket");
        }
        // GET: Catalogs/Edit/5
        // GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var catalog = await _context.Catalog.FindAsync(id);
            if (catalog == null) return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(catalog);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Catalog catalog)
        {
            ModelState.Remove("OrderCatalogs");

            if (ModelState.IsValid)
            {
                var existing = await _context.Catalog.FindAsync(id);
                if (existing == null) return NotFound();

                existing.Product = catalog.Product;
                existing.Id_Ctegory = catalog.Id_Ctegory;
                existing.Description = catalog.Description;
                existing.Price = catalog.Price;
                existing.PhotoPath = catalog.PhotoPath;

                // Загружаем новую картинку если путь указан
                if (!string.IsNullOrEmpty(catalog.PhotoPath))
                {
                    var imagePath = Path.Combine("wwwroot/images", catalog.PhotoPath);
                    if (System.IO.File.Exists(imagePath))
                    {
                        existing.Photo = await System.IO.File.ReadAllBytesAsync(imagePath);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "✅ Обновлено!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(catalog);
        }


        // GET: Catalogs/Create
        // GET: Catalogs/Create
        // GET: Catalogs/Create
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: Catalogs/Create
        [HttpPost]
        public async Task<IActionResult> Create(Catalog catalog)
        {
            ModelState.Remove("OrderCatalogs");

            if (ModelState.IsValid)
            {
                // Загружаем картинку если путь указан
                if (!string.IsNullOrEmpty(catalog.PhotoPath))
                {
                    var imagePath = Path.Combine("wwwroot/images", catalog.PhotoPath);
                    if (System.IO.File.Exists(imagePath))
                    {
                        catalog.Photo = await System.IO.File.ReadAllBytesAsync(imagePath);
                    }
                }

                _context.Add(catalog);
                await _context.SaveChangesAsync();

                TempData["Success"] = "✅ Добавлено!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(catalog);
        }
        // 🔥 GET: /Catalogs/Delete/10 - ПОКАЗЫВАЕТ форму подтверждения
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var catalog = await _context.Catalog
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id_Catalog == id);

            if (catalog == null) return NotFound();

            return View(catalog);
        }

        // 🔥 POST: /Catalogs/DeleteConfirmed/10 - УДАЛЯЕТ товар
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var catalog = await _context.Catalog.FindAsync(id);
            if (catalog != null)
            {
                _context.Catalog.Remove(catalog);
                await _context.SaveChangesAsync();
                TempData["Success"] = "✅ Товар удален!";
            }
            else
            {
                TempData["Error"] = "❌ Товар не найден!";
            }

            return RedirectToAction(nameof(Index));
        }










    }
}
