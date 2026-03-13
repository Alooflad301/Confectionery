using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Confectionery.Data;
using Confectionery.Models;

namespace Confectionery.Controllers
{
    public class BasketCatalogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BasketCatalogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BasketCatalogs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BasketCatalogs.Include(b => b.Basket).Include(b => b.Catalog);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BasketCatalogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basketCatalog = await _context.BasketCatalogs
                .Include(b => b.Basket)
                .Include(b => b.Catalog)
                .FirstOrDefaultAsync(m => m.Id_BasketCatalog == id);
            if (basketCatalog == null)
            {
                return NotFound();
            }

            return View(basketCatalog);
        }

        // GET: BasketCatalogs/Create
        public IActionResult Create()
        {
            ViewData["Id_Basket"] = new SelectList(_context.Baskets, "Id_Basket", "Id_Basket");
            ViewData["Id_Catalog"] = new SelectList(_context.Catalog, "Id_Catalog", "Product");
            return View();
        }

        // POST: BasketCatalogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_BasketCatalog,Id_Basket,Id_Catalog")] BasketCatalog basketCatalog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(basketCatalog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id_Basket"] = new SelectList(_context.Baskets, "Id_Basket", "Id_Basket", basketCatalog.Id_Basket);
            ViewData["Id_Catalog"] = new SelectList(_context.Catalog, "Id_Catalog", "Product", basketCatalog.Id_Catalog);
            return View(basketCatalog);
        }

        // GET: BasketCatalogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basketCatalog = await _context.BasketCatalogs.FindAsync(id);
            if (basketCatalog == null)
            {
                return NotFound();
            }
            ViewData["Id_Basket"] = new SelectList(_context.Baskets, "Id_Basket", "Id_Basket", basketCatalog.Id_Basket);
            ViewData["Id_Catalog"] = new SelectList(_context.Catalog, "Id_Catalog", "Product", basketCatalog.Id_Catalog);
            return View(basketCatalog);
        }

        // POST: BasketCatalogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_BasketCatalog,Id_Basket,Id_Catalog")] BasketCatalog basketCatalog)
        {
            if (id != basketCatalog.Id_BasketCatalog)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(basketCatalog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BasketCatalogExists(basketCatalog.Id_BasketCatalog))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id_Basket"] = new SelectList(_context.Baskets, "Id_Basket", "Id_Basket", basketCatalog.Id_Basket);
            ViewData["Id_Catalog"] = new SelectList(_context.Catalog, "Id_Catalog", "Product", basketCatalog.Id_Catalog);
            return View(basketCatalog);
        }

        // GET: BasketCatalogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basketCatalog = await _context.BasketCatalogs
                .Include(b => b.Basket)
                .Include(b => b.Catalog)
                .FirstOrDefaultAsync(m => m.Id_BasketCatalog == id);
            if (basketCatalog == null)
            {
                return NotFound();
            }

            return View(basketCatalog);
        }

        // POST: BasketCatalogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var basketCatalog = await _context.BasketCatalogs.FindAsync(id);
            if (basketCatalog != null)
            {
                _context.BasketCatalogs.Remove(basketCatalog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BasketCatalogExists(int id)
        {
            return _context.BasketCatalogs.Any(e => e.Id_BasketCatalog == id);
        }
    }
}
