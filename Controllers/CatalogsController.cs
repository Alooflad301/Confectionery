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
    public class CatalogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatalogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Catalogs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Catalog.Include(c => c.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Catalogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalog = await _context.Catalog
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id_Catalog == id);
            if (catalog == null)
            {
                return NotFound();
            }

            return View(catalog);
        }

        // GET: Catalogs/Create
        public IActionResult Create()
        {
            ViewData["Id_Ctegory"] = new SelectList(_context.Categories, "Id_Category", "Id_Category");
            return View();
        }

        // POST: Catalogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_Catalog,Product,Id_Ctegory")] Catalog catalog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(catalog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id_Ctegory"] = new SelectList(_context.Categories, "Id_Category", "Id_Category", catalog.Id_Ctegory);
            return View(catalog);
        }

        // GET: Catalogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalog = await _context.Catalog.FindAsync(id);
            if (catalog == null)
            {
                return NotFound();
            }
            ViewData["Id_Ctegory"] = new SelectList(_context.Categories, "Id_Category", "Id_Category", catalog.Id_Ctegory);
            return View(catalog);
        }

        // POST: Catalogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_Catalog,Product,Id_Ctegory")] Catalog catalog)
        {
            if (id != catalog.Id_Catalog)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(catalog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatalogExists(catalog.Id_Catalog))
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
            ViewData["Id_Ctegory"] = new SelectList(_context.Categories, "Id_Category", "Id_Category", catalog.Id_Ctegory);
            return View(catalog);
        }

        // GET: Catalogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalog = await _context.Catalog
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id_Catalog == id);
            if (catalog == null)
            {
                return NotFound();
            }

            return View(catalog);
        }

        // POST: Catalogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var catalog = await _context.Catalog.FindAsync(id);
            if (catalog != null)
            {
                _context.Catalog.Remove(catalog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CatalogExists(int id)
        {
            return _context.Catalog.Any(e => e.Id_Catalog == id);
        }
    }
}
