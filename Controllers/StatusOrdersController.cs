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
    public class StatusOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatusOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StatusOrders
        public async Task<IActionResult> Index()
        {
            return View(await _context.StatusOrders.ToListAsync());
        }

        // GET: StatusOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusOrder = await _context.StatusOrders
                .FirstOrDefaultAsync(m => m.Id_StatusOrder == id);
            if (statusOrder == null)
            {
                return NotFound();
            }

            return View(statusOrder);
        }

        // GET: StatusOrders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StatusOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_StatusOrder,Name")] StatusOrder statusOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(statusOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(statusOrder);
        }

        // GET: StatusOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusOrder = await _context.StatusOrders.FindAsync(id);
            if (statusOrder == null)
            {
                return NotFound();
            }
            return View(statusOrder);
        }

        // POST: StatusOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_StatusOrder,Name")] StatusOrder statusOrder)
        {
            if (id != statusOrder.Id_StatusOrder)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statusOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatusOrderExists(statusOrder.Id_StatusOrder))
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
            return View(statusOrder);
        }

        // GET: StatusOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusOrder = await _context.StatusOrders
                .FirstOrDefaultAsync(m => m.Id_StatusOrder == id);
            if (statusOrder == null)
            {
                return NotFound();
            }

            return View(statusOrder);
        }

        // POST: StatusOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var statusOrder = await _context.StatusOrders.FindAsync(id);
            if (statusOrder != null)
            {
                _context.StatusOrders.Remove(statusOrder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StatusOrderExists(int id)
        {
            return _context.StatusOrders.Any(e => e.Id_StatusOrder == id);
        }
    }
}
