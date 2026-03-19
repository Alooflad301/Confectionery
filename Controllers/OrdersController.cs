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
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = _context.Orders
                .Include(o => o.StatusOrder)
                .Include(o => o.User);
            return View(await orders.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.StatusOrder)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id_Order == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Edit/5
        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.StatusOrder)
                .FirstOrDefaultAsync(m => m.Id_Order == id);

            if (order == null)
                return NotFound();

            ViewBag.Id_StatusOrder = new SelectList(_context.StatusOrders, "Id_StatusOrder", "Name", order.Id_StatusOrder);
            return View(order);
        }

        // POST: Orders/Edit/5 — ✅ РАБОЧИЙ!
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // ✅ Берем статус ИЗ ФОРМЫ напрямую
            if (Request.Form["Id_StatusOrder"].Count > 0 &&
                int.TryParse(Request.Form["Id_StatusOrder"], out int newStatusId))
            {
                existingOrder.Id_StatusOrder = newStatusId;
                await _context.SaveChangesAsync();

                TempData["Success"] = "✅ Статус заказа обновлен!";
                return RedirectToAction("Details", new { id });
            }

            return BadRequest("Ошибка обновления статуса");
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id_Order == id);
        }
    }
}
